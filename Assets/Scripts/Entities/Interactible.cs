﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void Simple();

public class Interactible : MonoBehaviour {
    public Simple interact; // Function to interact with
    public int range;       // The range that the player can interact at
    public bool automatic;  // Whether the interaction happens automatically

    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {

    }
}
