using UnityEngine;

public class Utils
{
    public static T RandomInArr<T>(T[] arr)
    {
        int random = Mathf.FloorToInt(Random.value * arr.Length);
        return arr[random];
    }
}
