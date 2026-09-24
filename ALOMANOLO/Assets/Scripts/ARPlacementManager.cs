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

    private void Start()
    {
        ShowPlaneVisuals();
    }

    private void Update()
    {
        if (spawnedPlanetSystem != null)
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

            HidePlaneVisuals();
        }
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
            {
                visualizer.enabled = false;
            }

            MeshRenderer meshRenderer =
                plane.GetComponent<MeshRenderer>();

            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;
            }

            LineRenderer lineRenderer =
                plane.GetComponent<LineRenderer>();

            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
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
            {
                visualizer.enabled = true;
            }

            MeshRenderer meshRenderer =
                plane.GetComponent<MeshRenderer>();

            if (meshRenderer != null)
            {
                meshRenderer.enabled = true;
            }

            LineRenderer lineRenderer =
                plane.GetComponent<LineRenderer>();

            if (lineRenderer != null)
            {
                lineRenderer.enabled = true;
            }
        }
    }

}