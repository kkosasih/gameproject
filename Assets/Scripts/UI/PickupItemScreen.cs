﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItemScreen : MonoBehaviour {
    #region Attributes
    public static PickupItemScreen instance;    // The instance to references
    private PickupInventory inventory;          // The inventory attached to the UI
    #endregion

    #region Event Functions
    void Awake ()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

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
    // Change the inventory that is shown
    public void ChangeInventory(PickupInventory newInv)
    {
        inventory = newInv;
    }

    // Take the item in the given slot and close if empty
    public void TakeItem (int index)
    {
        inventory.TransferItem(GameObject.Find("Inventory").GetComponent<Inventory>(), index);
        // Close if no items left
        if (inventory.IsEmpty())
        {
            Close();
        }
    }

    // Take all items
    public void TakeAllItems ()
    {
        inventory.TransferAllItems(GameObject.Find("Inventory").GetComponent<Inventory>());
        Close();
    }

    // Open the inventory and stop player movement
    public void Open ()
    {
        ++PlayerCharacter.instance.MovementPreventions;
        GetComponent<UIPanel>().IsOpen = true;
        inventory.UpdateImages();
    }

    // Close the inventory and continue player movement
    public void Close ()
    {
        --PlayerCharacter.instance.MovementPreventions;
        GetComponent<UIPanel>().IsOpen = false;
    }
    #endregion
}
