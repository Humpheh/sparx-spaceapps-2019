using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckExpire : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ExpireTick());
    }

    IEnumerator ExpireTick()
    {
        yield return new WaitForSeconds(6f);
        for (float size = 0.25f; size > 0; size -= 0.01f)
        {
            transform.localScale = new Vector3(size, size, size);
            yield return new WaitForSeconds(0.2f);
        }
        Destroy(gameObject);
    }
}
