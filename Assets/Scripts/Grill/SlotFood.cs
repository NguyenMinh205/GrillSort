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
}