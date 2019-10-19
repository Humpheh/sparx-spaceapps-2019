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

        if (Input.GetKey(KeyCode.Q))
        {
            scroll(1);
        }
        if (Input.GetKey(KeyCode.E))
        {
            scroll(-1);
        }

        transform.position = transform.position + diff.normalized * Time.deltaTime * 5;

        mouseZoom();
        mouseDrag();
    }

    private void scroll(float s) {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + s * scrollMultiplier, 2, 20);
    }

    private void mouseZoom()
    {
        var amount = Input.mouseScrollDelta.y;
        if (Mathf.Abs(amount) <= 0)
            return;
        this.scroll(amount);
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
