using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlanetInteractionManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject enterPlanetButton;
    [SerializeField] private TMP_Text buttonText;

    [Header("Configuración")]
    [SerializeField] private float checkInterval = 0.1f;

    private PlanetInteraction currentPlanet;
    private float nextCheckTime;

    private void Start()
    {
        // El botón debe comenzar oculto.
        if (enterPlanetButton != null)
        {
            enterPlanetButton.SetActive(false);
        }
    }

    private void Update()
    {
        // No necesitamos comprobar cada frame.
        if (Time.time < nextCheckTime)
            return;

        nextCheckTime = Time.time + checkInterval;

        FindClosestPlanet();
    }

    private void FindClosestPlanet()
    {
        PlanetInteraction[] planets =
            FindObjectsByType<PlanetInteraction>(
                FindObjectsSortMode.None
            );

        PlanetInteraction closestPlanet = null;
        float closestDistance = Mathf.Infinity;

        foreach (PlanetInteraction planet in planets)
        {
            if (!planet.IsPlayerNear())
                continue;

            Camera mainCamera = Camera.main;

            if (mainCamera == null)
                continue;

            float distance = Vector3.Distance(
                mainCamera.transform.position,
                planet.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlanet = planet;
            }
        }

        currentPlanet = closestPlanet;

        UpdateButton();
    }

    private void UpdateButton()
    {
        if (enterPlanetButton == null)
            return;

        // No hay ningún planeta cerca.
        if (currentPlanet == null)
        {
            enterPlanetButton.SetActive(false);
            return;
        }

        // Hay un planeta cerca.
        enterPlanetButton.SetActive(true);

        if (buttonText != null)
        {
            buttonText.text =
                "ENTRAR A " + currentPlanet.PlanetName.ToUpper();
        }
    }

    public void EnterCurrentPlanet()
    {
        if (currentPlanet == null)
            return;

        Debug.Log(
            "Entrando al planeta: " +
            currentPlanet.PlanetName
        );
    }
}