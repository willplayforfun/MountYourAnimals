using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : Animal
{
    public override void Spawn(bool first)
    {
        base.Spawn(first);

        GameManager.Instance.abilityPrompt.SetActive(false);
    }

    protected override void DoAbility()
    {
        base.DoAbility();

        MouthTrigger trigger = GetComponentInChildren<MouthTrigger>();
        if (trigger != null && trigger.latestAnimal != null)
        {
            if(trigger.latestAnimal == this.latestHit.GetComponentInParent<Animal>())
            {
                latestHit = null;
                myJoint.enabled = false;
            }

            trigger.latestAnimal.Explode();
        }
    }

    protected override void Freeze()
    {
        base.Freeze();

        GetComponentInChildren<MouthTrigger>().enabled = false;
        Destroy(GetComponentInChildren<MouthTrigger>());
    }
}
