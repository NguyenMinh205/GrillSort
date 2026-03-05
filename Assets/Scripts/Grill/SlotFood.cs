using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotFood : MonoBehaviour
{
    [SerializeField] private Image _imageFood;

    public void OnSetFood(Sprite sprite)
    {
        _imageFood.sprite = sprite;
        _imageFood.gameObject.SetActive(true);
        _imageFood.enabled = true;
    }

    public bool IsEmpty()
    {
        return _imageFood.sprite == null;
    }
     public void OnClearSlot()
    {
        _imageFood.sprite = null;
        _imageFood.enabled = false;
    }
}
