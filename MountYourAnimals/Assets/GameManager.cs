using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    GameObject signal;
    private void Awake()
    {
        Instance = this;

        // TODO intro sequence?

        startUI.SetActive(true);
        gameUI.SetActive(false);
        freezePrompt.SetActive(false);
        grabPrompt.SetActive(false);
        movePrompt.SetActive(false);
        roundPrompt.SetActive(false);
        abilityPrompt.SetActive(false);
#if UNITY_EDITOR
        debugControlsUI.SetActive(debugControlsStartShowing);
#else
        debugControlsUI.SetActive(false);
#endif

        AnimalSpawner = GetComponent<AnimalSpawner>();

        // create a human initially
        SpawnHuman();

        signal = GameObject.Find("Signal1");
    }
    private void Start()
    {
        if (autoStart)
        {
            StartPlay();
        }
    }
    // ----------------

    [SerializeField]
    private GameObject startUI;
    [SerializeField]
    private GameObject debugControlsUI;
    [SerializeField]
    private GameObject gameUI;

    [SerializeField]
    internal GameObject freezePrompt;
    [SerializeField]
    internal GameObject grabPrompt;
    [SerializeField]
    internal GameObject movePrompt;
    [SerializeField]
    internal GameObject roundPrompt;
    [SerializeField]
    internal GameObject abilityPrompt;
    [SerializeField]
    internal NextUpPanel nextAnimalPanel;
    [SerializeField]
    internal PatienceBar patienceRef;

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

    // ----------------

    // called by UI to start the game
    public void StartPlay()
    {
        // TODO start the game
        // load in initial animal, start dialogue, etc.
        AnimalSpawner.SpawnAnimal();

        isPlaying = true;
        startUI.SetActive(false);
        gameUI.SetActive(true);
        // TODO fade out UI instead of immediately hiding, animate in game UI
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
        // TODO reset the game
        // i.e. reset flags, reset score, music, etc.

        roundNumber = 0;
        // delete human and create a new one
        SpawnHuman();
        // delete animals
        AnimalSpawner.ClearAllAnimals();


        isPlaying = false;
        startUI.SetActive(true);
        gameUI.SetActive(false);
        // TODO some sort of transition and game over screen
    }

    // called by UI to quit the application
    public void Quit()
    {
        Application.Quit();
    }

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

        // next round control
        if (waitingToStartRound && Input.GetKeyDown(KeyCode.Space))
        {
            StartNextRound();
        }

        if(score >= 3)
        {
            score = 0;
            ChangeSignalLocation();
        }
    }

    public float minHorizontal, horizontalVarianceMinimum, horizontalVarianceMaximum, minVertical, verticalVarianceMinimum, verticalVarianceMaximum;
    void ChangeSignalLocation()
    {
        float horizontalVariance = Random.Range(horizontalVarianceMinimum,horizontalVarianceMaximum);
        float verticalVariance = Random.Range(verticalVarianceMinimum,verticalVarianceMaximum);        
        signal.transform.position += new Vector3(minHorizontal + horizontalVariance, minVertical + verticalVariance, 0);
        if(signal.transform.position.x > 5)
            signal.transform.position = new Vector3(5f,signal.transform.position.y, 0);
        if(signal.transform.position.x < -5)
            signal.transform.position = new Vector3(-5f, signal.transform.position.y, 0);
    }

    [SerializeField]
    internal float score;
}
