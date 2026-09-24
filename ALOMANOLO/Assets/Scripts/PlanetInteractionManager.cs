using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

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

    [Header("Transición")]
    [SerializeField] private float fadeDuration = 0.6f;

    private PlanetInteraction currentPlanet;
    private GameObject currentInterior;

    private float nextCheckTime;

    private CanvasGroup fadeCanvas;

    private bool isTransitioning;

    private readonly List<GameObject> exteriorPlanets =
        new List<GameObject>();

    private void Start()
    {
        HideAllInteriors();

        if (enterPlanetButton != null)
            enterPlanetButton.SetActive(false);

        if (exitPlanetButton != null)
            exitPlanetButton.SetActive(false);

        CreateFadeCanvas();
    }

    public void SetPlanetSystem(GameObject system)
    {
        planetSystem = system;

        CacheExteriorPlanets();

        Debug.Log(
            "Sistema de planetas recibido. Planetas encontrados: " +
            exteriorPlanets.Count
        );
    }

    private void Update()
    {
        if (currentInterior != null)
            return;

        if (isTransitioning)
            return;

        if (Time.time < nextCheckTime)
            return;

        nextCheckTime =
            Time.time + checkInterval;

        FindClosestPlanet();
    }

    private void CacheExteriorPlanets()
    {
        exteriorPlanets.Clear();

        if (planetSystem == null)
            return;

        PlanetInteraction[] planets =
            planetSystem.GetComponentsInChildren<PlanetInteraction>(
                true
            );

        foreach (PlanetInteraction planet in planets)
        {
            Transform planetRoot =
                planet.transform;

            while (
                planetRoot.parent != null &&
                planetRoot.parent != planetSystem.transform)
            {
                planetRoot =
                    planetRoot.parent;
            }

            GameObject rootObject =
                planetRoot.gameObject;

            if (!exteriorPlanets.Contains(rootObject))
            {
                exteriorPlanets.Add(rootObject);
            }
        }
    }

    private void FindClosestPlanet()
    {
        if (planetSystem == null)
        {
            HideEnterButton();
            return;
        }

        PlanetInteraction[] planets =
            planetSystem.GetComponentsInChildren<PlanetInteraction>(
                true
            );

        PlanetInteraction closestPlanet = null;

        float closestDistance =
            Mathf.Infinity;

        Camera mainCamera =
            Camera.main;

        if (mainCamera == null)
        {
            HideEnterButton();
            return;
        }

        foreach (PlanetInteraction planet in planets)
        {
            if (!planet.gameObject.activeInHierarchy)
                continue;

            if (!planet.IsPlayerNear())
                continue;

            float distance =
                Vector3.Distance(
                    mainCamera.transform.position,
                    planet.transform.position
                );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlanet = planet;
            }
        }

        currentPlanet =
            closestPlanet;

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

        if (isTransitioning)
            return;

        StartCoroutine(
            EnterPlanetTransition()
        );
    }

    private IEnumerator EnterPlanetTransition()
    {
        isTransitioning = true;

        Camera mainCamera =
            Camera.main;

        if (mainCamera == null)
        {
            Debug.LogWarning(
                "No se encontró la Main Camera."
            );

            isTransitioning = false;
            yield break;
        }

        string planetName =
            currentPlanet.PlanetName.ToLower();

        GameObject selectedInterior =
            null;

        if (planetName == "venus")
        {
            selectedInterior =
                venusInterior;
        }
        else if (
            planetName == "marte" ||
            planetName == "mars")
        {
            selectedInterior =
                marsInterior;
        }
        else if (
            planetName == "júpiter" ||
            planetName == "jupiter")
        {
            selectedInterior =
                jupiterInterior;
        }
        else if (
            planetName == "saturno" ||
            planetName == "saturn")
        {
            selectedInterior =
                saturnInterior;
        }

        if (selectedInterior == null)
        {
            Debug.LogWarning(
                "No existe un interior configurado para: " +
                currentPlanet.PlanetName
            );

            isTransitioning = false;
            yield break;
        }

        HideEnterButton();

        yield return StartCoroutine(
            Fade(1f)
        );

        HideExteriorPlanets();

        HideAllInteriors();

        selectedInterior.transform.position =
            mainCamera.transform.position;

        selectedInterior.SetActive(true);

        currentInterior =
            selectedInterior;

        if (exitPlanetButton != null)
            exitPlanetButton.SetActive(true);

        yield return StartCoroutine(
            Fade(0f)
        );

        isTransitioning = false;

        Debug.Log(
            "Entrando al interior de " +
            currentPlanet.PlanetName
        );
    }

    public void ExitCurrentPlanet()
    {
        if (currentInterior == null)
            return;

        if (isTransitioning)
            return;

        StartCoroutine(
            ExitPlanetTransition()
        );
    }

    private IEnumerator ExitPlanetTransition()
    {
        isTransitioning = true;

        yield return StartCoroutine(
            Fade(1f)
        );

        currentInterior.SetActive(false);

        currentInterior =
            null;

        ShowExteriorPlanets();

        currentPlanet =
            null;

        HideEnterButton();

        if (exitPlanetButton != null)
            exitPlanetButton.SetActive(false);

        yield return StartCoroutine(
            Fade(0f)
        );

        isTransitioning = false;

        Debug.Log(
            "Saliendo del interior del planeta."
        );
    }

    private void HideExteriorPlanets()
    {
        if (exteriorPlanets.Count == 0)
        {
            CacheExteriorPlanets();
        }

        foreach (GameObject planet in exteriorPlanets)
        {
            if (planet != null)
                planet.SetActive(false);
        }

        Debug.Log(
            "Planetas exteriores ocultados: " +
            exteriorPlanets.Count
        );
    }

    private void ShowExteriorPlanets()
    {
        foreach (GameObject planet in exteriorPlanets)
        {
            if (planet != null)
                planet.SetActive(true);
        }

        Debug.Log(
            "Planetas exteriores mostrados: " +
            exteriorPlanets.Count
        );
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

    private void CreateFadeCanvas()
    {
        GameObject fadeObject =
            new GameObject("AR Fade");

        Canvas canvas =
            fadeObject.AddComponent<Canvas>();

        canvas.renderMode =
            RenderMode.ScreenSpaceOverlay;

        canvas.sortingOrder = 999;

        UnityEngine.UI.Image image =
            fadeObject.AddComponent<UnityEngine.UI.Image>();

        image.color =
            Color.black;

        RectTransform rect =
            fadeObject.GetComponent<RectTransform>();

        rect.anchorMin =
            Vector2.zero;

        rect.anchorMax =
            Vector2.one;

        rect.offsetMin =
            Vector2.zero;

        rect.offsetMax =
            Vector2.zero;

        fadeCanvas =
            fadeObject.AddComponent<CanvasGroup>();

        fadeCanvas.alpha =
            0f;

        fadeCanvas.blocksRaycasts =
            false;

        fadeCanvas.interactable =
            false;
    }

    private IEnumerator Fade(
        float targetAlpha)
    {
        if (fadeCanvas == null)
            yield break;

        float startAlpha =
            fadeCanvas.alpha;

        float elapsed =
            0f;

        while (
            elapsed < fadeDuration)
        {
            elapsed +=
                Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed /
                    fadeDuration
                );

            fadeCanvas.alpha =
                Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    t
                );

            yield return null;
        }

        fadeCanvas.alpha =
            targetAlpha;
    }

}