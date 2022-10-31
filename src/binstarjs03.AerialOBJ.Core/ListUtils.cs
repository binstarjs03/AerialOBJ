using System;
using System.Collections.Generic;

namespace binstarjs03.AerialOBJ.Core;
public static class ListUtils
{
    private static readonly Random s_random = new();

    public static void FisherYatesShuffe<T>(this IList<T> list)
    {
        int index = list.Count;
        while (index > 0)
        {
            index--;
            int randomIndex;
            lock(s_random)
            {
                randomIndex = s_random.Next(index + 1);
            }
            T temp = list[randomIndex];
            list[randomIndex] = list[index];
            list[index] = temp;
        }
    }
}
