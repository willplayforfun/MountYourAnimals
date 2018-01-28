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

    [SerializeField]
    private float width;

    private Vector3 startingPos;

    [SerializeField]
    private float popoutSpeed = 0.5f;

    private void Awake()
    {
        startingPos = this.transform.localPosition;
        Retract();
    }

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
        Debug.Log("Popping out panel");
        LeanTween.cancel(this.gameObject);
        LeanTween.moveLocal(this.gameObject, startingPos, popoutSpeed).setEaseOutQuad();
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
        Debug.Log("Retracting panel");
        LeanTween.cancel(this.gameObject);
        LeanTween.moveLocal(this.gameObject, startingPos + width  * Vector3.right, popoutSpeed).setEaseOutQuad();
    }
}
