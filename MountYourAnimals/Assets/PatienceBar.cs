using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatienceBar : MonoBehaviour {

    private float currentPatience;
    private float maxPatience = 100;
    private float patienceLossMultiplier = 1;
    private float patienceGainMultiplier = 3;

    public bool inSignalRange = false;

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
            if (!inSignalRange)
            {
                currentPatience -= Time.deltaTime * patienceLossMultiplier;
            }
            else if (currentPatience < maxPatience)
            {
                currentPatience += Time.deltaTime * patienceGainMultiplier;
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
