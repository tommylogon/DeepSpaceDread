using System;
using UnityEngine;

public static class JsonHelper
{
    public static (T[], object[]) FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        object[] settings = new object[] { wrapper.Continuous, wrapper.FillRate, wrapper.Tabs };
        return (wrapper.Slots, settings);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint = false, params object[] settings)
    {
        Wrapper<T> wrapper = new Wrapper<T>
        {
            Continuous = (bool)settings[0],
            FillRate = (float)settings[1],
            Tabs = (byte)settings[2],
            Slots = array
        };
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public bool Continuous;
        public float FillRate;
        public byte Tabs;
        public T[] Slots;
    }
}

internal class SerializableAttribute : Attribute
{
}