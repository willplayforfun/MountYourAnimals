using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPart : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] hitSounds;

    private AudioSource myAudioSource;

    private void Awake()
    {
        myAudioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayHitSound();
    }

    private void PlayHitSound()
    {
        if (hitSounds.Length > 0)
        {
            AudioClip clip = hitSounds[Random.Range(0, hitSounds.Length)];
            myAudioSource.PlayOneShot(clip);
        }
    }
}
