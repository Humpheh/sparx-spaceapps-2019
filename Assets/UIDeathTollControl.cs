using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDeathTollControl : MonoBehaviour
{
    public void GlobalDeathToll(int newDeathToll)
    {
        // Resources.Dead.value = newDate;
        GetComponent<Text>().text = "Infected: "+Resources.Infected.value+", Dead: "+newDeathToll.ToString()+" / 1,000,000";
    }
}
