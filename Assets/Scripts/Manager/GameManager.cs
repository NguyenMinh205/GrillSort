using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private int _totalTypeOfFood;
    [SerializeField] private int _totalGrill;
    [SerializeField] private int _totalMeals;
    [SerializeField] private List<Grill> _listGrill;
    [SerializeField] private List<Sprite> _listSpriteFood;

    private float _argFoodInPlate;

    private void Start()
    {
        OnInitLevel();
    }

    public void OnInitLevel()
    {
        List<Sprite> takenFood = _listSpriteFood.OrderBy(x => Random.value).Take(_totalTypeOfFood).ToList();
        List<Sprite> usedFood = new List<Sprite>();

        for (int i = 0; i < takenFood.Count; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                usedFood.Add(takenFood[i]);
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

}
