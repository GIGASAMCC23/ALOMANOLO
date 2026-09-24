using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacementManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private GameObject planetSystemPrefab;

    private GameObject spawnedPlanetSystem;

    private readonly List<ARRaycastHit> hits =
        new List<ARRaycastHit>();

    private bool planeDetectionFinished = false;

    private void Start()
    {
        StartPlaneDetection();
    }

    private void Update()
    {
        if (planeDetectionFinished)
            return;

        if (Touchscreen.current == null)
            return;

        if (!Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return;

        Vector2 touchPosition =
            Touchscreen.current.primaryTouch.position.ReadValue();

        if (raycastManager.Raycast(
            touchPosition,
            hits,
            TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            spawnedPlanetSystem = Instantiate(
                planetSystemPrefab,
                hitPose.position,
                hitPose.rotation
            );

            PlanetInteractionManager interactionManager =
                FindFirstObjectByType<PlanetInteractionManager>();

            if (interactionManager != null)
            {
                interactionManager.SetPlanetSystem(
                    spawnedPlanetSystem
                );
            }

            FinishPlaneDetection();
        }
    }

    private void StartPlaneDetection()
    {
        planeDetectionFinished = false;

        if (planeManager != null)
            planeManager.enabled = true;

        if (raycastManager != null)
            raycastManager.enabled = true;

        ShowPlaneVisuals();
    }

    private void FinishPlaneDetection()
    {
        planeDetectionFinished = true;

        HidePlaneVisuals();

        if (planeManager != null)
            planeManager.enabled = false;

        if (raycastManager != null)
            raycastManager.enabled = false;

        Debug.Log("Detección de plano finalizada.");
    }

    private void HidePlaneVisuals()
    {
        if (planeManager == null)
            return;

        foreach (ARPlane plane in planeManager.trackables)
        {
            ARPlaneMeshVisualizer visualizer =
                plane.GetComponent<ARPlaneMeshVisualizer>();

            if (visualizer != null)
                visualizer.enabled = false;

            MeshRenderer meshRenderer =
                plane.GetComponent<MeshRenderer>();

            if (meshRenderer != null)
                meshRenderer.enabled = false;

            LineRenderer lineRenderer =
                plane.GetComponent<LineRenderer>();

            if (lineRenderer != null)
                lineRenderer.enabled = false;
        }
    }

    private void ShowPlaneVisuals()
    {
        if (planeManager == null)
            return;

        foreach (ARPlane plane in planeManager.trackables)
        {
            ARPlaneMeshVisualizer visualizer =
                plane.GetComponent<ARPlaneMeshVisualizer>();

            if (visualizer != null)
                visualizer.enabled = true;

            MeshRenderer meshRenderer =
                plane.GetComponent<MeshRenderer>();

            if (meshRenderer != null)
                meshRenderer.enabled = true;

            LineRenderer lineRenderer =
                plane.GetComponent<LineRenderer>();

            if (lineRenderer != null)
                lineRenderer.enabled = true;
        }
    }

}