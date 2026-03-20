using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ToggleSwitch : MonoBehaviour, IPointerClickHandler
{
    [Header("Slider Value")]
    [SerializeField] private Slider _slider;

    public bool CurrentValue { get; private set; }

    [Header("Visuals")]
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Sprite _toggleOnSprite;
    [SerializeField] private Sprite _toggleOffSprite;

    [Header("Animations")]
    [SerializeField] private float _animationDuration = 0.25f;
    [SerializeField] private Ease _animationEase = Ease.InOutQuad;

    [Header("Events")]
    public UnityEvent OnToggleOn;
    public UnityEvent OnToggleOff;

    private void Start()
    {
        UpdateVisuals(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Toggle();
    }

    public void Toggle()
    {
        SetStateAndStartAnimation(!CurrentValue);
    }

    public void SetStateAndStartAnimation(bool state)
    {
        if (CurrentValue == state) return;

        CurrentValue = state;

        float targetValue = CurrentValue ? 1f : 0f;
        _slider.DOKill();
        _slider.DOValue(targetValue, _animationDuration).SetEase(_animationEase);

        _backgroundImage.DOKill();

        _backgroundImage.sprite = CurrentValue ? _toggleOnSprite : _toggleOffSprite;

        if (CurrentValue) OnToggleOn?.Invoke();
        else OnToggleOff?.Invoke();
    }

    private void UpdateVisuals(bool animate)
    {
        float targetValue = CurrentValue ? 1f : 0f;
        _backgroundImage.sprite = CurrentValue ? _toggleOnSprite : _toggleOffSprite;

        if (animate)
        {
            _slider.DOValue(targetValue, _animationDuration);
        }
        else
        {
            _slider.value = targetValue;
        }
    }

    public void OnValidate()
    {
        if (_slider != null)
        {
            _slider.interactable = false;
            _slider.transition = Selectable.Transition.None;
        }
    }
}