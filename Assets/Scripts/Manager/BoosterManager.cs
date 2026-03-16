using DG.Tweening;
using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BoosterManager : MonoBehaviour
{
    [Header("Matching Trio Booster")]
    [SerializeField] private List<Image> _imgMatchingFoods;
    [SerializeField] private Image _imgFoodBag;

    #region Matching Trio Booster
    public void OnCollectMatchingTrio()
    {
        List<Grill> activeGrills = GameplayManager.Instance.ListGrill.Where(g => g.gameObject.activeSelf && !g.IsDoneGrill()).ToList();

        Dictionary<Sprite, List<SlotFood>> foodsInGrill = new Dictionary<Sprite, List<SlotFood>>();
        Dictionary<Sprite, List<Grill>> foodsInPlate = new Dictionary<Sprite, List<Grill>>();

        foreach (var grill in activeGrills)
        {
            foreach (var slot in grill.GetFilledSlots())
            {
                Sprite s = slot.ImageFood.sprite;
                if (!foodsInGrill.ContainsKey(s)) foodsInGrill[s] = new List<SlotFood>();
                foodsInGrill[s].Add(slot);
            }

            if (grill.ListFoodForPlate != null && grill.ListFoodForPlate.Count > 0)
            {
                foreach (var s in grill.ListFoodForPlate[0])
                {
                    if (s == null) continue;

                    if (!foodsInPlate.ContainsKey(s)) foodsInPlate[s] = new List<Grill>();
                    foodsInPlate[s].Add(grill);
                }
            }
        }

        Sprite targetSprite = null;

        HashSet<Sprite> allSprites = new HashSet<Sprite>(foodsInGrill.Keys);
        allSprites.UnionWith(foodsInPlate.Keys);

        foreach (Sprite s in allSprites)
        {
            int grillCount = foodsInGrill.ContainsKey(s) ? foodsInGrill[s].Count : 0;
            int plateCount = foodsInPlate.ContainsKey(s) ? foodsInPlate[s].Count : 0;

            if (grillCount + plateCount >= 3)
            {
                targetSprite = s;
                break;
            }
        }

        if (targetSprite != null)
        {
            List<SlotFood> slotsToClear = new List<SlotFood>();
            List<Grill> platesToClear = new List<Grill>();

            int collected = 0;

            if (foodsInGrill.ContainsKey(targetSprite))
            {
                foreach (var slot in foodsInGrill[targetSprite])
                {
                    if (collected < 3)
                    {
                        slotsToClear.Add(slot);
                        collected++;
                    }
                }
            }

            if (collected < 3 && foodsInPlate.ContainsKey(targetSprite))
            {
                foreach (var grill in foodsInPlate[targetSprite])
                {
                    if (collected < 3)
                    {
                        platesToClear.Add(grill);
                        collected++;
                    }
                }
            }

            PlayCollectMatchingFoodAnimation(targetSprite, slotsToClear, platesToClear);
        }
        else
        {
            Debug.Log("Không tìm thấy bộ 3 món ăn nào để thu thập!");
        }
    }

    public void PlayCollectMatchingFoodAnimation(Sprite sprite, List<SlotFood> slots, List<Grill> plates)
    {
        Sequence seq = DOTween.Sequence();
        int targetIndex = 0;
        _imgFoodBag.gameObject.SetActive(true);
        _imgFoodBag.transform.localScale = Vector3.zero;
        _imgFoodBag.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        foreach (var slot in slots)
        {
            Image targetImg = _imgMatchingFoods[targetIndex];
            targetImg.sprite = sprite;
            targetImg.gameObject.SetActive(true);

            targetImg.transform.position = slot.ImageFood.transform.position;
            targetImg.transform.localScale = Vector3.one;

            slot.OnClearSlot();

            seq.Join(targetImg.transform.DOLocalMove(_imgFoodBag.transform.position, 0.5f).SetEase(Ease.OutBack));
            targetIndex++;
        }

        foreach (var grill in plates)
        {
            Image targetImg = _imgMatchingFoods[targetIndex];
            targetImg.sprite = sprite;
            targetImg.gameObject.SetActive(true);

            int indexToRemove = grill.ListFoodForPlate[0].IndexOf(sprite);

            if (indexToRemove != -1)
            {
                targetImg.transform.position = grill.Plate.GetFoodPosition(indexToRemove);
                targetImg.transform.localScale = Vector3.one;

                grill.ListFoodForPlate[0][indexToRemove] = null;
            }

            bool isPlateEmpty = grill.ListFoodForPlate[0].All(s => s == null);

            if (isPlateEmpty)
            {
                grill.ListFoodForPlate.RemoveAt(0);
                if (grill.ListFoodForPlate.Count > 0)
                {
                    grill.Plate.OnSetListFood(grill.ListFoodForPlate[0]);
                    grill.Plate.AnimateShowNextFood(0.3f);
                }
                else
                {
                    grill.Plate.OnClearPlate();
                    grill.Plate.gameObject.SetActive(false);
                }
            }
            else
            {
                grill.Plate.OnSetListFood(grill.ListFoodForPlate[0]);
            }

            seq.Join(targetImg.transform.DOLocalMove(_imgFoodBag.transform.position, 0.5f).SetEase(Ease.OutBack));
            targetIndex++;
        }

        seq.OnComplete(() => {
            foreach (var img in _imgMatchingFoods)
            {
                img.gameObject.SetActive(false);
            }

            _imgFoodBag.gameObject.SetActive(false);

            HashSet<Grill> affectedGrills = new HashSet<Grill>();
            foreach (var slot in slots) affectedGrills.Add(slot.GrillCtrl);
            foreach (var grill in plates) affectedGrills.Add(grill);

            foreach (var grill in affectedGrills)
            {
                grill.OnCheckEmptyGrill();
            }

            ObserverManager<GameEvent>.PostEvent(GameEvent.OnDoneGrill);
        });
    }
    #endregion

    public void OnSwapItems()
    {

    }

    public void OnUnlockNewGrill()
    {

    }

    public void OnExtendTimer()
    {

    }
}