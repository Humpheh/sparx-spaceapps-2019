using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SplashEntry : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ToGame());
    }

    void Update()
    {
        if (Input.anyKey)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    IEnumerator ToGame()
    {
        yield return new WaitForSeconds(10);
        SceneManager.LoadScene("SampleScene");
    }
}
