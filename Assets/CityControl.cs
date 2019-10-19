using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityControl : MonoBehaviour
{
    public static CityControl CurrentSelection;

    public Location location;
    public GameObject text;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)){ 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform == transform){
                Debug.LogFormat("clicked city {0}", location.city);
                
                var lastSelection = CurrentSelection;
                if (CurrentSelection != null) CurrentSelection.Deselect();
                if (lastSelection != this) Select();
            }
        }
        
        RectTransform canvasRect = Map.GetSingleton().canvas.GetComponent<RectTransform>();
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        Vector2 WorldObject_ScreenPosition=new Vector2(
            ((viewportPosition.x*canvasRect.sizeDelta.x)-(canvasRect.sizeDelta.x*0.5f)),
            ((viewportPosition.y*canvasRect.sizeDelta.y)-(canvasRect.sizeDelta.y*0.5f)));
 
        //now you can set the position of the ui element
        var uiElement = text.GetComponent<RectTransform>();
        uiElement.anchoredPosition = WorldObject_ScreenPosition;
    }

    void Select()
    {
        CurrentSelection = this;
        GetComponent<MeshRenderer>().material.color = new Color(0,1,1);
        transform.localScale = new Vector3(1, 1, 1);
    }

    public void Deselect()
    {
        GetComponent<MeshRenderer>().material.color = new Color(1,1,0);
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        CurrentSelection = null;
    }
}
