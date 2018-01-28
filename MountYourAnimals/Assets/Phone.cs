using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Phone : MonoBehaviour
{
	int signalBar;
	public Sprite[] signalSprites;
	public GameObject signalIndicatorPivot;
	SpriteRenderer signalRenderer;

	private void Start () {
		
		signalRenderer = signalIndicatorPivot.GetComponentInChildren<SpriteRenderer>();
	}
	
	private void Update () {

        if (signalIndicatorPivot != null)
        {
            signalIndicatorPivot.transform.position = transform.position;
            if (signalBar >= 3)
            {
                signalIndicatorPivot.transform.LookAt(signalIndicatorPivot.transform.position + Vector3.up, Vector3.forward);
            }
            else
            {
                signalIndicatorPivot.transform.LookAt(GameManager.Instance.signal.transform, Vector3.forward);
            }
        }

		if(signalBar == 3)
		{
			GameManager.Instance.score += 1*Time.deltaTime;
		}
		if(signalBar == 4)
		{
			GameManager.Instance.score += 1*Time.deltaTime;
		}
	}	

	public void AddSignal()
	{
		signalBar++;
		signalRenderer.sprite = signalSprites[signalBar];
		// print(signalBar);
        if (signalBar >= 3)
        {
            GameManager.Instance.patienceRef.inSignalRange = true;
            GameManager.Instance.patienceRef.inStopRange = false;
        }
        if (signalBar == 2)
        {

            GameManager.Instance.patienceRef.inStopRange = true;
        }
    }
	public void SubtractSignal()
	{
		signalBar--;
		signalRenderer.sprite = signalSprites[signalBar];
		// print(signalBar);
        if (signalBar < 3)
        {
            GameManager.Instance.patienceRef.inSignalRange = false;
            GameManager.Instance.patienceRef.inStopRange = true;

        }
        if(signalBar < 2)
        {
            GameManager.Instance.patienceRef.inStopRange = false;
        }
        
    }
}
