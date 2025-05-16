using System.Collections.Generic;
using UnityEngine;

public static class JsonUtilityWrapper
{
    public static Dictionary<string, T> FromJson<T>(string json)
    {
        string fixedJson = "{\"items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(fixedJson);
        Dictionary<string, T> result = new Dictionary<string, T>();
        foreach (var entry in wrapper.items)
        {
            result[entry.Key] = entry.Value;
        }
        return result;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public List<Entry<T>> items;
    }

    [System.Serializable]
    private class Entry<T>
    {
        public string Key;
        public T Value;
    }
}
