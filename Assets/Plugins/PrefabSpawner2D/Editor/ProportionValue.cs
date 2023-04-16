using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ProportionValue<T>
{
    public double Proportion { get; set; }
    public T Value { get; set; }
}

public static class ProportionValue
{
    public static ProportionValue<T> Create<T>(double proportion, T value)
    {
        return new ProportionValue<T> { Proportion = proportion, Value = value };
    }

    public static T ChooseRandomly<T>(this IEnumerable<ProportionValue<T>> collection)
    {
        double random = Random.Range(0.0f, 1.0f);

        foreach (var item in collection)
        {
            if (random < item.Proportion)
            {
                return item.Value;
            }

            random -= item.Proportion;
        }

        var list = collection.ToList();
        return list[Random.Range(0, list.Count)].Value;
    }
}
