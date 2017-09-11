using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{

    private static float
        timer;


    private void Update()
    {
        Debug.Log("Hello");
        if(timer > 0)
        {
            ProgressTimer();
        }
    }

    //Counts down from val. True = counting, False = ended.
    public static void StartTimer(float val)
    {
        if (timer == 0)
        {
            timer = val;
        }

        else
        {
            Debug.Log("StartTimer error!");
        }
    }

    public static bool TimerStatus()
    {
        if (timer <= 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private static void ProgressTimer()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = 0;
        }
        Debug.Log(Mathf.Floor(timer));
    }

}