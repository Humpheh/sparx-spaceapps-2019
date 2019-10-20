using System.Collections;
using System.Collections.Generic;
using mosquitodefenders.Tickers;
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

    private float GROW_TIME = 60; // seconds

    public MozEvent evt;
    public float growSpeed = 0.1f;
    public float alive = 0;

    public bool infection_risk;
    public bool deadly;
    public float population;
    public int toll;
    public float nearbyDoctorEffect;
    public bool nearbyDoctor;
    public float change;

    public string eventType;

    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log(this.eventType);
        transform.localScale = new Vector3(0, 0, 0);
        growSpeed = Random.value + 0.1f;
        population = Random.value * 10000; // Density based on location?
    }

    void Start()
    {
        GROW_TIME = evt.timer;
    }
    // Update is called once per frame
    void Update()
    {
        if (Map.GetSingleton().IsPaused()) return;

        eventType = infection_risk ? "outbreak" : "report";
        nearbyDoctorEffect = NearbyDoctorTimeMultipler();
        change = Time.deltaTime * nearbyDoctorEffect * 40;
        alive = Mathf.Clamp(alive + change, -0.2f, GROW_TIME * growSpeed);

        var timer = alive / GROW_TIME * growSpeed;
        var scaleRatio = timer * MAX_SIZE;

        if (alive < -0.01f)
        {
            Cure();
            return;
        }

        transform.localScale = new Vector3(scaleRatio, scaleRatio, scaleRatio);
        GetComponent<SpriteRenderer>().color = Color.Lerp(START_COLOR, END_COLOR, timer);

        deadly = GROW_TIME * growSpeed > CAT_1 && infection_risk && change >= 0.3f;
        

        if (deadly) toll = (int)Mathf.Round(population * Random.value / 200);
        //Debug.Log(toll);
        Resources.Dead.value += toll;
    }

    private void Cure()
    {
        GameObject checkPrefab = UnityEngine.Resources.Load("CheckIcon") as GameObject;
        Instantiate(checkPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);

        //this cancels future event spread - dispatch in the actual cure event
        SendMessage("CuredLocation", new DataPoint
        {
            latitude = evt.location.latitude,
            longitude = evt.location.longitude,
        });
    }

    public float NearbyDoctorTimeMultipler()
    {
        return Map.GetSingleton().NearbyDoctorTimeMultipler(transform.position);
    }
}
