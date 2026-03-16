using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameplayManager : Singleton<GameplayManager>
{
    [Header("Manager")]
    [SerializeField] private UIManager _uiManager;
    public UIManager UIManager => _uiManager;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private BoosterManager _boosterManager;

    [Header("Stats Level")]
    [SerializeField] private int _totalMeal;
    [SerializeField] private int _totalTypeOfFood;
    [SerializeField] private int _totalGrill;
    [SerializeField] private float _durationLevel = 180f;
    public float DurationLevel => _durationLevel;
    public int TotalMeals => _totalMeal;
    [SerializeField] private List<Grill> _listGrill;
    public List<Grill> ListGrill => _listGrill;
    [SerializeField] private List<Sprite> _listSpriteFood;

    private float _argFoodInPlate;
    private int _currentMealFinish = 0;
    private GameState _gameState;
    public GameState GameState { get => _gameState; set => _gameState = value; }

    private void Start()
    {
        OnInitLevel();
        _uiManager.Init(_durationLevel);
        ObserverManager<GameEvent>.AddRegisterEvent(GameEvent.OnStartGame, OnPlayerStart);
        ObserverManager<GameEvent>.AddRegisterEvent(GameEvent.OnDoneGrill, OnMealFinish);

        if (_uiManager != null)
        {
            _uiManager.UpdateMealFinishText(_currentMealFinish, TotalMeals);
        }
    }

    private void OnDestroy()
    {
        ObserverManager<GameEvent>.RemoveAddListener(GameEvent.OnStartGame, OnPlayerStart);
        ObserverManager<GameEvent>.RemoveAddListener(GameEvent.OnDoneGrill, OnMealFinish);
    }

    public void OnInitLevel()
    {
        List<Sprite> takenFood = _listSpriteFood.OrderBy(x => Random.value).Take(_totalTypeOfFood).ToList();
        List<Sprite> usedFood = new List<Sprite>();

        for (int i = 0; i < _totalMeal; i++)
        {
            int n = i % takenFood.Count;
            for (int j = 0; j < 3; j++)
            {
                usedFood.Add(takenFood[n]);
            }
        }

        for (int i = 0; i < usedFood.Count; i++)
        {
            int randomIndex = Random.Range(0, usedFood.Count);
            Sprite temp = usedFood[randomIndex];
            usedFood[randomIndex] = usedFood[i];
            usedFood[i] = temp;
        }

        _argFoodInPlate = Random.Range(1.5f, 2.5f);
        int totalPlate = Mathf.RoundToInt(usedFood.Count / _argFoodInPlate);

        List<int> platePerGrill = DistributeEvelyn(_totalGrill, totalPlate);
        List<int> foodPerGrill = DistributeEvelyn(_totalGrill, usedFood.Count);

        for (int i = 0; i < _listGrill.Count; i++)
        {
            bool activeGrill = i < _totalGrill;
            _listGrill[i].gameObject.SetActive(activeGrill);

            if (activeGrill)
            {
                List<Sprite> listFood = Utils.TakeAndRemoveRandom(usedFood, foodPerGrill[i]);
                _listGrill[i].OnInitGrill(platePerGrill[i], listFood);
            }
        }

        _gameState = GameState.Waiting;
        _currentMealFinish = 0;
    }

    public List<int> DistributeEvelyn(int totalGrill, int totalPlate)
    {
        List<int> evelynDistribution = new List<int>();
        float avg = (float)totalPlate / totalGrill;
        int lowerBound = Mathf.FloorToInt(avg);
        int upperBound = Mathf.CeilToInt(avg);

        int highCount = totalPlate - (lowerBound * totalGrill);
        int lowCount = totalGrill - highCount;

        for (int i = 0; i < lowCount; i++)
        {
            evelynDistribution.Add(lowerBound);
        }

        for (int i = 0; i < highCount; i++)
        {
            evelynDistribution.Add(upperBound);
        }

        for (int i = 0; i < evelynDistribution.Count; i++)
        {
            int randomIndex = Random.Range(0, evelynDistribution.Count);
            int temp = evelynDistribution[randomIndex];
            evelynDistribution[randomIndex] = evelynDistribution[i];
            evelynDistribution[i] = temp;
        }

        return evelynDistribution;
    }

    private void OnPlayerStart(object param)
    {
        GameState = GameState.Playing;
    }

    private void OnMealFinish(object param)
    {
        _currentMealFinish++;
        if (_uiManager != null)
        {
            _uiManager.UpdateMealFinishText(_currentMealFinish, TotalMeals);
        }

        if (_currentMealFinish >= TotalMeals)
        {
            EndGame(true);
        }
    }

    public void EndGame(bool isWin)
    {
        GameState = isWin ? GameState.Winning : GameState.Losing;
        if (isWin)
        {
            _uiManager.ShowUIWin();
        }
        else
        {
            _uiManager.ShowUILose();
        }
    }

    public bool TryShowHint()
    {
        List<Grill> activeGrills = _listGrill.Where(g => g.gameObject.activeSelf && !g.IsCompletelyEmpty() && !g.IsDoneGrill()).ToList();
        Dictionary<Sprite, List<SlotFood>> foodToSlotsMap = new Dictionary<Sprite, List<SlotFood>>();

        for (int i = 0; i < activeGrills.Count; i++)
        {
            for (int j = 0; j < activeGrills[i].GetFilledSlots().Count; j++)
            {
                SlotFood slotFood = activeGrills[i].GetFilledSlots()[j];
                Sprite foodSprite = slotFood.ImageFood.sprite;
                if (!foodToSlotsMap.ContainsKey(foodSprite))
                {
                    foodToSlotsMap[foodSprite] = new List<SlotFood>();
                }
                foodToSlotsMap[foodSprite].Add(slotFood);
            }
        }

        foreach (var typeOfFood in foodToSlotsMap)
        {
            if (typeOfFood.Value.Count >= 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    typeOfFood.Value[i].PlayHintAnimation();
                }
                return true;
            }
        }

        return false;
    }
}