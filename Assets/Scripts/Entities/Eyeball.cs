﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Eyeball : Monster {
    #region Attributes

    #endregion

    #region Properties

    #endregion

    #region Event Functions
    // Update is called once per frame
    protected override void Update ()
    {
        // If not in a cutscene
        if (health > 0 && Preventions == 0 && OnMap)
        {
            // Attack player if in range
            if (Mathf.Abs(HoriDistance(PlayerCharacter.instance.CurrentTile)) <= 5 && VertDistance(PlayerCharacter.instance.CurrentTile) == 0 ||
                Mathf.Abs(VertDistance(PlayerCharacter.instance.CurrentTile)) <= 5 && HoriDistance(PlayerCharacter.instance.CurrentTile) == 0)
            {
                StartCoroutine(Attack(DirectionToward(PlayerCharacter.instance.CurrentTile)));
            }
        }
        base.Update();
    }
    #endregion

    #region Methods

    #endregion

    #region Coroutines
    // Attack in a given direction dir
    public override IEnumerator Attack (Direction dir)
    {
        ++movementPreventions;
        SetAllIntegers("direction", (int)dir);
        StartCoroutine(Helper.PlayInTime(animators, "attacking", true, false, 1f));
        yield return new WaitForSeconds(0.5f);
        AttackController.instance.StraightAttack(new Attack(teamID, 5, 0.2f), dir, CurrentTile, 4, 1, 5f);
        yield return new WaitForSeconds(0.5f);
        --movementPreventions;
    }
    #endregion
}
