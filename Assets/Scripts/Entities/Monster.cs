﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Monster : Character {
    #region Attributes
    //public PlayerCharacter player;  // The player script to track
    //public bool pickuptile = false;
    protected GameObject canvas;
    [SerializeField]
    protected int range;            // The range before player detection
    [SerializeField]
    protected bool dead = false;    // Whether the monster is dead
    protected DungeonRoom room;     // The room that the monster is in
    #endregion

    #region Event Functions
    // Use this for initialization
    protected void Start ()
    {

    }

    // Update is called once per frame
    protected override void Update ()
    {
        canvas = GameObject.Find("Canvas");
        // If not in a cutscene
        if (GameController.instance.map == room && health > 0 && Preventions == 0 && OnMap)
        {
            // Attack player if in range
            if (Mathf.Abs(HoriDistance(PlayerCharacter.instance.CurrentTile)) <= 4 && VertDistance(PlayerCharacter.instance.CurrentTile) == 0 ||
                Mathf.Abs(VertDistance(PlayerCharacter.instance.CurrentTile)) <= 4 && HoriDistance(PlayerCharacter.instance.CurrentTile) == 0)
            {
                StartCoroutine(Attack(DirectionToward(PlayerCharacter.instance.CurrentTile)));
            }
            else if (lastMove >= delay && !moving)
            {
                // Move to player if detected
                if (TotalDistance(PlayerCharacter.instance.CurrentTile) <= range)
                {
                    Move(DirectionToward(PlayerCharacter.instance.CurrentTile));
                }
                // Move randomly if not detected
                else
                {
                    Move((Direction)Random.Range(0, 4));
                }
            }
        }
        // Die after animation
        if (dead)
        {
            DropItem();
        }
        base.Update();
    }
    #endregion

    #region Methods
    // Kill this character
    public override void Die ()
    {
        canvas.GetComponent<QuestTracking>().questobj(name);
        SetAllBools("dead", true);
    }

    // Place the object at the point on the map
    public override void PlaceOnMap (int tile)
    {
        if (room == null)
        {
            room = transform.parent.GetComponentInParent<DungeonRoom>();
        }
        if (room != null)
        {
            if (!room.TakenTiles.ContainsKey(this))
            {
                room.TakenTiles.Add(this, tile);
            }
            else
            {
                CurrentTile = tile;
            }
            transform.position = room.Tiles[tile].transform.position;
        }
    }

    // Delete the character and drop an item
    protected void DropItem ()
    {
        GameObject drop = GameController.instance.map.ObjectTakingTile(CurrentTile);
        // If the tile isn't already a pickup tile
        if (drop == null || drop.GetComponent<PickupInventory>() == null)
        {
            drop = Instantiate(Resources.Load<GameObject>("OtherPrefabs/ItemDrop"));
            StaticObject s = drop.GetComponent<StaticObject>();
            s.PlaceOnMap(CurrentTile);
            drop.GetComponent<PickupInventory>().SetSize(4);
        }
        // Add items here
        drop.GetComponent<PickupInventory>().AddItemByID(2);
        base.Die();
    }
    #endregion

    #region Coroutines
    // Attack in a given direction dir
    public override IEnumerator Attack (Direction dir)
    {
        ++movementPreventions;
        SetAllIntegers("direction", (int)dir);
        StartCoroutine(Helper.PlayInTime(animators, "attacking", true, false, 1f));
        yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(1f);
        --movementPreventions;
    }
    #endregion
}