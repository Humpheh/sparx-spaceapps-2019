﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityControl : MonoBehaviour
{
    public static CityControl CurrentSelection;

    public Location location;
    public GameObject text;
    public Vector3 worldLocation;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Map.GetSingleton().IsPaused()) return;

        //        if (Input.GetMouseButtonDown(0)){ 
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            RaycastHit hit;
//            if (Physics.Raycast(ray, out hit) && hit.transform == transform){
//                Debug.LogFormat("clicked city {0}", location.city);
//                
//                var lastSelection = CurrentSelection;
//                if (CurrentSelection != null) CurrentSelection.Deselect();
//                if (lastSelection != this) Select();
//            }
//        }
        SetPosition();
    }

    public void SetPosition()
    {
        RectTransform canvasRect = Map.GetSingleton().canvas.GetComponent<RectTransform>();
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(worldLocation);
        Vector2 screenPosition = new Vector2(
            ((viewportPosition.x*canvasRect.sizeDelta.x)-(canvasRect.sizeDelta.x*0.5f)),
            ((viewportPosition.y*canvasRect.sizeDelta.y)-(canvasRect.sizeDelta.y*0.5f)));
 
        //now you can set the position of the ui element
        var uiElement = text.GetComponent<RectTransform>();
        uiElement.anchoredPosition = screenPosition;
        GetComponent<RectTransform>().anchoredPosition = screenPosition;
    }
    
    public void HandleClick()
    {
        //Debug.LogFormat("clicked city {0}", location.city);

        var lastSelection = CurrentSelection;
        if (CurrentSelection != null) CurrentSelection.Deselect();
        if (lastSelection != this) Select();

        if (location.isLocked == true)
        {
            Choice.OpenChoice(
                location.city,
                "Select something to do:",
                new[]
                {
                    new ChoiceOption("Buy Doctor", "$100,000", delegate { UnlockCity(); }, Resources.Bank.Balance >= 100000)
                },
                delegate { Deselect(); }
            );
        }
        else if (location.isStatic == false)
        {
            Choice.OpenChoice(
                location.city,
                "Select something to do:",
                new[]
                {
                    //new ChoiceOption("Remove Doctor", "$0", delegate { RemoveDoctor(); })
                    new ChoiceOption("Deploy Doctor", "$10,000", delegate { })
                },
                delegate { Deselect(); }
            );
        }
        else
        {
            Choice.OpenChoice(
                location.city,
                "Select something to do:",
                new[]
                {
                    new ChoiceOption("Deploy Doctor", "$10,000", delegate { }, Resources.Bank.Balance >= 10000 && CurrentSelection.HasDoctors(1)),
                    new ChoiceOption("Buy Doctor", "$100,000", delegate { AddDoctor(); }, Resources.Bank.Balance >= 100000)
                },
                delegate { Deselect(); }
            );
        }
    }

    void Select()
    {
        if (location.isLocked == true) return;
        CurrentSelection = this;
        GetComponent<Image>().color = new Color(1.0f, 0.55f, 0f);
    }

    public void UpdateText()
    {
        text.GetComponent<Text>().text = location.city + " x" + location.doctors;
    }

    public void Deselect()
    {
        CurrentSelection = null;
        if (location.isStatic == false) GetComponent<Image>().color = Color.blue;
        else if (location.isLocked == true) GetComponent<Image>().color = Color.grey;
        else GetComponent<Image>().color = Color.red;

        Map.GetSingleton().currentlyCasting = false;
    }

    public void TryRemoveCity()
    {
        if (location.doctors == 0 && !location.isStatic)
        {
            Destroy(text);
            Destroy(gameObject);
        }
    }
    
    public bool CanRemoveDoctor()
    {
        return location.doctors > 0;
    }

    public bool HasDoctors(int number)
    {
        return location.doctors >= number;
    }
    
    public void RemoveDoctor()
    {
        if (location.doctors > 0)
        {
            location.doctors--;
            UpdateText();
            TryRemoveCity();
        }
        Deselect();
    }

    public void AddDoctor()
    {
        location.doctors++;
        Resources.Bank.Spend(100000);
        UpdateText();
        Deselect();
    }

    public void UnlockCity()
    {
        location.isLocked = false;
        GetComponent<Image>().color = Color.red;
        Resources.Level.value++;
        AddDoctor();
    }
}
