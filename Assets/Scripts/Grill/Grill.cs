using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Grill : MonoBehaviour
{
    [SerializeField] private List<SlotFood> _slotFoods;
    [SerializeField] private Plate _plate;
    private List<List<Sprite>> _listFoodForPlate;

    public void OnInitGrill(int totalPlate, List<Sprite> listFood)
    {
        int foodCount = Random.Range(1, _slotFoods.Count + 1);
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
    }

    public SlotFood GetSlotNull()
    {
        for (int i = 0; i < _slotFoods.Count; i++)
        {
            if (_slotFoods[i].IsEmpty())
            {
                return _slotFoods[i];
            }
        }
        return null;
    }

    public void OnCheckDoneGrill()
    {
        if (IsDoneGrill())
        {
            Sequence clearSeq = DOTween.Sequence();

            foreach (var slot in _slotFoods)
            {
                clearSeq.Join(slot.ImageFood.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack));
            }

            clearSeq.OnComplete(() => {
                OnFillGrill();
            });
        }
    }

    public void OnCheckEmptyGrill()
    {
        for (int i = 0; i < _slotFoods.Count; i++)
        {
            if (!_slotFoods[i].IsEmpty())
            {
                return;
            }
        }

        OnFillGrill();
    }

    public void OnFillGrill()
    {
        OnClearGrill();

        foreach (var slot in _slotFoods)
        {
            slot.ImageFood.transform.localScale = Vector3.one;
        }

        if (_listFoodForPlate != null && _listFoodForPlate.Count > 0)
        {
            List<Sprite> nextFood = _listFoodForPlate[0];
            _listFoodForPlate.RemoveAt(0);

            Sequence moveSeq = DOTween.Sequence();

            for (int i = 0; i < nextFood.Count; i++)
            {
                _slotFoods[i].OnSetFood(nextFood[i]);
                Transform foodTrans = _slotFoods[i].ImageFood.transform;

                foodTrans.position = _plate.GetFoodPosition(i);

                moveSeq.Join(foodTrans.DOLocalMove(Vector3.zero, 0.35f).SetEase(Ease.OutQuad));
            }

            moveSeq.OnComplete(() => {
                if (_listFoodForPlate.Count > 0)
                {
                    _plate.OnSetListFood(_listFoodForPlate[0]);
                    _plate.AnimateShowNextFood(0.3f);
                }
                else
                {
                    _plate.OnClearPlate();
                    _plate.gameObject.SetActive(false);
                }

                OnCheckDoneGrill();
            });
        }
    }    

    public void OnClearGrill()
    {
        foreach (var slot in _slotFoods)
        {
            slot.OnClearSlot();
        }
    }

    public bool IsDoneGrill()
    {
        if (GetSlotNull() != null)
        {
            return false;
        }

        Sprite sameSprite = _slotFoods[0].ImageFood.sprite;
        for (int i = 1; i < _slotFoods.Count; i++)
        {
            if (_slotFoods[i].ImageFood.sprite != sameSprite)
            {
                return false;
            }
        }
        return true;
    }
}