using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float dragMultipler = 20;
    private Vector3 dragOrigin;
    private Vector3 pos;
    //public GameObject canvas;

    const float scrollMultiplier = -0.5f;
    private float dragSpeed;

    private Vector2 earthSize = new Vector2(82, 41);
    private float limX, limY;

    // Start is called before the first frame update
    void Start()
    {
        limX = Mathf.Clamp((float)(1.725 * (20 - Camera.main.orthographicSize) + 6.6), 6.6f, 37.5f);
        limY = Mathf.Clamp((float)((20 - Camera.main.orthographicSize) + 0.5), 0.5f, 18.5f);
}

    // Update is called once per frame
    void Update()
    {
        if (Map.GetSingleton().IsPaused()) return;
        
        Vector3 diff = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        // -41 < x < 41
        // CS = 2, xbound  +- 37.5
        // CS = 6, xbound  +- 30.6
        // CS = 10, xbound  +- 23.7
        // CS = 14, xbound  +- 16.8
        // CS = 18, xbound  +- 10
        // CS = 20, xbound  +- 6.6

        // -20.5 < y < 20.5
        // CS = 2, xbound  +- 18.5
        // CS = 6, xbound  +- 14.5
        // CS = 10, xbound  +- 10.5
        // CS = 14, xbound  +- 6.5
        // CS = 18, xbound  +- 2.5
        // CS = 20, xbound  +- 0.5

        if (diff.x + transform.position.x < -limX)
        {
            diff.x = (-limX - transform.position.x) / 2;
        }
        else if (diff.x + transform.position.x > limX)
        {
            diff.x = (limX - transform.position.x) / 2;
        }
        if (diff.y + transform.position.y < -limY)
        {
            diff.y = (-limY - transform.position.y) / 2;
        }
        else if (diff.y + transform.position.y > limY)
        {
            diff.y = (limY - transform.position.y) / 2;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            scroll(1);
        }
        if (Input.GetKey(KeyCode.E))
        {
            scroll(-1);
        }

        transform.position = transform.position + diff * Time.deltaTime * 10;

        mouseZoom();
        mouseDrag();
    }

    private void scroll(float s)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + s * scrollMultiplier, 2, 20);
        limX = Mathf.Clamp((float)(1.725 * (20 - Camera.main.orthographicSize) + 6.6), 6.6f, 37.5f);
        limY = Mathf.Clamp((float)((20 - Camera.main.orthographicSize) + 0.5), 0.5f, 18.5f);
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
        if (Input.GetMouseButton(0))
        {
            pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            dragOrigin = Input.mousePosition;
            dragSpeed = Mathf.Sqrt(Camera.main.orthographicSize) * dragMultipler * 6;          
        }
        else
        {
            dragOrigin = Input.mousePosition;
            dragSpeed = (float)(dragSpeed - (Time.deltaTime * Camera.main.orthographicSize * 10));
            if (dragSpeed < 0)
            {
                dragSpeed = 0f;
            }
        }
        
        Vector3 move = new Vector3(-pos.x * dragSpeed, -pos.y * dragSpeed, 0);
        if (move.x + transform.position.x < -limX)
        {
            move.x = 0;
        }
        else if (move.x + transform.position.x > limX)
        {
            move.x = 0;
        }
        if (move.y + transform.position.y < -limY)
        {
            move.y = 0;
        }
        else if (move.y + transform.position.y > limY)
        {
            move.y = 0;
        }

        transform.Translate(move, Space.World);
    }
}
