﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveChar : DialoguePart {
    #region Attributes
    private Character charToMove;   // The character script to act upon
    private List<Direction> moveTo; // The list of movement inputs for the character to move in
    #endregion

    #region Event Functions
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    #region Methods
    // Change the variables based on a string
    public override void ChangeSettings (string data)
    {
        base.ChangeSettings(data);
        string[] parameters = data.Split('|');
        charToMove = GameObject.Find(parameters[2]).GetComponent<Character>();
        moveTo = new List<Direction>();
        foreach (string s in parameters[3].Split(','))
        {
            switch (s)
            {
                case "U":
                    moveTo.Add(Direction.Up);
                    break;
                case "D":
                    moveTo.Add(Direction.Down);
                    break;
                case "L":
                    moveTo.Add(Direction.Left);
                    break;
                case "R":
                    moveTo.Add(Direction.Right);
                    break;
            }
        }
    }
    #endregion

    #region Coroutines
    // Wait for time1, path the character to the position, then wait for time2
    public override IEnumerator PerformPart()
    {
        isRunning = true;
        yield return new WaitForSeconds(time1);
        yield return StartCoroutine(charToMove.AutoMove(moveTo));
        yield return new WaitForSeconds(time2);
        isRunning = false;
    }
    #endregion
}
