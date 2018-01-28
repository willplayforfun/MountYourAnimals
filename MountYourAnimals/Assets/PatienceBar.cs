using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatienceBar : MonoBehaviour {

    private float currentPatience;
    private float maxPatience = 100;
    private float patienceLossMultiplier = 0.75f;
    private float patienceGainMultiplier = 5;

    public bool inSignalRange = false;
    public bool inStopRange = false;

    [SerializeField]
    private Slider patienceBar;

    private int signalRef;

	private void Start ()
    {
        ResetPatience();
	}

    public void ResetPatience()
    {
        currentPatience = maxPatience;
    }

    private void Update ()
    {
        if (GameManager.Instance.isPlaying)
        {
            if (!inStopRange)
            {
                if (!inSignalRange)
                {
                    currentPatience -= Time.deltaTime * patienceLossMultiplier;
                }
                else if (currentPatience < maxPatience)
                {
                    currentPatience += Time.deltaTime * patienceGainMultiplier;
                }
            }
            
        }

        patienceBar.value = currentPatience / 100;

        if(currentPatience <= 0)
        {
            //you lose
            GameManager.Instance.GameOver();
        }

        //Debug.Log(patienceBar.value);

	}
}
