using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Plate : MonoBehaviour
{
    [SerializeField] private List<Image> _listFood;

    private void Awake()
    {
        OnClearPlate();
    }

    public void OnSetListFood(List<Sprite> listFood)
    {
        OnClearPlate();

        if (listFood.Count > _listFood.Count)
        {
            return;
        }

        for (int i = 0; i < listFood.Count; i++)
        {
            if (listFood[i] != null)
            {
                _listFood[i].sprite = listFood[i];
                _listFood[i].gameObject.SetActive(true);
            }
            else
            {
                _listFood[i].gameObject.SetActive(false);
                _listFood[i].sprite = null;
            }
        }
    }

    public void OnClearPlate()
    {
        for (int i = 0; i < _listFood.Count; i++)
        {
            _listFood[i].gameObject.SetActive(false);
            _listFood[i].sprite = null;
        }
    }

    public Vector3 GetFoodPosition(int index)
    {
        if (index >= 0 && index < _listFood.Count)
            return _listFood[index].transform.position;
        return transform.position;
    }

    public Sequence HideCurrentFoodsAnimation(float duration)
    {
        Sequence seq = DOTween.Sequence();
        foreach (var img in _listFood)
        {
            if (img.gameObject.activeSelf)
            {
                seq.Join(img.transform.DOScale(Vector3.zero, duration).SetEase(Ease.InBack));
            }
        }
        return seq;
    }

    public Sequence ShowCurrentFoodsAnimation(float duration)
    {
        Sequence seq = DOTween.Sequence();
        foreach (var img in _listFood)
        {
            if (img.gameObject.activeSelf)
            {
                img.transform.localScale = Vector3.zero;
                seq.Join(img.transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack));
            }
        }
        return seq;
    }

    public void AnimateShowNextFood(float duration)
    {
        foreach (var img in _listFood)
        {
            if (img.gameObject.activeSelf)
            {
                img.transform.localScale = Vector3.zero;
                img.transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack);
            }
        }
    }
}