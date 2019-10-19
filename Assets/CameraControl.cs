using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private Vector3 velocity;
    
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
            velocity.y += 1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            velocity.y -= 1;
        }

        transform.position = transform.position + diff.normalized * Time.deltaTime;
    }
}
