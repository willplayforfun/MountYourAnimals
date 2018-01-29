using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{
    [SerializeField]
    private Transform speechPivot;
    //[SerializeField]
    //private 
    [SerializeField]
    private TMPro.TextMeshPro speechText;
    [SerializeField]
    private Transform phone;
    [SerializeField]
    private Transform torso;

    private float speechPivotX;

    private HumanPart[] parts;

    private void Awake()
    {
        parts = GetComponentsInChildren<HumanPart>();
        //speechPivotX = s

        speechPivot.gameObject.SetActive(false);
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

    private void Update()
    {
        if(phone.position.x > torso.position.x)
        {
            // flip to be on right
        }
        else
        {
            // flip to be on left
        }
        speechPivot.position = phone.position;
    }

    [SerializeField]
    private string[] angryLines;
    [SerializeField]
    private string[] mehLines;
    [SerializeField]
    private string[] goodLines;
    int goodIndex = 0;

    internal bool cinematic = true;

    public void SaySomething()
    {
        if (!cinematic)
        {
            if (GameManager.Instance.patienceRef.currentPatience / GameManager.Instance.patienceRef.maxPatience < 0.5f)
            {
                if (angryLines.Length > 0)
                {
                    ShowTextBubble(angryLines[Random.Range(0, angryLines.Length)]);
                }
            }
            else
            {
                if (goodLines.Length > 0)
                {
                    ShowTextBubble(goodLines[goodIndex]);
                    goodIndex = Mathf.RoundToInt(Mathf.Repeat(goodIndex + 1, goodLines.Length));
                }
                }
        }
    }
    [SerializeField]
    private AudioSource source;
    [SerializeField]
    private AudioClip[] speechSounds;

    public void ShowTextBubble(string line)
    {
        if(routine != null)
        {
            StopCoroutine(routine);
        }
        source.PlayOneShot(speechSounds[Random.Range(0, speechSounds.Length)]);
        routine = StartCoroutine(TextBubbleRoutine(line));
    }
    private Coroutine routine;
    private IEnumerator TextBubbleRoutine(string line)
    {
        speechPivot.gameObject.SetActive(true);

        speechText.SetText(line);

        yield return new WaitForSeconds(3);

        speechPivot.gameObject.SetActive(false);

        routine = null;
    }
}
