// Coloca este script dentro de una carpeta llamada "Editor" en tu proyecto,
// por ejemplo: Assets/Editor/FixARSetup.cs
// (Debe estar en una carpeta "Editor" o Unity dará error al compilar)

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR
public class FixARSetup
{
    // Ajusta esta ruta si tu Renderer Asset tiene otro nombre o ubicación
    private const string RendererAssetPath = "Assets/Settings/Mobile_Renderer.asset";

    [MenuItem("Tools/AR/Fix URP Renderer for AR Foundation")]
    public static void AddARBackgroundFeature()
    {
        UniversalRendererData rendererData = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(RendererAssetPath);

        if (rendererData == null)
        {
            Debug.LogError($"[FixARSetup] No se encontró el Renderer Asset en: {RendererAssetPath}. " +
                            $"Revisa el nombre/ruta real en Assets/Settings/ y ajústalo en el script.");
            return;
        }

        // Revisa si ya tiene la feature de AR Background
        bool alreadyHasFeature = false;
        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature != null && feature.GetType().Name.Contains("ARBackground"))
            {
                alreadyHasFeature = true;
                break;
            }
        }

        if (alreadyHasFeature)
        {
            Debug.Log("[FixARSetup] El Renderer ya tiene la AR Background Renderer Feature. Todo bien.");
            return;
        }

        Debug.LogWarning("[FixARSetup] No se encontró la AR Background Renderer Feature en el Renderer. " +
                          "Agrégala manualmente desde el Inspector del asset (botón '+' en Renderer Features), " +
                          "ya que Unity no expone una API pública estable para añadirla por código sin riesgo " +
                          "de romper el asset serializado.");

        // Selecciona el asset automáticamente para que solo tengas que darle "+" y elegir la feature
        Selection.activeObject = rendererData;
        EditorGUIUtility.PingObject(rendererData);
    }

    [MenuItem("Tools/AR/Check Android Build Settings")]
    public static void CheckAndroidSettings()
    {
        Debug.Log($"[FixARSetup] Minimum API Level actual: {PlayerSettings.Android.minSdkVersion}");
        Debug.Log($"[FixARSetup] Scripting Backend: {PlayerSettings.GetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android)}");
        Debug.Log($"[FixARSetup] Target Architectures: {PlayerSettings.Android.targetArchitectures}");

        if ((int)PlayerSettings.Android.minSdkVersion < 24)
        {
            Debug.LogWarning("[FixARSetup] Minimum API Level muy bajo para ARCore. Recomendado: API 24 o superior (29+ si usas Vulkan).");
        }
    }
}
#endif