using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public DateTime currentDate;
    public float ticker = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        currentDate = DateTime.Parse("01/01/2000");
    }

    // Update is called once per frame
    void Update()
    {
        ticker += Time.deltaTime;
        if (ticker > 1)
        {
            currentDate = currentDate.AddHours(1);
            ticker = 0;
            Debug.LogFormat("Update time {0}", currentDate.ToString());
            gameObject.BroadcastMessage("GlobalTimeStep", currentDate);
        }
    }
}
