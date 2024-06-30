using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using Ping = UnityEngine.Ping;

public class ChonkerUtility
{
    
    public static void Shuffle<T>(IList<T> ts) {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    public static T[] Shuffle<T>(T[] array) {
        var count = array.Length;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = array[i];
            array[i] = array[r];
            array[r] = tmp;
        }

        return array;
    }
    
    public static T pickRandomElement<T>(List<T> array) {
        return array[Random.Range(0, array.Count)];
    }
    
    public static T pickRandomElement<T>(List<T> array, int minIndex, int maxIndex) {
        return array[Random.Range(minIndex, maxIndex)];
    }

    public static T pickRandomElement<T>(T[] array) {
        return array[Random.Range(0, array.Length)];
    }
    
    public static T pickRandomElement<T>(T[] array, int minIndex, int maxIndex) {
        return array[Random.Range(minIndex, maxIndex)];
    }
    
}
