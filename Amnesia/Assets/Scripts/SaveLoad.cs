﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoad {
    public static GameObject player = GameObject.FindWithTag("Player");
    public static GameObject map = GameObject.FindWithTag("Map");

    // Saves the needed game info
    public static void Save (int slot)
    {
        string slotNum = slot.ToString();
        if (player != null)
        {
            PlayerPrefs.SetString(slotNum + "SceneNum", SceneManager.GetActiveScene().name);
            PlayerPrefs.SetInt(slotNum + "PlayerTile", GameObject.FindWithTag("MainCamera").GetComponent<PlayerController>().currentTile);
        }
    }

    // Loads the needed game info
    public static void Load (int slot)
    {
        string slotNum = slot.ToString();
        if (player != null)
        {
            SceneManager.LoadScene(PlayerPrefs.GetString(slotNum + "SceneNum"));
            GameObject.FindWithTag("MainCamera").GetComponent<PlayerController>().MovePlayer(PlayerPrefs.GetInt(slotNum + "PlayerTile"));
        }
    }
}
