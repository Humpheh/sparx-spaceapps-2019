using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Outbreak : MonoBehaviour
{
    private Color START_COLOR = new Color(1, 0.75f, 0, 1);
    private Color END_COLOR = new Color(1, 0, 0.05f, 0.3f);

    const float MAX_SIZE = 2f;
    const float CAT_1 = 40f;
    const float CAT_2 = 20f;
    const float CAT_3 = 5f;

    private const float GROW_TIME = 60; // seconds

    public float growSpeed = 0.1f;
    public float alive = 0;

    public bool malariaRisk;
    public bool deadly;
    public float population;
    public int toll;
    public bool nearbyDoctor;

    // Start is called before the first frame update
    void Awake()
    {
        transform.localScale = new Vector3(0, 0, 0);
        growSpeed = Random.value + 0.1f;
        malariaRisk = Random.value > 0.5;
        deadly = false;
        population = Random.value * 10000f;
        nearbyDoctor = FindYourLocalDoctor();
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

        nearbyDoctor = FindYourLocalDoctor();

        deadly = GROW_TIME * growSpeed > CAT_1 && malariaRisk && nearbyDoctor == false;
        if (deadly) toll = (int)Mathf.Round(population * Random.value / 10000);
        Resources.Dead.value += toll;
    }

    public bool FindYourLocalDoctor()
    {
        Vector3 worldLocation = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        return Map.GetSingleton().FindDocterNearby(worldLocation);

    }
}
