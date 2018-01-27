using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    public event System.Action onSpawnAnimal;

    [SerializeField]
    private Animal[] normalAnimals;
    [SerializeField]
    private Animal[] exoticAnimals;

    [SerializeField]
    private Transform[] spawnPoints;

    [Space(12)]

    [SerializeField]
    private NextUpPanel nextAnimalPanel;
    private Animal nextAnimal;

    private void SelectNextAnimal()
    {
        bool normalAnimal = true;

        if (normalAnimal)
        {
            nextAnimal = normalAnimals[Random.Range(0, normalAnimals.Length)];
        }
        else
        {
            nextAnimal = exoticAnimals[Random.Range(0, exoticAnimals.Length)];
        }
    }

    private void Awake()
    {
        SelectNextAnimal();
    }

    // called by the GameManager initially to spawn the first Animal
    public void SpawnAnimal()
    {
        Animal newAnimal = Instantiate(nextAnimal);
        newAnimal.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        // TODO flip the animal if it is coming in the wrong way
        newAnimal.Spawn();

        SelectNextAnimal();

        nextAnimalPanel.SetNextImage(nextAnimal.uiSprite);

        onSpawnAnimal.Invoke();
    }

    // called by the actively controlled Animal when the player freezes it
    public void CurrentAnimalFrozen()
    {
        // TODO show the stack and let the player activate abilities on any of the animals

        // for now, just spawn the next animal
        SpawnAnimal();
    }
}
