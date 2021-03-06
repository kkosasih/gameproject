﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Character {
    #region Attributes
    [SerializeField]
    protected List<string> path;                        // The list of conversations given
    protected string currentPath;                       // The conversation to bring up when next talked to
    protected Dictionary<string, string> nextConvos;    // Storage of conversation paths
    protected int pathIndex;                            // The index of the path list
    #endregion

    #region Event Functions
    protected override void Awake ()
    {
        base.Awake();
        currentPath = path[0];
        nextConvos = new Dictionary<string, string>();
        for (int i = 0; i < path.Count; ++i)
        {
            nextConvos.Add(path[i], path[Mathf.Min(i + 1, path.Count - 1)]);
        }
        Interactible inter = GetComponent<Interactible>();
        if (inter != null)
        {
            inter.interact = Talk;
        }
    }

    // Use this for initialization
    void Start ()
    {
		
	}
    #endregion

    #region Methods
    // Trigger a conversation and prepare the next
    public void Talk ()
    {
        DialogueController.instance.ChangeConversation("NPC/" + currentPath);
        currentPath = nextConvos[currentPath];
    }
    #endregion
}
