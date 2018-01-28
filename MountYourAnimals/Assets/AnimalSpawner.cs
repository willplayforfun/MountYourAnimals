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

    [Space(12)]

    private Animal nextAnimal;

    private List<Animal> allAnimals = new List<Animal>();

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
        newAnimal.Spawn(allAnimals.Count == 0);

        allAnimals.Add(newAnimal);

        SelectNextAnimal();

        GameManager.Instance.nextAnimalPanel.SetNextImage(nextAnimal.uiSprite, true);
    }

    public void ShowStack()
    {
        foreach(Animal a in allAnimals)
        {
            a.EnableCameraFocus();

            // let the player activate abilities on any of the animals
            a.SetMouseAbilityActivation(true);
        }
    }
    public void HideStack()
    {
        foreach (Animal a in allAnimals)
        {
            a.DisableCameraFocus();
            a.SetMouseAbilityActivation(false);
        }
    }

    public void ClearAllAnimals()
    {
        foreach(Animal a in allAnimals)
        {
            Destroy(a.gameObject);
        }
        allAnimals.Clear();
    }
}
