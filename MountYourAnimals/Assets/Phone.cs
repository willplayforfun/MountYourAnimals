using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phone : MonoBehaviour {

	int signalBar;

	public Sprite[] signalSprites;
	public GameObject signalIndicator;
	SpriteRenderer signalRenderer;

	// Use this for initialization
	void Start () {
		
		signalRenderer = signalIndicator.GetComponent<SpriteRenderer>();
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
	}
	public void SubtractSignal()
	{
		signalBar--;
		signalRenderer.sprite = signalSprites[signalBar];
		print(signalBar);
	}
}
