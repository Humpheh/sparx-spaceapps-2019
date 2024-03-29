using UnityEngine;

public class Utils
{
    public static T RandomInArr<T>(T[] arr)
    {
        int random = Mathf.FloorToInt(Random.value * arr.Length);
        return arr[random];
    }

    public static Vector2 CanvasPosition(Vector3 worldPosition)
    {
        RectTransform canvasRect = Map.GetSingleton().canvas.GetComponent<RectTransform>();
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(worldPosition);
        return new Vector2(
            ((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));

    }

    public static Vector3 LatLongToMapCoords(int lat, int longitude)
    {
        return new Vector3(Map.GetSingleton().GridToMapX(longitude + 180), Map.GetSingleton().GridToMapY(lat + 90), -1);
    }

    public static Vector3 ApiEvtToMapCoords(DataPoint location)
    {
        return new Vector3(
            Map.GetSingleton().GridToMapX((int)location.longitude),
            Map.GetSingleton().GridToMapY(180 - (int)location.latitude),
            -1
        );
    }
}
