using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Phone : MonoBehaviour

{
	int signalBar;
	public Sprite[] signalSprites;
	public GameObject signalIndicatorPivot;
	SpriteRenderer signalRenderer;

//	public AudioMixer bMixer;
//	public AudioMixerSnapshot[] bMixerSnapshots;
//	public float[] bMixerWeights;
//
//	public float backgroundAudioMinY;
//	public float backgroundAudioMaxY;

	private void Start () {
		
		signalRenderer = signalIndicatorPivot.GetComponentInChildren<SpriteRenderer>();
	}
	
	private void Update () {

//		float spacePercent = (gameObject.transform.position.y - backgroundAudioMinY)
//			/ (backgroundAudioMaxY - backgroundAudioMinY);
//
//		bMixerWeights [0] = 1 - spacePercent;
//		bMixerWeights [1] = spacePercent;
//		bMixer.TransitionToSnapshots(bMixerSnapshots, bMixerWeights, 0f);

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
		print(signalBar);
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
		print(signalBar);
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
