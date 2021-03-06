﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    // --- STARTUP STUFF ---
    [SerializeField]
    private bool autoStart;
    [SerializeField]
    private bool debugControlsStartShowing;

    // singleton/global access
    public static GameManager Instance { get; private set; }
    internal AnimalSpawner AnimalSpawner { get; private set; }

    List<float> scores = new List<float>();


    private void Awake()
    {
        Instance = this;

        if (PlayerPrefs.HasKey("Scores"))
        {
            int numScores = PlayerPrefs.GetInt("Scores");
            for (int i = 0; i < numScores; i++)
            {
                Debug.Log("Score " + i + ": " + PlayerPrefs.GetFloat("Score" + i));
                scores.Add(PlayerPrefs.GetFloat("Score" + i));
            }
        }
        else
        {
            PlayerPrefs.SetInt("Scores", 0);
        }

        // TODO intro sequence?

        startUI.SetActive(true);
        endUI.SetActive(false);
        gameUI.SetActive(false);
        freezePrompt.SetActive(false);
        grabPrompt.SetActive(false);
        movePrompt.SetActive(false);
        roundPrompt.SetActive(false);
        abilityPrompt.SetActive(false);
        skipPrompt.SetActive(false);
        unstickPrompt.SetActive(false);
#if UNITY_EDITOR
        debugControlsUI.SetActive(debugControlsStartShowing);
#else
        debugControlsUI.SetActive(false);
#endif

        AnimalSpawner = GetComponent<AnimalSpawner>();

        signalStartPosition = signal.transform.position;

        // create a human initially
        SpawnHuman();
    }
    private void Start()
    {
        if (autoStart)
        {
            StartPlay(true);
        }
    }
    // ----------------

    [SerializeField]
    internal GameObject signal;
    private Vector3 signalStartPosition;

    [Space(12)]

    [SerializeField]
    private GameObject startUI;
    [SerializeField]
    private GameObject debugControlsUI;
    [SerializeField]
    private GameObject gameUI;
    [SerializeField]
    private GameObject endUI;

    [SerializeField]
    internal GameObject skipPrompt;
    [SerializeField]
    internal GameObject freezePrompt;
    [SerializeField]
    internal GameObject grabPrompt;
    [SerializeField]
    internal GameObject movePrompt;
    [SerializeField]
    internal GameObject roundPrompt;
    [SerializeField]
    internal GameObject unstickPrompt;
    [SerializeField]
    internal GameObject abilityPrompt;
    [SerializeField]
    internal UnityEngine.UI.Text abilityName;
    [SerializeField]
    internal NextUpPanel nextAnimalPanel;
    [SerializeField]
    internal PatienceBar patienceRef;
    [SerializeField]
    private UnityEngine.UI.Text yourScore;
    [SerializeField]
    private UnityEngine.UI.Text relativeScore;
	[SerializeField]
	public GameObject backgroundMusic;
	[SerializeField]
	public AudioMixerSnapshot forestAudioSnapshot;

    // --- HUMAN STUFF ---
    [Space(12)]
    [SerializeField]
    private GameObject humanPrefab;
    private GameObject humanInstance;
    [SerializeField]
    private Transform humanSpawnPoint;

    private void SpawnHuman()
    {
        if (humanInstance != null)
        {
            Destroy(humanInstance);
        }
        humanInstance = Instantiate(humanPrefab, humanSpawnPoint.position, humanSpawnPoint.rotation);
    }
    // ----------------

    // --- GAME FLAGS ---
    
    // are we playing, or has the game ended / not started?
    internal bool isPlaying { get; private set; }

    // how many animals have we placed? starts at 0, 
    // goes up by 1 after each animal is created
    internal int roundNumber { get; private set; }
    
    // true if we are waiting for player input to start the next round
    internal bool waitingToStartRound { get; private set; }

    internal float score;
    private float startTime;

    // ----------------

    // called by UI to start the game
    public void StartPlay(bool cinematic)
    {
        if (!isPlaying)
        {
            if (cinematic)
            {
                // start the game with intro sequence
                introRoutine = StartCoroutine(IntroSequence());
            }
            else
            {
                humanInstance.GetComponentInChildren<Phone>().rotateBar = true;
                humanInstance.GetComponentInChildren<Human>().cinematic = false;
                AnimalSpawner.SpawnAnimal();
            }

            waitingToStartRound = false;
            isPlaying = true;
            startUI.SetActive(false);
            gameUI.SetActive(true);
            endUI.SetActive(false);
            // TODO fade out UI instead of immediately hiding, animate in game UI
        }
    }

    private Coroutine introRoutine;
    private IEnumerator IntroSequence()
    {
        skipPrompt.SetActive(true);
        // TODO start dialogue
        yield return new WaitForSeconds(1);
        humanInstance.GetComponentInChildren<Phone>().AddSignal(true);
        yield return new WaitForSeconds(0.5f);
        humanInstance.GetComponentInChildren<Phone>().AddSignal(true);
        yield return new WaitForSeconds(0.5f);
        humanInstance.GetComponentInChildren<Phone>().AddSignal(true);
        yield return new WaitForSeconds(0.5f);
        humanInstance.GetComponent<Human>().ShowTextBubble("I see you're applying for a job...");
        yield return new WaitForSeconds(5);
        humanInstance.GetComponent<Human>().ShowTextBubble("Hello? Are you there?");
        yield return new WaitForSeconds(5);
        humanInstance.GetComponentInChildren<Phone>().SubtractSignal(true);
        yield return new WaitForSeconds(1f);
        humanInstance.GetComponentInChildren<Phone>().SubtractSignal(true);
        yield return new WaitForSeconds(1f);
        humanInstance.GetComponentInChildren<Phone>().SubtractSignal(true);
        yield return new WaitForSeconds(1f);
        humanInstance.GetComponentInChildren<Phone>().rotateBar = true;
        humanInstance.GetComponentInChildren<Human>().cinematic = false;

        humanInstance.GetComponentInChildren<Human>().ShowTooltip(true);
        StartCoroutine(TurnOffTooltip());

        // load in initial animal
        AnimalSpawner.SpawnAnimal();
		backgroundMusic.SetActive (true);

        introRoutine = null;
        skipPrompt.SetActive(false);

        startTime = Time.time;
    }
    private IEnumerator TurnOffTooltip()
    {
        yield return new WaitForSeconds(5);

        humanInstance.GetComponentInChildren<Human>().ShowTooltip(false);
    }

    // called by the actively controlled Animal when the player freezes it
    public void CurrentAnimalFrozen()
    {
        waitingToStartRound = true;

        // show the stack
        AnimalSpawner.ShowStack();
        roundPrompt.SetActive(true);
        nextAnimalPanel.PopOut();
        humanInstance.GetComponent<Human>().EnableCameraFocus();

        //Combined Freeze + Next round button press into one 
        //StartNextRound();
    }

    // called in GameManager's Update() function when the player presses space
    public void StartNextRound()
    {
        waitingToStartRound = false;

        AnimalSpawner.HideStack();
        roundPrompt.SetActive(false);
        nextAnimalPanel.Retract();
        humanInstance.GetComponent<Human>().DisableCameraFocus();

        // start next round
        AnimalSpawner.SpawnAnimal();
        roundNumber += 1;
    }

    // called when patience reaches zero, ending the game
    public void GameOver()
    {
        if (isPlaying)
        {

			backgroundMusic.SetActive (false);
			forestAudioSnapshot.TransitionTo (1f);

            // TODO reset the game
            // i.e. reset flags, reset score, music, etc.

            score = 0;
            lastSignalChange = 0;
            // reset patience level
            patienceRef.ResetPatience();
            roundNumber = 0;
            // delete human and create a new one
            SpawnHuman();
            // delete animals
            AnimalSpawner.ClearAllAnimals();
            signal.transform.position = signalStartPosition;

            isPlaying = false;
            startUI.SetActive(false);
            gameUI.SetActive(false);
            endUI.SetActive(true);
            // TODO some sort of transition


            //END
            float totalTime = Time.time - startTime;
            int numLess = 0;
            for (int i = 0; i < scores.Count; i++)
            {
                if(scores[i] < totalTime)
                {
                    numLess++;
                }
            }
            yourScore.text = "(" + Mathf.RoundToInt(totalTime) + " seconds)";
            if (scores.Count == 0)
            {
                relativeScore.text = "and he was the only candidate.";
            }
            else
            {
                relativeScore.text = "but he stayed on the phone longer than " + Mathf.RoundToInt(100 * numLess / scores.Count) + "% of candidates.";
            }

            //add score
            PlayerPrefs.SetFloat("Score" + scores.Count, totalTime);
            scores.Add(totalTime);
            PlayerPrefs.SetInt("Scores", scores.Count);

        }
    }

    public void ReturnToMainMenu()
    {
        startUI.SetActive(true);
        gameUI.SetActive(false);
        endUI.SetActive(false);
        // TODO some sort of transition
    }

    // called by UI to quit the application
    public void Quit()
    {
        Application.Quit();
    }

    private float lastSignalChange;

    private void Update()
    {
        // debug controls
        if(Input.GetKeyDown(KeyCode.F1))
        {
            debugControlsUI.SetActive(!debugControlsUI.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            GameOver();
        }
        if(Input.GetKeyDown(KeyCode.F12))
        {
            ResetHighScores();
        }

        // next round control
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (waitingToStartRound)
            {
                StartNextRound();
            }
            else if (introRoutine != null)
            {
                StopCoroutine(introRoutine);
                humanInstance.GetComponentInChildren<Phone>().SubtractSignal(true);
                humanInstance.GetComponentInChildren<Phone>().SubtractSignal(true);
                humanInstance.GetComponentInChildren<Phone>().SubtractSignal(true);
                humanInstance.GetComponentInChildren<Phone>().rotateBar = true;
                humanInstance.GetComponentInChildren<Human>().cinematic = false;
                AnimalSpawner.SpawnAnimal();
                introRoutine = null;
                skipPrompt.SetActive(false);
				backgroundMusic.SetActive (true);
                humanInstance.GetComponentInChildren<Human>().ShowTooltip(true);
                StartCoroutine(TurnOffTooltip());
                startTime = Time.time;
            }
        }
        // if (waitingToStartRound && Input.GetKeyDown(KeyCode.Return))
        // {
        //     StartNextRound();
        // }

        if(score - lastSignalChange >= signalChangeFrequency)
        {
            lastSignalChange = score;
            ChangeSignalLocation();
        }
    }

    [SerializeField]
    private float signalChangeFrequency;

    [Space(12)]

    [SerializeField]
    private float minHorizontal;
    [SerializeField]
    private float horizontalVarianceMinimum;
    [SerializeField]
    private float horizontalVarianceMaximum;
    [SerializeField]
    private float minVertical;
    [SerializeField]
    private float verticalVarianceMinimum;
    [SerializeField]
    private float verticalVarianceMaximum;

    private void ChangeSignalLocation()
    {
        float horizontalVariance = Random.Range(horizontalVarianceMinimum,horizontalVarianceMaximum);
        float verticalVariance = Random.Range(verticalVarianceMinimum,verticalVarianceMaximum);        
        signal.transform.position += new Vector3(minHorizontal + horizontalVariance, minVertical + verticalVariance, 0);
        if(signal.transform.position.x > 5)
            signal.transform.position = new Vector3(5f,signal.transform.position.y, 0);
        if(signal.transform.position.x < -5)
            signal.transform.position = new Vector3(-5f, signal.transform.position.y, 0);
    }


    private void ResetHighScores()
    {
        PlayerPrefs.DeleteKey("Scores");
    }
}
