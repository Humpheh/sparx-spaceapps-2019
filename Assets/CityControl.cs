using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class CityControl : MonoBehaviour
{
    private static float ZOOM_CUTOFF = 10;

    public static CityControl CurrentSelection;

    public Location location;
    public GameObject text;
    public Vector3 worldLocation;
    public float effectiveness;

    private bool fullText = false;

    // Update is called once per frame
    void Update()
    {
        // Update if the text should be shown
        if (Camera.main.orthographicSize < ZOOM_CUTOFF && !fullText)
        {
            fullText = true;
            UpdateText();
        }
        else if (Camera.main.orthographicSize >= ZOOM_CUTOFF && fullText)
        {
            fullText = false;
            UpdateText();
        }

        if (Map.GetSingleton().IsPaused()) return;

        SetPosition();

        effectiveness = location.effectiveness;
    }

    public void SetPosition()
    {
        var screenPosition = Utils.CanvasPosition(worldLocation);
        var uiElement = text.GetComponent<RectTransform>();
        uiElement.anchoredPosition = screenPosition;
        GetComponent<RectTransform>().anchoredPosition = screenPosition;
    }

    public void HandleClick()
    {
//        if (Map.GetSingleton().dispatchType != PlaneBehaviour.PlaneType.NONE)
//        {
//            return;
//        }
        //Debug.LogFormat("clicked city {0}", location.city);

        var lastSelection = CurrentSelection;
        if (CurrentSelection != null) CurrentSelection.Deselect();
        if (lastSelection != this) Select();

        var mapSingleton = Map.GetSingleton();
        if (location.isStatic == false)
        {
            // Shortcut to send
            if (Input.GetKey(KeyCode.Space) && Resources.Bank.Balance >= 10000 && HasDoctors(1))
            {
                mapSingleton.dispatchType = PlaneBehaviour.PlaneType.DOCTOR;
                return;
            }
            
            Choice.OpenChoice(
                location.city,
                "Select something to do:",
                new[]
                {
                    new ChoiceOption("Deploy Doctor", "$10,000", delegate {
                        mapSingleton.dispatchType = PlaneBehaviour.PlaneType.DOCTOR;
                    }, Resources.Bank.Balance >= 10000 && HasDoctors(1)),
                    new ChoiceOption("Buy Mosquito Netting", "$1,000", delegate {
                        AddDoctor(0, 0.2f, 0);
                    }, Resources.Bank.Balance >= 1000),
                    new ChoiceOption("Buy Insecticide", "$2,250", delegate {
                        AddDoctor(0, 0.3f, 0);
                    }, Resources.Bank.Balance >= 2250),
                },
                delegate { Deselect(); }
            );
        }
        else
        {
            // Shortcut to send
            if (Input.GetKey(KeyCode.Space) && Resources.Bank.Balance >= 10000 && HasDoctors(1))
            {
                mapSingleton.dispatchType = PlaneBehaviour.PlaneType.DOCTOR;
                return;
            }
            
            Choice.OpenChoice(
                location.city,
                location.doctors + " doctors available",
                new[]
                {
                    new ChoiceOption("Deploy Doctor", "$10,000", delegate {
                        mapSingleton.dispatchType = PlaneBehaviour.PlaneType.DOCTOR;
                        Debug.Log("Deploying from here");
                    }, Resources.Bank.Balance >= 10000 && HasDoctors(1)),
                    new ChoiceOption("Fund Doctor", "$100,000", delegate {
                        AddDoctor(1, 1, 0, true);
                    }, Resources.Bank.Balance >= 100000),
                    new ChoiceOption("Dispatch Mosquito Netting", "$1,000", delegate {
                        mapSingleton.dispatchType = PlaneBehaviour.PlaneType.AIRDROP_NETTING; 
                    }, Resources.Bank.Balance >= 1000),
                    new ChoiceOption("Dispatch Insecticide", "$2,250", delegate {
                        mapSingleton.dispatchType = PlaneBehaviour.PlaneType.AIRDROP_INSECTICIDE; 
                    }, Resources.Bank.Balance >= 2250),
                    new ChoiceOption("Research Upgrade (L"+Resources.Level.value+")",
                        "$1,000,000", delegate { UnlockCity(); },
                        Resources.Bank.Balance >= 1000000)
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
        if (fullText)
        {
            text.GetComponent<Text>().text = location.city + " x" + location.doctors;
        }
        else
        {
            text.GetComponent<Text>().text = location.doctors.ToString();
        }
    }

    public void Deselect()
    {
        CurrentSelection = null;
        if (location.isStatic == false) GetComponent<Image>().color = new Color(0, 0.42f, 1f);
        else if (location.isLocked == true && location.doctors == 0) GetComponent<Image>().color = Color.grey;
        else GetComponent<Image>().color = new Color(1f, 0.5f, 0.25f);
    }

    public void TryRemoveCity()
    {
        if (location.doctors == 0 && !location.isStatic && location.effectiveness <= 0)
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
            location.effectiveness--;
            UpdateText();
            TryRemoveCity();
        }
        Deselect();
    }

    public void AddDoctor(int doctors, float effectiveness, int timeDuration, bool cost = false)
    {
        if (cost)
        {
            Resources.Bank.Spend(100000);
        }

        location.doctors += doctors;
        location.effectiveness += effectiveness;
        UpdateText();
        Deselect();
    }

    public void UnlockCity()
    {
        location.isLocked = false;
        GetComponent<Image>().color = Color.red;
        Resources.Level.value++;
        Resources.Bank.Spend(900000);
        AddDoctor(1, 1, 0, false);
        Modal.OpenModal(
            "Congratulations!",
            "You're now Level " + Resources.Level.value + "!\\nDoctors are now more effective in the fight against Malaria.",
            delegate { });
    }
}
