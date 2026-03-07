using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragDropController : MonoBehaviour
{
    [SerializeField] private Image _imgFood;

    private SlotFood _currentSlot, _tempSlot;
    private bool _isDragging;
    private Vector3 _offset;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _currentSlot = Utils.GetRayCast<SlotFood>(Input.mousePosition);
            if (_currentSlot != null && !_currentSlot.IsEmpty())
            {
                _isDragging = true;
                _tempSlot = _currentSlot;
                _imgFood.sprite = _currentSlot.ImageFood.sprite;
                _imgFood.gameObject.SetActive(true);
                _currentSlot.OnActiveFood(false);
                _imgFood.transform.position = _currentSlot.transform.position;

                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _offset = mouseWorldPos - _currentSlot.transform.position;
            }
        }

        if (Input.GetMouseButton(0) && _isDragging)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 foodPos = mouseWorldPos + _offset;
            foodPos.z = 0;
            _imgFood.transform.position = foodPos;

            SlotFood targetSlot = Utils.GetRayCast<SlotFood>(Input.mousePosition);
            if (targetSlot != null && targetSlot.IsEmpty() && targetSlot.GetInstanceID() != _tempSlot.GetInstanceID())
            {
                _tempSlot.OnHideFood();
                _tempSlot = targetSlot;
                _tempSlot.OnSetFood(_currentSlot.ImageFood.sprite);
                _tempSlot.OnFadeFood();
            }
        }

        if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            _tempSlot.OnHideFood();
            _currentSlot.OnActiveFood(true);
            _isDragging = false;
            _imgFood.gameObject.SetActive(false);
            _imgFood.sprite = null;
        }
    }
}
