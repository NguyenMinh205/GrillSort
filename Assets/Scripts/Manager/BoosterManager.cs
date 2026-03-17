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
        seq.Append(_imgFoodBag.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack));

        float startTime = 0.4f;
        float staggerTime = 0.15f;

        foreach (var slot in slots)
        {
            AnimateFoodToBag(seq, sprite, targetIndex, startTime, slot.ImageFood.transform.position, () => {
                slot.OnClearSlot();
            });

            startTime += staggerTime;
            targetIndex++;
        }

        foreach (var grill in plates)
        {
            int indexToRemove = grill.ListFoodForPlate[0].IndexOf(sprite);
            if (indexToRemove != -1)
            {
                Vector3 startPos = grill.Plate.GetFoodPosition(indexToRemove);

                AnimateFoodToBag(seq, sprite, targetIndex, startTime, startPos, () => {
                    grill.ListFoodForPlate[0][indexToRemove] = null;
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
                });

                startTime += staggerTime;
                targetIndex++;
            }
        }

        seq.OnComplete(() => {
            _imgFoodBag.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() => {
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
        });
    }

    private void AnimateFoodToBag(Sequence seq, Sprite sprite, int targetIndex, float startTime, Vector3 startPos, Action clearDataAction)
    {
        Image targetImg = _imgMatchingFoods[targetIndex];
        targetImg.sprite = sprite;
        targetImg.gameObject.SetActive(true);

        targetImg.transform.position = startPos;
        targetImg.transform.localScale = Vector3.one;

        clearDataAction?.Invoke();

        seq.Insert(startTime, targetImg.transform.DOMove(_imgFoodBag.transform.position, 0.4f).SetEase(Ease.InBack));

        seq.Insert(startTime, targetImg.transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack));

        seq.InsertCallback(startTime + 0.4f, () => {
            targetImg.gameObject.SetActive(false);

            _imgFoodBag.transform.DOKill(true);
            _imgFoodBag.transform.localScale = Vector3.one;

            _imgFoodBag.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0f), 0.2f, 5, 1f);
        });
    }
    #endregion

    #region Swap Food Booster
    public void OnSwapFoods()
    {
        List<Grill> activeGrills = GameplayManager.Instance.ListGrill.Where(g => g.gameObject.activeSelf).ToList();

        List<SlotFood> allFilledSlots = new List<SlotFood>();
        List<Sprite> tempGrillFoods = new List<Sprite>();

        List<Action<Sprite>> plateSetters = new List<Action<Sprite>>();
        List<Sprite> tempPlateFoods = new List<Sprite>();

        foreach (var grill in activeGrills)
        {
            foreach (var slot in grill.GetFilledSlots())
            {
                allFilledSlots.Add(slot);
                tempGrillFoods.Add(slot.ImageFood.sprite);
            }

            if (grill.ListFoodForPlate != null)
            {
                for (int i = 0; i < grill.ListFoodForPlate.Count; i++)
                {
                    for (int j = 0; j < grill.ListFoodForPlate[i].Count; j++)
                    {
                        Sprite s = grill.ListFoodForPlate[i][j];
                        if (s != null)
                        {
                            tempPlateFoods.Add(s);

                            int listIndex = i;
                            int slotIndex = j;
                            Grill g = grill;
                            plateSetters.Add((newSprite) => {
                                g.ListFoodForPlate[listIndex][slotIndex] = newSprite;
                            });
                        }
                    }
                }
            }
        }

        tempGrillFoods = tempGrillFoods.OrderBy(x => UnityEngine.Random.value).ToList();
        tempPlateFoods = tempPlateFoods.OrderBy(x => UnityEngine.Random.value).ToList();

        Sequence seq = DOTween.Sequence();
        float duration = 0.3f;

        foreach (var slot in allFilledSlots)
        {
            seq.Join(slot.ImageFood.transform.DOScale(Vector3.zero, duration).SetEase(Ease.InBack));
        }

        foreach (var grill in activeGrills)
        {
            if (grill.ListFoodForPlate != null && grill.ListFoodForPlate.Count > 0)
            {
                seq.Join(grill.Plate.HideCurrentFoodsAnimation(duration));
            }
        }

        seq.OnComplete(() => {
            for (int i = 0; i < allFilledSlots.Count; i++)
            {
                allFilledSlots[i].OnSetFood(tempGrillFoods[i]);
                allFilledSlots[i].ImageFood.transform.localScale = Vector3.zero;
            }

            for (int i = 0; i < plateSetters.Count; i++)
            {
                plateSetters[i].Invoke(tempPlateFoods[i]);
            }

            Sequence seqUp = DOTween.Sequence();

            foreach (var slot in allFilledSlots)
            {
                seqUp.Join(slot.ImageFood.transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack));
            }

            foreach (var grill in activeGrills)
            {
                if (grill.ListFoodForPlate != null && grill.ListFoodForPlate.Count > 0)
                {
                    grill.Plate.OnSetListFood(grill.ListFoodForPlate[0]);
                    seqUp.Join(grill.Plate.ShowCurrentFoodsAnimation(duration));
                }
            }

            seqUp.OnComplete(() => {
                foreach (var grill in activeGrills)
                {
                    grill.OnCheckDoneGrill();
                }
            });
        });
    }
    #endregion

    #region Unlock New Grill Booster
    public void OnUnlockNewGrill()
    {
        Grill unActiveGrills = GameplayManager.Instance.ListGrill.FirstOrDefault(g => !g.gameObject.activeSelf);
        if (unActiveGrills != null)
        {
            OnAddGrill(unActiveGrills);
            return;
        }
        Debug.Log("Không còn bếp nào để mở khóa!");
    }

    public void OnAddGrill(Grill grillToAdd)
    {
        grillToAdd.gameObject.SetActive(true);
        grillToAdd.Plate.gameObject.SetActive(false);
        grillToAdd.gameObject.transform.localScale = Vector3.zero;
        grillToAdd.gameObject.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }
    #endregion

    #region Add Extend Booster 
    public void OnExtendTimer()
    {
        GameplayManager.Instance.UIManager.AddExtraTime(60f);
    }
    #endregion
}