using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Locations
{
    public Location[] LocationsList =
    {
        new Location("Exeter", -3.52751, 50.7236),
        new Location("New York", -74.0059728, 40.7127753),
        new Location("Seattle", -122.3320708, 47.6062095),
        new Location("Mexico City", -99.133208, 19.4326077),
        new Location("Bogota", -74.072092, 4.7109886),
        new Location("Punta Arenas", -70.9170683, -53.1638329),
        new Location("Santiago", -70.6692655, -33.4488897),
        new Location("Cape Town", 18.4240553, -33.9248685),
        new Location("Nairobi", 36.8219462, -1.2920659),
        new Location("Accra", -0.186964399999965, 5.6037168),
        new Location("Casablanca", -7.58984340000006, 33.5731104),
        new Location("Cairo", 31.2357116000001, 30.0444196),
        new Location("Reykjavík", -21.9426354, 64.146582),
        new Location("Moscow", 37.6172999, 55.755826),
        new Location("Mumbai", 72.8776559, 19.0759837),
        new Location("Sydney", 151.2092955, -33.8688197),
        new Location("Perth", 115.8604572, -31.9505269),
        // Spacing so title is left-aligned and in view
        new Location("Wellington             ", 174.776236, -41.2864603),
        new Location("Tokyo", 139.6503106, 35.6761919),
        new Location("Ulaanbaatar", 106.9057439, 47.8863988),
        new Location("Jakarta", 106.845599, -6.2087634),
        new Location("Antananarivo", 47.5079055, -18.8791902),
        new Location("Brasília", -47.8821658, -15.7942287),
        new Location("Tashkent", 69.2400734, 41.2994958)
    };
}

public class Location
{
    public string city;
    public double lat, lon;
    public int x, y;
    public GameObject obj;

    public Location(string city, double lon, double lat)
    {
        this.city = city;
        this.lat = lat;
        this.lon = lon;
        this.x = LonToX(lon);
        this.y = LatToY(lat);
    }

    private int LatToY(double lat)
    {
        // Lat is -90 -> 90
        // Map is 0 -> 180
        int y = (int)Mathf.Round((float)((float)lat + 90.0));
        return y;
    }
    private int LonToX(double lon)
    {
        // lon is -180 -> 180
        // Map is 0 -> 360
        int x = (int)Mathf.Round((float)((float)lon + 180.0));
        return x;
    }
}
