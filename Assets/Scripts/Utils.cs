using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
