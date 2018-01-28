using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phone : MonoBehaviour {

	GameObject signal;
	int signalBar;
	public Sprite[] signalSprites;
	public GameObject signalIndicatorPivot;
	SpriteRenderer signalRenderer;

	// Use this for initialization
	void Start () {
		
		signalRenderer = signalIndicatorPivot.GetComponentInChildren<SpriteRenderer>();
		signal = GameObject.Find("Signal1");

	}
	
	// Update is called once per frame
	void Update () {

        if (signalIndicatorPivot != null)
        {
            signalIndicatorPivot.transform.position = transform.position + new Vector3(0, 1, 0);
			signalIndicatorPivot.transform.LookAt(signal.transform, Vector3.forward);
		}

		if(signalBar == 3)
		{
			GameManager.Instance.score += 1*Time.deltaTime;
		}
		if(signalBar == 4)
		{
			GameManager.Instance.score += 1.5f*Time.deltaTime;
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
