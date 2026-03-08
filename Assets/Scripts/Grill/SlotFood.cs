using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotFood : MonoBehaviour
{
    [SerializeField] private Image _imageFood;
    [SerializeField] private Grill _grillCtrl;
    public Grill GrillCtrl => _grillCtrl;

    private Color _defaultColor = new Color(1f, 1f, 1f, 1f);
    private Color _fadeColor = new Color(1f, 1f, 1f, 0.5f);
    public Image ImageFood => _imageFood;

    public void OnSetFood(Sprite sprite)
    {
        _imageFood.sprite = sprite;
        _imageFood.gameObject.SetActive(true);
        _imageFood.enabled = true;

        _imageFood.transform.localPosition = Vector3.zero;
        _imageFood.transform.localScale = Vector3.one;
    }

    public bool IsEmpty()
    {
        return _imageFood.enabled == false || _imageFood.color != _defaultColor;
    }

    public void OnActiveFood(bool active)
    {
        _imageFood.enabled = active;
        _imageFood.color = _defaultColor;
    }

    public void OnFadeFood()
    {
        OnActiveFood(true);
        _imageFood.color = _fadeColor;
    }

    public void OnHideFood()
    {
        OnActiveFood(false);
        _imageFood.color = _defaultColor;
    }

    public SlotFood GetSlotNull => _grillCtrl.GetSlotNull();

    public void OnClearSlot()
    {
        _imageFood.sprite = null;
        _imageFood.enabled = false;
    }

    public void PlayHintAnimation()
    {
        if (_imageFood == null || !_imageFood.gameObject.activeSelf) return;

        _imageFood.transform.DOKill();

        _imageFood.transform.localScale = Vector3.one;
        _imageFood.transform.localPosition = Vector3.zero;
        _imageFood.transform.localRotation = Quaternion.identity;

        Sequence hintSeq = DOTween.Sequence();

        hintSeq.Append(_imageFood.transform.DOScale(new Vector3(1.25f, 1.25f, 1f), 0.15f).SetEase(Ease.OutQuad));

        hintSeq.Append(_imageFood.transform.DOShakeRotation(0.5f, new Vector3(0f, 0f, 25f), 15, 90f, false));

        hintSeq.Append(_imageFood.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.InQuad));

        hintSeq.OnComplete(() => {
            _imageFood.transform.localScale = Vector3.one;
            _imageFood.transform.localRotation = Quaternion.identity;
            _imageFood.transform.localPosition = Vector3.zero;
        });
    }
}