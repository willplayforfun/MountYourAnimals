using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject mainBackground;
    [SerializeField]
    private float mainBackgroundHeight;
    private float mainBackgroundStartY;

    [SerializeField]
    private GameObject starBackgroundPrefab;
    [SerializeField]
    private float starBackgroundHeight;


    private struct AmbienceZone
    {
        public float minY;
        public float maxY;
        public AudioClip loop;
    }
    [Space(12)]
    [SerializeField]
    private AmbienceZone[] ambienceZones;

    private void Awake()
    {
        mainBackgroundStartY = mainBackground.transform.position.y;
    }

    private void Update()
    {
        GameObject cameraObj = Camera.main.gameObject;
    }
}
