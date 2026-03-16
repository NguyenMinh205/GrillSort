using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleSwitch : MonoBehaviour, IPointerClickHandler
{
    [Header("Slider Value")]
    [SerializeField] private Slider _slider;
    [SerializeField, Range(0,1)] private float _sliderValue = 0;

    public bool CurrentValue { get; private set; }

    [Header("Animations")]
    [SerializeField] private float _animationDuration = 0.5f;
    [SerializeField] private AnimationCurve _sliderAnimationCurve = AnimationCurve.EaseInOut(0,0,1,1);
    private Coroutine _sliderCoroutine;

    [Header("Events")]
    public UnityEvent OnToggleOn;
    public UnityEvent OnToggleOff;

    public void OnValidate()
    {
        SetUpToggle();

        if (_slider != null)
        {
            _slider.value = _sliderValue;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Toggle();
    }
    public void SetUpToggle()
    {
        _slider.interactable = false;
        var sliderColors = _slider.colors;
        sliderColors.disabledColor = Color.white;
        _slider.colors = sliderColors;
        _slider.transition = Selectable.Transition.None;
    }

    public void Toggle()
    {
        if (_sliderCoroutine != null)
        {
            StopCoroutine(_sliderCoroutine);
        }

        SetStateAndStartAnimation(!CurrentValue);
    }    

    public void SetStateAndStartAnimation(bool state)
    {
        CurrentValue = state;

        if (CurrentValue)
        {
            OnToggleOn?.Invoke();
        }
        else
        {
            OnToggleOff.Invoke();
        }

        _sliderCoroutine = StartCoroutine(AnimateSlider());
    }

    private IEnumerator AnimateSlider()
    {
        float elapsedTime = 0;
        float startValue = _slider.value;
        float targetValue = CurrentValue ? 1 : 0;
        while (elapsedTime < _animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / _animationDuration);
            _slider.value = Mathf.Lerp(startValue, targetValue, _sliderAnimationCurve.Evaluate(t));
            yield return null;
        }
        _slider.value = targetValue;
    }
}
