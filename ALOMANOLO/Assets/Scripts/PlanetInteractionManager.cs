using UnityEngine;
using TMPro;

public class PlanetInteractionManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject enterPlanetButton;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private GameObject exitPlanetButton;

    [Header("Configuración")]
    [SerializeField] private float checkInterval = 0.1f;

    [Header("Sistema de planetas")]
    [SerializeField] private GameObject planetSystem;

    [Header("Interiores 360")]
    [SerializeField] private GameObject venusInterior;
    [SerializeField] private GameObject marsInterior;
    [SerializeField] private GameObject jupiterInterior;
    [SerializeField] private GameObject saturnInterior;

    private PlanetInteraction currentPlanet;
    private float nextCheckTime;

    private GameObject currentInterior;

    private void Start()
    {
        HideAllInteriors();

        if (enterPlanetButton != null)
            enterPlanetButton.SetActive(false);

        if (exitPlanetButton != null)
            exitPlanetButton.SetActive(false);
    }

    private void Update()
    {
        // Si estamos dentro de un planeta, no buscamos
        // planetas exteriores.
        if (currentInterior != null)
            return;

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

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            HideEnterButton();
            return;
        }

        foreach (PlanetInteraction planet in planets)
        {
            if (!planet.IsPlayerNear())
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

        UpdateEnterButton();
    }

    private void UpdateEnterButton()
    {
        if (enterPlanetButton == null)
            return;

        if (currentPlanet == null)
        {
            HideEnterButton();
            return;
        }

        enterPlanetButton.SetActive(true);

        if (buttonText != null)
        {
            buttonText.text =
                "ENTRAR A " +
                currentPlanet.PlanetName.ToUpper();
        }
    }

    private void HideEnterButton()
    {
        if (enterPlanetButton != null)
            enterPlanetButton.SetActive(false);
    }

    public void EnterCurrentPlanet()
    {
        if (currentPlanet == null)
            return;

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogWarning("No se encontró la Main Camera.");
            return;
        }

        string planetName =
            currentPlanet.PlanetName.ToLower();

        GameObject selectedInterior = null;

        if (planetName == "venus")
        {
            selectedInterior = venusInterior;
        }
        else if (planetName == "marte" || planetName == "mars")
        {
            selectedInterior = marsInterior;
        }
        else if (planetName == "júpiter" ||
                 planetName == "jupiter")
        {
            selectedInterior = jupiterInterior;
        }
        else if (planetName == "saturno" ||
                 planetName == "saturn")
        {
            selectedInterior = saturnInterior;
        }

        if (selectedInterior == null)
        {
            Debug.LogWarning(
                "No existe un interior configurado para: " +
                currentPlanet.PlanetName
            );

            return;
        }

        // Ocultamos el sistema exterior.
        if (planetSystem != null)
        {
            planetSystem.SetActive(false);
        }

        // Ocultamos cualquier interior anterior.
        HideAllInteriors();

        // Colocamos el centro del interior
        // exactamente donde está la cámara.
        selectedInterior.transform.position =
            mainCamera.transform.position;

        // Activamos el interior 360.
        selectedInterior.SetActive(true);

        currentInterior = selectedInterior;

        // Cambiamos los botones.
        HideEnterButton();

        if (exitPlanetButton != null)
            exitPlanetButton.SetActive(true);

        Debug.Log(
            "Entrando al interior de " +
            currentPlanet.PlanetName
        );
    }

    public void ExitCurrentPlanet()
    {
        if (currentInterior == null)
            return;

        // Apagamos el interior 360.
        currentInterior.SetActive(false);

        currentInterior = null;

        // Volvemos a mostrar los planetas.
        if (planetSystem != null)
        {
            planetSystem.SetActive(true);
        }

        // Ocultamos el botón de salir.
        if (exitPlanetButton != null)
            exitPlanetButton.SetActive(false);

        Debug.Log("Saliendo del interior del planeta.");
    }

    private void HideAllInteriors()
    {
        if (venusInterior != null)
            venusInterior.SetActive(false);

        if (marsInterior != null)
            marsInterior.SetActive(false);

        if (jupiterInterior != null)
            jupiterInterior.SetActive(false);

        if (saturnInterior != null)
            saturnInterior.SetActive(false);

        currentInterior = null;
    }
}