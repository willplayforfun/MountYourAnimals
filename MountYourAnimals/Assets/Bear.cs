using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : Animal
{
    protected override void DoAbility()
    {
        base.DoAbility();

        MouthTrigger trigger = GetComponentInChildren<MouthTrigger>();
        if (trigger.latestAnimal != null)
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
    }
}
