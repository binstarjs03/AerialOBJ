using System;
using System.Collections.Generic;

namespace binstarjs03.AerialOBJ.Core;
public static class ListUtils
{
    public static void FisherYatesShuffe<T>(this IList<T> list)
    {
        int index = list.Count;
        Random random = new();
        while (index > 0)
        {
            index--;
            int randomIndex;
            randomIndex = random.Next(index + 1);
            (list[index], list[randomIndex]) = (list[randomIndex], list[index]);
        }
    }
}
