using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Phone : MonoBehaviour
{

	GameObject signal;
	int signalBar;
	public Sprite[] signalSprites;
	public GameObject signalIndicatorPivot;
	SpriteRenderer signalRenderer;

	private void Start () {
		
		signalRenderer = signalIndicatorPivot.GetComponentInChildren<SpriteRenderer>();
		signal = GameObject.Find("Signal1");

	}
	
	private void Update () {

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
        if (signalBar >= 3)
        {
            GameManager.Instance.patienceRef.inSignalRange = true;
        }
    }
	public void SubtractSignal()
	{
		signalBar--;
		signalRenderer.sprite = signalSprites[signalBar];
		print(signalBar);
        if (signalBar < 3)
        {
            GameManager.Instance.patienceRef.inSignalRange = false;
        }
    }
}
