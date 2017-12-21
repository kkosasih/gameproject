﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest : Item {

    public Quest(string name, int id, int output, string desc, int value, int stack, int amount)
    {
        itemName = name;
        itemId = id;
        itemOutput = output;
        itemDesc = desc;
        itemStack = stack;
        itemStackAmount = stack;
        itemValue = value;
        itemIcon = Resources.Load<Texture2D>("Item Icons/" + name);
    }
}