using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class MozDataParse : MonoBehaviour
{
    Dictionary<(int, int), bool> pointLookup = new Dictionary<(int, int), bool>();
    public DataPoint[] points;
    public void Awake()
    {
        TextAsset bindata = UnityEngine.Resources.Load("mozData") as TextAsset;
        var data = JsonUtility.FromJson<DataPoints>(bindata.text);
        points = data.data;

        foreach (var point in points)
        {
            var yx = ((int)Mathf.Round(point.latitude + 90), (int)Mathf.Round(point.longitude + 180));
            if (!pointLookup.ContainsKey(yx))
            {
                pointLookup.Add(yx, true);
            }
        }
    }

    public bool HasEvent(int x, int y)
    {
        return pointLookup.ContainsKey((y, x));
    }
}

//{"countryCode": "USA", "countryName": "United States", "elevation": "78.1", "mosquitohabitatmapperBreedingGroundEliminated": "true", "mosquitohabitatmapperDataSource": "GLOBE Observer App", "mosquitohabitatmapperMeasuredAt": "2017-06-07T21:13:00", "mosquitohabitatmapperMeasurementElevation": "78.1", "mosquitohabitatmapperMosquitoHabitatMapperId": "50", "mosquitohabitatmapperUserid": "6972382", "mosquitohabitatmapperWaterSource": "dish or pot", "mosquitohabitatmapperWaterSourceType": "container: artificial", "organizationId": "organizationId", "organizationName": "Institute for Global Environmental Strategies (IGES) GLOBE v-School", "protocol": "mosquito_habitat_mapper", "siteId": "siteId", "siteName": "18SUJ131033", "latitude": 38.858688, "longitude": -77.154017}

[Serializable]
class DataPoints
{
    public DataPoint[] data;
}

[Serializable]
public class DataPoint
{
    public float latitude;
    public float longitude;
}
