using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gib : MonoBehaviour
{
    [SerializeField]
    private float lifeTime = 3;
    [SerializeField]
    private float fadeTime = 1;

    private void Start()
    {
        StartCoroutine(FadeRoutine());
    }
    private IEnumerator FadeRoutine()
    {
        yield return new WaitForSeconds(lifeTime);
        LeanTween.alpha(this.gameObject, 0, fadeTime);
        yield return new WaitForSeconds(fadeTime);
        Destroy(this.gameObject);
    }
}
