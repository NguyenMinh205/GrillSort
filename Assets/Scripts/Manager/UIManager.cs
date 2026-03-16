using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _panelWin;
    [SerializeField] private GameObject _panelLose;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI _txtTimer;
    [SerializeField] private float _durationTimer = 60f;
    public float DurationTimer
    {
        get => _durationTimer;
        set
        {
            _durationTimer = value;
            UpdateTimerText();
        }
    }
    private float _timer;
    private bool _isTimerRunning = false;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI _txtMealFinish;

    [Header("Hint System")]
    [SerializeField] private float _timeToWaitHint = 5f;
    private float _idleTimer = 0f;

    private void OnEnable()
    {
        ObserverManager<GameEvent>.AddRegisterEvent(GameEvent.OnStartGame, OnGameStartTriggered);
        ObserverManager<GameEvent>.AddRegisterEvent(GameEvent.OnPlayerAction, OnPlayerActionResetTimer);
    }

    private void OnDestroy()
    {
        ObserverManager<GameEvent>.RemoveAddListener(GameEvent.OnStartGame, OnGameStartTriggered);
        ObserverManager<GameEvent>.RemoveAddListener(GameEvent.OnPlayerAction, OnPlayerActionResetTimer);
    }

    public void Init()
    {
        _timer = _durationTimer;
        UpdateTimerText();

    }

    private void OnGameStartTriggered(object param)
    {
        _isTimerRunning = true;
        _idleTimer = 0f;
    }

    private void OnPlayerActionResetTimer(object param)
    {
        _idleTimer = 0f;
    }

    private void Update()
    {
        if (!_isTimerRunning || GameplayManager.Instance.GameState != GameState.Playing) return;

        _timer -= Time.deltaTime;
        UpdateTimerText();

        if (_timer <= 0)
        {
            _timer = 0;
            UpdateTimerText();
            _isTimerRunning = false;
            GameplayManager.Instance.EndGame(false);
        }

        _idleTimer += Time.deltaTime;
        if (_idleTimer >= _timeToWaitHint)
        {
            _idleTimer = _timeToWaitHint/3;
            GameplayManager.Instance.TryShowHint();
        }
    }

    public void UpdateTimerText()
    {
        float minutes = Mathf.FloorToInt(_timer / 60);
        float seconds = Mathf.FloorToInt(_timer % 60);
        _txtTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UpdateMealFinishText(int currentMealFinish, int totalMeal)
    {
        _txtMealFinish.text = $"{currentMealFinish}/{totalMeal}";
    }

    public void ShowUIWin()
    {
        _isTimerRunning = false;
        if (_panelWin) _panelWin.SetActive(true);
    }

    public void ShowUILose()
    {
        _isTimerRunning = false;
        if (_panelLose) _panelLose.SetActive(true);
    }
}