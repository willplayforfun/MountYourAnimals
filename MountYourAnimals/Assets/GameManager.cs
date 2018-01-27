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

    private void Awake()
    {
        Instance = this;

        // TODO intro sequence?

        startUI.SetActive(true);
        gameUI.SetActive(false);
#if UNITY_EDITOR
        debugControlsUI.SetActive(debugControlsStartShowing);
#else
        debugControlsUI.SetActive(false);
#endif

        AnimalSpawner = GetComponent<AnimalSpawner>();
        AnimalSpawner.onSpawnAnimal += () => { roundNumber += 1; };

        // create a human initially
        SpawnHuman();

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
    
    // ----------------


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
    public void GameOver()
    {
        // TODO reset the game
        // i.e. delete animals, reset flags, respawn/move human back, reset score, music, etc.
        roundNumber = 0;
        SpawnHuman();

        isPlaying = false;
        startUI.SetActive(true);
        gameUI.SetActive(false);
        // TODO some sort of transition and game over screen
    }
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
    }
}
