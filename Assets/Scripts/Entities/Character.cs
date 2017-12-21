﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour {
    public static GameController controller;
    public int teamID;
    public int currentTile;
    public int movementPreventions = 0;
    public float delay;
    public float lastMove = 0.0f;
    public int lastTile = -1;
    public int movementType = 0;
    public int health;
    public int maxHealth;
    public GameObject healthSlider;
    protected bool attacked = false;
    protected Coroutine movementRoutine;

    protected virtual void Awake ()
    {
        healthSlider = Instantiate((GameObject)Resources.Load("GUI/CharHealthSlider"), GameObject.Find("DynamicCanvas").transform);
        controller = GameObject.FindWithTag("MainCamera").GetComponent<GameController>();
    }

    // Use this for initialization
    protected virtual void Start ()
    {
        transform.position = controller.map.tiles[currentTile].transform.position;
    }
	
	// Update is called once per frame
	protected virtual void Update ()
    {
        if (lastMove < delay)
        {
            lastMove += Time.deltaTime;
        }
        if (TileHurts() && !attacked)
        {
            ChangeHealth(health - controller.map.tiles[currentTile].GetComponent<Tile>().attackDamage);
            attacked = true;
        }
        else if (!TileHurts())
        {
            attacked = false;
        }
        if (health <= 0)
        {
            Die();
        }
	}

    // Move the character to another tile
    public virtual void Move (int moveTo)
    {
        if (controller.map.tiles[moveTo].GetComponent<Tile>().type != TileType.Wall)
        {
            lastMove = 0.0f;
            lastTile = currentTile;
            StartCoroutine(ChangeTile(moveTo));
            if (movementRoutine != null)
            {
                StopCoroutine(movementRoutine);
            }
            movementRoutine = StartCoroutine(Helper.PlayInTime(GetComponent<Animator>(), "moveType", 1, 0, Mathf.Min(0.5f, delay)));
        }
    }

    // Move the character by multiple tiles automatically
    public IEnumerator AutoMove (List<MoveOptions> path)
    {
        foreach (MoveOptions o in path)
        {
            switch (o)
            {
                case MoveOptions.Up:
                    ChangeTile(controller.map.TileAbove(currentTile));
                    break;
                case MoveOptions.Down:
                    ChangeTile(controller.map.TileBelow(currentTile));
                    break;
                case MoveOptions.Left:
                    ChangeTile(controller.map.TileLeft(currentTile));
                    break;
                case MoveOptions.Right:
                    ChangeTile(controller.map.TileRight(currentTile));
                    break;
            }
        }
        yield return new WaitForEndOfFrame();
    }

    // Put an attack on a tile for a given time
    public void Attack (int tile, int damage, float duration)
    {
        if (tile != -1)
        {
            StartCoroutine(controller.map.tiles[tile].GetComponent<Tile>().GiveAttack(teamID, damage, duration));
        }
    }

    // Kill this character
    public virtual void Die ()
    {
        Destroy(healthSlider.gameObject);
        Destroy(gameObject);
    }

    // Change the health of the character to the new value
    public void ChangeHealth (int newHealth)
    {
        if (newHealth < health)
        {
            StartCoroutine(LoseHealth(newHealth));
        }
        else
        {
            StartCoroutine(GainHealth(newHealth));
        }
        health = newHealth;
    }

    // Has the health bar react to gaining health
    private IEnumerator GainHealth (int newHealth)
    {
        healthSlider.transform.GetChild(0).Find("Fill Area").Find("Fill").gameObject.GetComponent<Image>().color = new Color(0, 1, 0);
        healthSlider.transform.GetChild(0).gameObject.GetComponent<Slider>().value = newHealth * 100f / maxHealth;
        yield return new WaitForSeconds(1);
        healthSlider.transform.GetChild(1).gameObject.GetComponent<Slider>().value = newHealth * 100f / maxHealth;
    }

    // Has the health bar react to losing health
    private IEnumerator LoseHealth (int newHealth)
    {
        healthSlider.transform.GetChild(1).gameObject.GetComponent<Slider>().value = newHealth * 100f / maxHealth;
        healthSlider.transform.GetChild(0).Find("Fill Area").Find("Fill").gameObject.GetComponent<Image>().color = new Color(0.5f, 0, 0);
        yield return new WaitForSeconds(1);
        healthSlider.transform.GetChild(0).gameObject.GetComponent<Slider>().value = newHealth * 100f / maxHealth;
    }

    // Performs all of the movement to another tile
    private IEnumerator ChangeTile (int moveTo)
    {
        Vector3 oldPos = transform.position;
        Vector3 newPos = controller.map.tiles[moveTo].transform.position;
        for (float timePassed = 0; timePassed < Mathf.Min(0.5f, delay); timePassed += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(oldPos, newPos, timePassed / Mathf.Min(0.5f, delay));
            yield return new WaitForEndOfFrame();
        }
        transform.position = newPos;
        currentTile = moveTo;
        HandleTile();
    }

    // Performs all special actions that a tile would perform if moved to
    protected virtual void HandleTile ()
    {
        if (TileHurts())
        {
            ChangeHealth(health - controller.map.tiles[currentTile].GetComponent<Tile>().attackDamage);
            attacked = true;
        }
    }

    // Checks if the tile is harmful to the character
    protected bool TileHurts ()
    {
        return controller.map.tiles[currentTile].GetComponent<Tile>().attackID != 0 && controller.map.tiles[currentTile].GetComponent<Tile>().attackID != teamID;
    }
}

public enum MoveOptions
{
    Up,
    Down,
    Left,
    Right
}