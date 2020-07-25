using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CountDownTimer : MonoBehaviour
{
    public float timeLeft = 3.0f;
    public Text countTimeText;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            SceneManager.LoadScene("MyScenes/MainScene");
        }
        else if (timeLeft < 1)
        {
            countTimeText.text = "Ready";
        }
        else
        {
            countTimeText.text = (timeLeft).ToString("0");
        }
    }
}