using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class Zoom : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private CinemachineCamera cam;

    [SerializeField] private int defaultFOV = 60;
    [SerializeField] private int zoomFOV = 20;

    private void Update()
    {
        if (playerInputHandler.ZoomTriggered)
        {
            cam.Lens.FieldOfView = zoomFOV;
        }
        else
        {
           cam.Lens.FieldOfView = defaultFOV;
        }
    }
}
