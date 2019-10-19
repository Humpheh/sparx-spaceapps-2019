using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outbreak : MonoBehaviour
{
    private Color START_COLOR = new Color(1, 0.75f, 0, 1);
    private Color END_COLOR = new Color(1, 0, 0.05f, 0.3f);
    
    const float MAX_SIZE = 2f;
    private const float GROW_TIME = 60; // seconds

    public float growSpeed = 0.1f;
    public float alive = 0;
    
    // Start is called before the first frame update
    void Awake()
    {
        transform.localScale = new Vector3(0, 0, 0);
        growSpeed = Random.value + 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        alive += Time.deltaTime;

        if (alive < GROW_TIME * growSpeed)
        {
            var timer = alive / GROW_TIME * growSpeed;
            var scaleRatio = timer * MAX_SIZE;
            transform.localScale = new Vector3(scaleRatio, scaleRatio, scaleRatio);
            GetComponent<SpriteRenderer>().color = Color.Lerp(START_COLOR, END_COLOR, timer);
        }
    }
}
