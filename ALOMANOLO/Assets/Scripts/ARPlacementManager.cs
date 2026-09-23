using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacementManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private GameObject planetSystemPrefab;

    private GameObject spawnedPlanetSystem;

    private readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Update()
    {
        // Si los planetas ya fueron colocados, no hacemos nada más.
        if (spawnedPlanetSystem != null)
            return;

        // Verificamos que exista una pantalla táctil.
        if (Touchscreen.current == null)
            return;

        // Solo actuamos cuando el usuario toca la pantalla.
        if (!Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return;

        // Obtenemos la posición del dedo en la pantalla.
        Vector2 touchPosition =
            Touchscreen.current.primaryTouch.position.ReadValue();

        // Lanzamos un Raycast hacia los planos detectados.
        if (raycastManager.Raycast(
            touchPosition,
            hits,
            TrackableType.PlaneWithinPolygon))
        {
            // Obtenemos la posición encontrada en el mundo AR.
            Pose hitPose = hits[0].pose;

            // Creamos nuestro sistema de planetas en esa posición.
            spawnedPlanetSystem = Instantiate(
                planetSystemPrefab,
                hitPose.position,
                hitPose.rotation
            );
        }
    }
}