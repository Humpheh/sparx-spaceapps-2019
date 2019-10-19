using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Modal : MonoBehaviour
{
    public delegate void ModalCallback(string option);
    
    public GameObject titleGameObject;
    public GameObject textGameObject;
    public ModalCallback callback;
    
    public string title;
    public string text;
    
    // Start is called before the first frame update
    void Start()
    {
        titleGameObject.GetComponent<Text>().text = title;
        textGameObject.GetComponent<Text>().text = text;
    }

    public void OnClose()
    {
        Map.GetSingleton().UnpauseMap();
        Destroy(gameObject);
        callback?.Invoke("Continue");
    }

    public static void OpenModal(string title, string text, ModalCallback callback)
    {
        GameObject modalPrefab = UnityEngine.Resources.Load("ModalPrefab") as GameObject;
        
        var modal = Instantiate(modalPrefab);
        modal.GetComponent<Modal>().title = title;
        modal.GetComponent<Modal>().text = text.Replace("\\n", "\n");
        modal.GetComponent<Modal>().callback = callback;
        modal.transform.parent = Map.GetSingleton().canvas.transform;
        modal.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        modal.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        
        Map.GetSingleton().PauseMap();
    }
}
