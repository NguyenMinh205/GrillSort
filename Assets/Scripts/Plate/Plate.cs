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
            Debug.LogError("List food count does not match the number of food images on the plate.");
            return;
        }

        for (int i = 0; i < listFood.Count; i++)
        {
            _listFood[i].sprite = listFood[i];
            _listFood[i].gameObject.SetActive(true);
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