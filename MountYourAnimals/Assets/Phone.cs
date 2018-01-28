using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Phone : MonoBehaviour {

	int signalBar;

	public Sprite[] signalSprites;
	public GameObject signalIndicator;
	SpriteRenderer signalRenderer;

    PatienceBar patienceRef;

	// Use this for initialization
	void Start () {
		
		signalRenderer = signalIndicator.GetComponent<SpriteRenderer>();
        patienceRef = GameObject.Find("Patience").GetComponent<PatienceBar>();
	}
	
	// Update is called once per frame
	void Update () {

        if (signalIndicator != null)
        {
            signalIndicator.transform.position = transform.position + new Vector3(0, 1, 0);
        }
	}	

	public void AddSignal()
	{
		signalBar++;
		signalRenderer.sprite = signalSprites[signalBar];
		print(signalBar);
        if (signalBar == 3)
        {
            patienceRef.inSignalRange = true;
        }
    }
	public void SubtractSignal()
	{
		signalBar--;
		signalRenderer.sprite = signalSprites[signalBar];
		print(signalBar);
        if (signalBar != 3)
        {
            patienceRef.inSignalRange = false;
        }
    }
}
