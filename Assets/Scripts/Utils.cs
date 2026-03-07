using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Utils
{
    public static List<T> GetListFromChildren<T>(Transform parent)
    {
        List<T> list = new List<T>();
        foreach (Transform child in parent)
        {
            T component = child.GetComponent<T>();
            if (component != null)
            {
                list.Add(component);
            }
        }
        return list;
    }

    public static List<T> TakeAndRemoveRandom<T>(List<T> source, int n)
    {
        List<T> result = new List<T>();
        n = Mathf.Min(n, source.Count);

        for (int i = 0; i < n; i++)
        {
            int randomIndex = Random.Range(0, source.Count);
            result.Add(source[randomIndex]);
            source.RemoveAt(randomIndex);
        }

        return result;
    }

    public static T GetRayCast<T>(Vector2 position) where T : MonoBehaviour
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = position;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        if (raycastResults.Count > 0)
        {
            foreach (RaycastResult result in raycastResults)
            {
                T component = result.gameObject.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
            }
        }

        return null;
    }
}
