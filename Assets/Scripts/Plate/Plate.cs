using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plate : MonoBehaviour
{
    [SerializeField] private List<Image> _listFood;

    private void Awake()
    {
        for (int i = 0; i < _listFood.Count; i++)
        {
            _listFood[i].gameObject.SetActive(false);
        }
    }

    public void OnSetListFood(List<Sprite> listFood)
    {
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
}
