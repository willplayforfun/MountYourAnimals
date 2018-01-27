using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    [SerializeField]
    private Animal[] normalAnimals;
    [SerializeField]
    private Animal[] exoticAnimals;

    [SerializeField]
    private Transform[] spawnPoints;

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
    }
}
