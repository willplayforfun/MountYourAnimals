using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool autoStart;

    public GameManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;

        startUI.SetActive(true);
        gameUI.SetActive(false);
#if UNITY_EDITOR
        debugControlsUI.SetActive(true);
#else
        debugControlsUI.SetActive(false);
#endif

        animalSpawner = GetComponent<AnimalSpawner>();

        if(autoStart)
        {
            StartPlay();
        }
    }

    [SerializeField]
    private GameObject startUI;
    [SerializeField]
    private GameObject debugControlsUI;
    [SerializeField]
    private GameObject gameUI;


    private AnimalSpawner animalSpawner;


    internal bool isPlaying { get; private set; }
    internal int roundNumber { get; private set; }

    public void StartPlay()
    {
        // TODO start the game
        // load in initial animal, start dialogue, etc.
        animalSpawner.SpawnAnimal();

        isPlaying = true;
        startUI.SetActive(false);
        gameUI.SetActive(true);
    }
    public void GameOver()
    {
        // TODO reset the game
        // i.e. delete animals, reset flags, respawn/move human back, reset score, music, etc.
        roundNumber = 0;

        isPlaying = false;
        startUI.SetActive(true);
        gameUI.SetActive(false);
    }
    public void Quit()
    {
        Application.Quit();
    }

    private void Update()
    {
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
