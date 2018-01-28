using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : Animal
{
    public override void Spawn(bool first)
    {
        base.Spawn(first);

        GameManager.Instance.abilityPrompt.SetActive(false);
    }

    protected override void DoAbility()
    {
        base.DoAbility();
    }
}
