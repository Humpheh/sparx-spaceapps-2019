using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBehaviour : MonoBehaviour
{
    public float maxSpeed = 2f;
    public float speed = 0.5f;
    public float timer;
    public Vector2 startPosition;
    public Vector2 endPosition;

    public GameObject line;

    // Update is called once per frame
    void Update()
    {
        var diff = (new Vector3(endPosition.x, endPosition.y, 0) - transform.position).normalized * Time.deltaTime * speed;
        speed = Mathf.Clamp(speed + Time.deltaTime / 2, 0, maxSpeed);
        transform.position += diff;
        timer += Time.deltaTime;

        var distance = new Vector2(transform.position.x, transform.position.y) - endPosition;
        if (distance.magnitude < 0.1)
        {
            Debug.Log("reached destination");
            Destroy(gameObject);
            Destroy(line);
        }
    }

    public static void SpawnPlane(Vector2 from, Vector2 to)
    {
        GameObject planePrefab = UnityEngine.Resources.Load("Plane") as GameObject;
        var plane = Instantiate(planePrefab, from, Quaternion.identity);
        var planeB = plane.GetComponent<PlaneBehaviour>();
        planeB.startPosition = from;
        planeB.endPosition = to;

        Material lineMaterial = UnityEngine.Resources.Load("NoLight") as Material;
        GameObject line = new GameObject("PlaneLine");
        var lineRender = line.AddComponent<LineRenderer>();
        lineRender.material = lineMaterial;
        lineRender.startWidth = 0.1f;
        lineRender.endWidth = 0.1f;
        lineRender.SetPositions(new[] { new Vector3(from.x, from.y, -0.1f), new Vector3(to.x, to.y, -0.1f) });
        planeB.line = line;
    }
}
