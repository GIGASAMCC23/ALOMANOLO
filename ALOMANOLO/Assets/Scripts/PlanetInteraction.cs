using UnityEngine;

public class PlanetInteraction : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string planetName = "Planeta";
    [SerializeField] private float interactionDistance = 1.5f;

    private Transform playerCamera;

    public string PlanetName => planetName;
    public float InteractionDistance => interactionDistance;

    private void Start()
    {
        if (Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }
    }

    public bool IsPlayerNear()
    {
        if (playerCamera == null && Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }

        if (playerCamera == null)
            return false;

        float distance = Vector3.Distance(
            playerCamera.position,
            transform.position
        );

        return distance <= interactionDistance;
    }
}
