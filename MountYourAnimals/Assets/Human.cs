﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{
    private HumanPart[] parts;

    private void Awake()
    {
        parts = GetComponentsInChildren<HumanPart>();
    }

    private Animal currentGrabber;

    public void UnstickFromCurrentAnimal()
    {
        if(currentGrabber != null)
        {
            currentGrabber.UnstickFromHuman();
        }
    }
    public void SetGrabbingAnimal(Animal newAnimal)
    {
        currentGrabber = newAnimal;
    }

    public void EnableCameraFocus()
    {
        foreach(HumanPart p in parts)
        {
            p.EnableCameraFocus();
        }
    }
    public void DisableCameraFocus()
    {
        foreach (HumanPart p in parts)
        {
            p.DisableCameraFocus();
        }
    }
}
