using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreachVision : MonoBehaviour
{

    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform ownerBreach;
    [SerializeField] private Transform otherBreach;

    [SerializeField] private Camera view;
    [SerializeField] private Material cameraMat;

    void Start()
    {
        if (view.targetTexture != null)
        {
            view.targetTexture.Release();
        }

        view.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        cameraMat.mainTexture = view.targetTexture;
        transform.position = ownerBreach.position;
    }

    void Update()
    {
        Vector3 playerBreachOffset = playerCamera.position - otherBreach.position;
        playerBreachOffset.y *= -1;
        transform.position = ownerBreach.position - playerBreachOffset;

        float breachAngularDiff = Quaternion.Angle(ownerBreach.rotation, otherBreach.rotation);
        Quaternion breachRotDiff = Quaternion.AngleAxis(breachAngularDiff, Vector3.up);
        Vector3 newCameraDir = breachRotDiff * -playerCamera.forward;
        newCameraDir.y *= -1;
        transform.rotation = Quaternion.LookRotation(newCameraDir, Vector3.up);
    }
}
