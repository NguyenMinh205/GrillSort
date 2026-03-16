using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _panelWin;
    [SerializeField] private GameObject _panelLose;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI _txtTimer;

    private float _timer;
    private bool _isTimerRunning = false;
    private bool _isAddingTime = false;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI _txtMealFinish;

    [Header("Hint System")]
    [SerializeField] private float _timeToWaitHint = 5f;
    private float _idleTimer = 0f;

    public void Init(float durationTimer)
    {
        _timer = durationTimer;
        UpdateTimerText();

        ObserverManager<GameEvent>.AddRegisterEvent(GameEvent.OnStartGame, OnGameStartTriggered);
        ObserverManager<GameEvent>.AddRegisterEvent(GameEvent.OnPlayerAction, OnPlayerActionResetTimer);
    }

    private void OnDestroy()
    {
        ObserverManager<GameEvent>.RemoveAddListener(GameEvent.OnStartGame, OnGameStartTriggered);
        ObserverManager<GameEvent>.RemoveAddListener(GameEvent.OnPlayerAction, OnPlayerActionResetTimer);
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

        if (_isAddingTime) return;

        _timer -= Time.deltaTime;
        UpdateTimerText();

        if (_timer <= 0)
        {
            _timer = 0;
            _isTimerRunning = false;
            GameplayManager.Instance.EndGame(false);
        }

        _idleTimer += Time.deltaTime;
        if (_idleTimer >= _timeToWaitHint)
        {
            _idleTimer = 0f;
            GameplayManager.Instance.TryShowHint();
        }
    }

    public void UpdateTimerText()
    {
        float minutes = Mathf.FloorToInt(_timer / 60);
        float seconds = Mathf.FloorToInt(_timer % 60);
        _txtTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void AddExtraTime(float extraTime)
    {
        _isAddingTime = true;
        float targetTime = _timer + extraTime;

        _txtTimer.transform.DOKill(true);
        _txtTimer.transform.localScale = Vector3.one;

        _txtTimer.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0f), 1f, 5, 0.5f);
        DOTween.To(() => _timer, x =>
        {
            _timer = x;
            UpdateTimerText();
        }, targetTime, 1f).SetEase(Ease.OutQuad).OnComplete(() => {
            _isAddingTime = false;
        });
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