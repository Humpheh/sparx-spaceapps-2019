using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBehaviour : MonoBehaviour
{
    public float maxScale = 0.5f;
    public float maxSpeed = 2f;
    public Vector2 startPosition;
    public Vector2 endPosition;
    
    public GameObject line;

    // Update is called once per frame
    void Update()
    {
        if (Map.GetSingleton().IsPaused()) return;

        var distanceStart = Vector2.Distance(startPosition, transform.position);
        var distanceEnd = Vector2.Distance(endPosition, transform.position);

        float scale = 1f;
        if (distanceStart < 4f) scale = distanceStart / 4f;
        if (distanceEnd < 4f) scale = distanceEnd / 4f;

        // Update the scale based on distance from start
        var planeScale = Mathf.Clamp((scale + 0.5f) * maxScale, 0.2f, maxScale);
        transform.localScale = new Vector3(planeScale, planeScale, planeScale);        
        
        // Update the position based on calculated speed
        var speed = maxSpeed * scale + 0.1f;
        transform.position += (new Vector3(endPosition.x, endPosition.y, transform.position.z) - transform.position).normalized * Time.deltaTime * speed;

        if (distanceEnd < 0.2f)
        {
            Debug.Log("reached destination");
            Destroy(gameObject);
            Destroy(line);
            Map.GetSingleton().DropDoctor(endPosition);
        }
    }

    public static void SpawnPlane(Vector2 from, Vector2 to)
    {
        var from3 = new Vector3(from.x, from.y, -6f);
        var to3 = new Vector3(to.x, to.y, -6f);
        
        Material lineMaterial = UnityEngine.Resources.Load("NoLight") as Material;
        GameObject line = new GameObject("PlaneLine");
        var lineRender = line.AddComponent<LineRenderer>();
        lineRender.material = lineMaterial;
        lineRender.material.color = new Color(0.8f, 0.8f, 0.8f);
        lineRender.startWidth = 0.05f;
        lineRender.endWidth = 0.05f;
        lineRender.numCornerVertices = 4;
        lineRender.SetPositions(new[] { new Vector3(from.x, from.y, -5f), new Vector3(to.x, to.y, -5f) });

        var dir = from - to;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 135;
        
        GameObject planePrefab = UnityEngine.Resources.Load("Plane") as GameObject;
        var plane = Instantiate(planePrefab, from3, Quaternion.AngleAxis(angle, Vector3.forward));
        var planeB = plane.GetComponent<PlaneBehaviour>();
        planeB.startPosition = from3;
        planeB.endPosition = to3;
        planeB.line = line;

        Resources.Bank.Spend(30000);

    }
}
