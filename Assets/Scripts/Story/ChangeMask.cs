﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeMask : DialoguePart {
    #region Attributes
    private float time3;    // The time spent changing the mask
    private Color color;    // The color to change the mask to
    #endregion

    #region Event Functions
    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {

    }
    #endregion

    #region Methods
    // Change the variables based on a string
    public override void ChangeSettings (string data)
    {
        base.ChangeSettings(data);
        string[] parameters = data.Split('|');
        time3 = float.Parse(parameters[2]);
        string[] colorPoints = parameters[3].Split(',');
        color = new Color(float.Parse(colorPoints[0]), float.Parse(colorPoints[1]), float.Parse(colorPoints[2]), float.Parse(colorPoints[3]));
    }
    #endregion

    #region Coroutines
    // Wait for time1, change the mask for time3 seconds, then wait for time2
    public override IEnumerator PerformPart ()
    {
        isRunning = true;
        yield return new WaitForSeconds(time1);
        yield return StartCoroutine(Helper.ChangeColorInTime(GameObject.FindWithTag("UIMask").GetComponent<Image>(), color, time3));
        yield return new WaitForSeconds(time2);
        isRunning = false;
    }
    #endregion
}
