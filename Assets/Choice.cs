using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Choice : MonoBehaviour
{
    public delegate void ChoiceCallback();

    public GameObject titleGameObject;
    public GameObject textGameObject;
    public GameObject optionsObject;
    
    public ChoiceCallback cancel;
        
    public void OnClose()
    {
        CloseModal();
        cancel?.Invoke();
    }

    private void CloseModal()
    {
        Destroy(gameObject);
        Map.GetSingleton().UnpauseMap();
    }
    
    public static void OpenChoice(string title, string text, ChoiceOption[] choices, ChoiceCallback cancel)
    {
        GameObject modalPrefab = UnityEngine.Resources.Load("ChoicePrefab") as GameObject;
        GameObject optionPrefab = UnityEngine.Resources.Load("OptionPrefab") as GameObject;
                
        var modal = Instantiate(modalPrefab);
        var choice = modal.GetComponent<Choice>();
        choice.cancel = cancel;
        choice.titleGameObject.GetComponent<Text>().text = title;
        choice.textGameObject.GetComponent<Text>().text = text.Replace("\\n", "\n");
        
        foreach (var option in choices)
        {
            var optionObject = Instantiate(optionPrefab);
            optionObject.transform.Find("LeftText").GetComponent<Text>().text = option.leftText;
            optionObject.transform.Find("RightText").GetComponent<Text>().text = option.rightText;
            optionObject.GetComponent<Button>().interactable = option.active;
            optionObject.GetComponent<Button>().onClick.AddListener(delegate
            {
                choice.CloseModal();
                option.callback();
            });
            optionObject.transform.parent = choice.optionsObject.transform;
        }
        
        modal.transform.parent = Map.GetSingleton().canvas.transform;
        modal.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        modal.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        
        Map.GetSingleton().PauseMap();
    }
}

public class ChoiceOption
{
    public string leftText;
    public string rightText;
    public Choice.ChoiceCallback callback;
    public bool active;

    public ChoiceOption(string leftText, string rightText, Choice.ChoiceCallback callback, bool active = true)
    {
        this.leftText = leftText;
        this.rightText = rightText;
        this.callback = callback;
        this.active = active;
    }
}