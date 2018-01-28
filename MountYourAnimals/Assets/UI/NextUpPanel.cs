using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextUpPanel : MonoBehaviour
{
    [SerializeField]
    private Image animalImage;

    [SerializeField]
    private float popoutTime = 3;

    public void SetNextImage(Sprite newSprite, bool hideAfterTime)
    {
        animalImage.sprite = newSprite;

        PopOut();
        if (hideAfterTime)
        {
            if(retractCoroutine != null)
            {
                StopCoroutine(retractCoroutine);
            }
            retractCoroutine = StartCoroutine(RetractRoutine());
        }
    }

    public void PopOut()
    {
        // TODO show
    }
    private Coroutine retractCoroutine;
    private IEnumerator RetractRoutine()
    {
        yield return new WaitForSeconds(popoutTime);
        Retract();
        retractCoroutine = null;
    }
    public void Retract()
    {
        // TODO hide
    }
}
