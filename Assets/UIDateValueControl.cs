using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDateValueControl : MonoBehaviour
{
    public void GlobalTimeStep(DateTime newDate)
    {
        Resources.Time.value = newDate;
        GetComponent<Text>().text = newDate.ToString("dd MMMM yyyy");
    }
}
