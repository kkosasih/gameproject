﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : DialoguePart {
    private float time3;
    private Vector3 moveTo;

    // Change the variables based on a string
    public override void ChangeSettings (string data)
    {
        base.ChangeSettings(data);
        string[] parameters = data.Split('|');
        time3 = float.Parse(parameters[2]);
        string[] vectorPoints = parameters[3].Split(',');
        moveTo = new Vector3(float.Parse(vectorPoints[0]), float.Parse(vectorPoints[1]), float.Parse(vectorPoints[2]));
    }

    // Wait for time1, move the camera for time3 seconds, then wait for time2
    public override IEnumerator PerformPart ()
    {
        isRunning = true;
        yield return new WaitForSeconds(time1);
        yield return StartCoroutine(Helper.LerpMovement(GameObject.FindWithTag("MainCamera"), moveTo, time3));
        yield return new WaitForSeconds(time2);
        isRunning = false;
    }
}
