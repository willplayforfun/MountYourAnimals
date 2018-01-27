using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextUpPanel : MonoBehaviour
{
    [SerializeField]
    private Image animalImage;

    public void SetNextImage(Sprite newSprite)
    {
        animalImage.sprite = newSprite;
    }

    private void PopOut()
    {
        // TODO show
    }
    private void Retract()
    {
        // TODO hide
    }
}
