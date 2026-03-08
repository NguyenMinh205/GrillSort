using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DragDropController : MonoBehaviour
{
    [SerializeField] private Image _imgFood;
    [SerializeField] private float _durationTransition = 0.25f;

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
            if (targetSlot != null)
            {
                if (targetSlot.IsEmpty())
                {
                    if (_tempSlot != null)
                    {
                        if (targetSlot.GetInstanceID() != _tempSlot.GetInstanceID())
                        {
                            _tempSlot.OnHideFood();
                            _tempSlot = targetSlot;
                            _tempSlot.OnSetFood(_currentSlot.ImageFood.sprite);
                            _tempSlot.OnFadeFood();
                        }
                    }
                    else
                    {
                        _tempSlot = targetSlot;
                        _tempSlot.OnSetFood(_currentSlot.ImageFood.sprite);
                        _tempSlot.OnFadeFood();
                    }
                }
                else
                {
                    SlotFood emptySlot = targetSlot.GetSlotNull;
                    if (emptySlot != null && emptySlot.GetInstanceID() != _tempSlot.GetInstanceID())
                    {
                        _tempSlot.OnHideFood();
                        _tempSlot = emptySlot;
                        _tempSlot.OnSetFood(_currentSlot.ImageFood.sprite);
                        _tempSlot.OnFadeFood();
                    }
                }
            }
            else
            {
                OnClearTempSlot();
            }
        }

        if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            if (_tempSlot != null && _tempSlot.GetInstanceID() != _currentSlot.GetInstanceID())
            {
                _imgFood.transform.DOMove(_tempSlot.transform.position, _durationTransition).OnComplete(() => {
                    _tempSlot.OnSetFood(_currentSlot.ImageFood.sprite);
                    _tempSlot.OnActiveFood(true);

                    _currentSlot.OnClearSlot();
                    _currentSlot.GrillCtrl.OnCheckEmptyGrill();

                    _tempSlot.GrillCtrl.OnCheckDoneGrill();

                    _tempSlot = null;
                    _currentSlot = null;
                    _imgFood.gameObject.SetActive(false);
                    _imgFood.sprite = null;
                });
            }
            else
            {
                _imgFood.transform.DOMove(_currentSlot.transform.position, _durationTransition).OnComplete(() => {
                    _currentSlot.OnActiveFood(true);
                    OnClearTempSlot();
                    _imgFood.gameObject.SetActive(false);
                    _imgFood.sprite = null;
                    _currentSlot = null;
                });
            }

            _isDragging = false;
        }
    }

    public void OnClearTempSlot()
    {
        if (_tempSlot != null && _currentSlot != null)
        {
            if (_currentSlot.GetInstanceID() != _tempSlot.GetInstanceID())
            {
                _tempSlot.OnHideFood();
                _tempSlot = null;
            }
        }
    }
}