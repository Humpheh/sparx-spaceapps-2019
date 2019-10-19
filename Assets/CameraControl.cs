using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float dragMultipler = 2;
    private Vector3 dragOrigin;

    const float scrollMultiplier = -0.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 diff = new Vector3();
        
        if (Input.GetKey(KeyCode.W))
        {
            diff.y += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            diff.y -= 1;
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            diff.x -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            diff.x += 1;
        }

        transform.position = transform.position + diff.normalized * Time.deltaTime * 5;

        mouseZoom();
        mouseDrag();
    }

    private void mouseZoom()
    {
        var scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0)
        {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + scroll * scrollMultiplier, 2, 20);
        }
    }
    
    private void mouseDrag()
    {
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        dragOrigin = Input.mousePosition;
 
        if (Input.GetMouseButton(0))
        {
            var dragSpeed = Camera.main.orthographicSize * dragMultipler;
            Vector3 move = new Vector3(-pos.x * dragSpeed, -pos.y * dragSpeed, 0);
            transform.Translate(move, Space.World);  
        }
    }
}
