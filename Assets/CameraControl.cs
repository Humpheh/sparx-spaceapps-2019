using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private Vector3 velocity;
    public float dragSpeed = 10;
    private Vector3 dragOrigin;

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

        Debug.Log(Input.mouseScrollDelta);
        
        mouseDrag();
    }

    private void mouseDrag()
    {
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(-pos.x * dragSpeed, -pos.y * dragSpeed, 0);
        dragOrigin = Input.mousePosition;
 
        if (Input.GetMouseButton(0))
        {
            transform.Translate(move, Space.World);  
        }
    }
}
