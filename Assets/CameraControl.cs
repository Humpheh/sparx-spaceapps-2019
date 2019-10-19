using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float dragMultipler = 2;
    private Vector3 dragOrigin;

    const float scrollMultiplier = -0.5f;

    private Vector2 earthSize = new Vector2(82, 41);

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 diff = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (diff.x + transform.position.x < -earthSize.x / 2)
        {
            diff.x = 0f;
        }
        else if (diff.x + transform.position.x > earthSize.x / 2)
        {
            diff.x = 0f;
        }
        if (diff.y + transform.position.y < -earthSize.y / 2)
        {
            diff.y = 0f;
        }
        else if (diff.y + transform.position.y > earthSize.y / 2)
        {
            diff.y = 0f;
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

    private void scroll(float s)
    {
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
