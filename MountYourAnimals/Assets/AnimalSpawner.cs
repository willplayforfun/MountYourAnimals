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

    // called by the GameManager initially to spawn the first Animal
    public void SpawnAnimal()
    {
        bool normalAnimal = true;

        Animal newAnimal = null;
        if(normalAnimal)
        {
            newAnimal = Instantiate(normalAnimals[Random.Range(0, normalAnimals.Length)]);
        }
        else
        {
            newAnimal = Instantiate(exoticAnimals[Random.Range(0, exoticAnimals.Length)]);
        }

        newAnimal.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        // TODO flip the animal if it is coming in the wrong way

        newAnimal.Spawn();

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
