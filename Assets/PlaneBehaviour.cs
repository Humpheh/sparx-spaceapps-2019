using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBehaviour : MonoBehaviour
{
    public float speed = 1f;
    public float timer;
    public Vector2 startPosition;
    public Vector2 endPosition;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.Lerp(startPosition, endPosition, timer * speed);
        timer += Time.deltaTime;

        var distance = new Vector2(transform.position.x, transform.position.y) - endPosition;
        if (distance.magnitude < 0.01)
        {
            Debug.Log("reached destination");
            Destroy(gameObject);
        }
    }
}
