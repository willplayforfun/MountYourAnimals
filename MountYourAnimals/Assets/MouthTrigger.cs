using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject biteIndicator;

    private void Awake()
    {
        biteIndicator.SetActive(false);
    }

    internal Animal latestAnimal;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponentInParent<Animal>() != null)
        {
            latestAnimal = collision.GetComponentInParent<Animal>();
            biteIndicator.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.GetComponentInParent<Animal>() == latestAnimal)
        {
            latestAnimal = null;
            biteIndicator.SetActive(false);
        }
    }

    private void OnDisable()
    {
        biteIndicator.SetActive(false);
    }
}
