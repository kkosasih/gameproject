﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawInventory : MonoBehaviour {
    #region Attributes
    public List<Item> inventory = new List<Item>();
	public List<Item> slots = new List<Item>();
	//public GUISkin skin;
	public GameObject Itemdata;
	public GameObject screen;
	public GameObject inventorySlots;
	public GameObject slot;
	public Texture2D slot_texture;
	public Texture2D equip_slot;
	//private bool showinventory = true;
	//private bool showtooltip = false;
	//private ItemDatabase database;
	private bool draw = false;
    #endregion

    #region Event Functions
    void Awake ()
    {
        for (int i = 0; i < 21; i++) //remeber to increase based on database
        {
            slots.Add(new Item());
        }

        for (int i = 0; i < slots.Count; i++)
        {
            //GameObject newSlot = (GameObject)Instantiate (slot);
            //newSlot.transform.SetParent (inventorySlots.transform);
            GameObject newSlot = (GameObject)Instantiate(Resources.Load("Slot"), GameObject.Find("Screen").transform);
            if (i < inventory.Count)
            {
                newSlot.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Item Icons/" + inventory[i].itemName);
                newSlot.transform.GetChild(0).gameObject.GetComponent<RectTransform>().offsetMin = newSlot.GetComponent<RectTransform>().offsetMin;
                newSlot.transform.GetChild(0).gameObject.GetComponent<RectTransform>().offsetMax = newSlot.GetComponent<RectTransform>().offsetMax;
            }
        }
    }

    // Use this for initialization
    void Start ()
    {
        //Starting Items
        for (int i = 0; i < GameController.instance.Database.items.Count; i++)
        {
            inventory.Add(GameController.instance.Database.items[i]);
        }
    }

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.I))
			draw = !draw;
		if (Input.GetKeyDown (KeyCode.Q) || Input.GetKeyDown (KeyCode.Escape))
			draw = false;
		screen.SetActive (draw);
		inventorySlots.SetActive (draw);
	}
    #endregion
}
