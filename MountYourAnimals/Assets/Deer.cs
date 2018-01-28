using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deer : Animal
{
    [Space(12)]
    [SerializeField]
    private GameObject antlers;

    protected override void DoAbility()
    {
        base.DoAbility();

        antlers.AddComponent<Rigidbody2D>();
        antlers.transform.SetParent(this.transform.parent);
        if (antlerJoint != null)
        {
            antlerJoint.enabled = false;

            FixedJoint2D joint = antlers.AddComponent<FixedJoint2D>();
            joint.connectedBody = antlerJoint.connectedBody;
        }
    }

    private FixedJoint2D antlerJoint;
    private bool antlersStuck;

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if(!antlersStuck && collision.collider.tag == "Antlers" && collision.otherCollider.tag == "Antlers")
        {
            antlersStuck = true;
            antlerJoint = this.gameObject.AddComponent<FixedJoint2D>();
            antlerJoint.connectedBody = collision.collider.GetComponentInParent<Rigidbody2D>();
        }
    }
}
