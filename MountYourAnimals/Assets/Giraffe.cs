using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giraffe : Animal
{
    [SerializeField]
    private Transform neckContainer;
    [SerializeField]
    private float minScale = 0.175f;
    [SerializeField]
    private float maxScale = 1;
    [SerializeField]
    private float extendSpeed = 1;

    [Space(12)]

    [SerializeField]
    private Transform headAttachPoint;
    [SerializeField]
    private Transform head;

    private bool extended;

    protected override void DoAbility()
    {
        base.DoAbility();

        if(!extended)
        {
            extended = true;
            LeanTween.scaleY(neckContainer.gameObject, maxScale, extendSpeed).setEaseInOutQuad();
        }
    }

    protected override void Update()
    {
        base.Update();

        head.transform.position = headAttachPoint.transform.position;
        head.transform.rotation = headAttachPoint.transform.rotation;
    }
}
