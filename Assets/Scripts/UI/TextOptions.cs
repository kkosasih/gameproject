﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextOptions : MonoBehaviour {

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
    // Destroy current objects and create new ones
    public void CreateOptions (List<string> options)
    {
        // Destroy past objects
        //while (transform.childCount > 0)
        //{
        //    Destroy(transform.GetChild(0).gameObject);
        //}

		int count = transform.childCount;
	
		if (count > 0)
		{
			for (int i = 0; i < count; ++i)
			{
				Destroy (transform.GetChild (0).gameObject);
			}
		}

        // Make options buttons on screen depending on number assigned
        for (int i = 0; i < options.Count; ++i)
        {
            GameObject g = Instantiate(Resources.Load<GameObject>("GUI/ChoicePanel"), transform);
            RectTransform rt = g.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.2f, 0.35f + 0.1f * options.Count - 0.2f * i);
            rt.anchorMax = new Vector2(0.8f, 0.45f + 0.1f * options.Count - 0.2f * i);
            g.transform.Find("Text").GetComponent<Text>().text = options[i];
            g.GetComponent<ChoiceButton>().choice = (char)(i + 'A');
        }
    }
    #endregion
}
