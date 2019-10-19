using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityControl : MonoBehaviour
{
    private static CityControl currentSelection;

    public Location location;
    
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
                if (currentSelection != null)
                {
                    currentSelection.Deselect();
                }
                currentSelection = this;
                Select();
            }
        }
    }

    void Select()
    {
        GetComponent<MeshRenderer>().material.color = new Color(0,1,1);
        transform.localScale = new Vector3(1, 1, 1);
    }

    void Deselect()
    {
        GetComponent<MeshRenderer>().material.color = new Color(1,1,0);
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }
}
