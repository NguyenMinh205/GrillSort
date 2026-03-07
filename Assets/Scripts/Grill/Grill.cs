using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grill : MonoBehaviour
{
    [SerializeField] private List<SlotFood> _slotFoods;
    [SerializeField] private Plate _plate;
    private List<List<Sprite>> _listFoodForPlate;

    public void OnInitGrill(int totalPlate, List<Sprite> listFood)
    {
        int foodCount = Random.Range(0, _slotFoods.Count + 1);
        List<Sprite> foodInGrill = Utils.TakeAndRemoveRandom(listFood, foodCount);

        for (int i = 0; i < foodInGrill.Count; i++)
        {
            _slotFoods[i].OnSetFood(foodInGrill[i]);
        }

        _listFoodForPlate = new List<List<Sprite>>();

        for (int i = 0; i < totalPlate - 1; i++)
        {
            _listFoodForPlate.Add(new List<Sprite>());
            int foodIndex = Random.Range(0, listFood.Count);
            _listFoodForPlate[i].Add(listFood[foodIndex]);
            listFood.RemoveAt(foodIndex);
        }

        while (listFood.Count > 0)
        {
            List<int> availablePlates = new List<int>();
            for (int i = 0; i < _listFoodForPlate.Count; i++)
            {
                if (_listFoodForPlate[i].Count < 3)
                {
                    availablePlates.Add(i);
                }
            }

            if (availablePlates.Count == 0)
            {
                break;
            }

            int randomAvailableIndex = Random.Range(0, availablePlates.Count);
            int targetPlateIndex = availablePlates[randomAvailableIndex];

            int randomFoodIndex = Random.Range(0, listFood.Count);
            _listFoodForPlate[targetPlateIndex].Add(listFood[randomFoodIndex]);
            listFood.RemoveAt(randomFoodIndex);
        }

        if (_listFoodForPlate.Count > 0)
        {
            _plate.OnSetListFood(_listFoodForPlate[0]);
        }
        Debug.Log(_listFoodForPlate.Count);
    }    
}
