#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePositionScanner : EditorWindow
{
    bool exportActive = true;
    bool exportInactive = true;
    bool exportComponents = true;
    bool exportLocalTransform = true;
    bool exportWorldTransform = true;
    bool exportLayer = true;
    bool exportTag = true;
    
    Vector2 scrollPos;
    
    [MenuItem("Tools/Scanner Position Scène")]
    public static void ShowWindow()
    {
        GetWindow<ScenePositionScanner>("Scanner Position");
    }
    
    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        GUILayout.Label("Scanner Position de la Scène", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        Scene activeScene = SceneManager.GetActiveScene();
        EditorGUILayout.HelpBox($"Scène active: {activeScene.name}\nPath: {activeScene.path}", MessageType.Info);
        
        GUILayout.Space(10);
        
        // Options
        GUILayout.Label("Options d'export:", EditorStyles.boldLabel);
        exportActive = EditorGUILayout.Toggle("  Objets actifs", exportActive);
        exportInactive = EditorGUILayout.Toggle("  Objets inactifs", exportInactive);
        exportComponents = EditorGUILayout.Toggle("  Lister components", exportComponents);
        exportLocalTransform = EditorGUILayout.Toggle("  Transform local", exportLocalTransform);
        exportWorldTransform = EditorGUILayout.Toggle("  Transform world", exportWorldTransform);
        exportLayer = EditorGUILayout.Toggle("  Layer", exportLayer);
        exportTag = EditorGUILayout.Toggle("  Tag", exportTag);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("📍 EXPORTER POSITIONS", GUILayout.Height(40)))
        {
            ExportSceneHierarchy();
        }
        
        EditorGUILayout.EndScrollView();
    }
    
    void ExportSceneHierarchy()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        string defaultName = activeScene.name + "_positions.txt";
        string path = EditorUtility.SaveFilePanel("Enregistrer positions", "", defaultName, "txt");
        
        if (string.IsNullOrEmpty(path)) return;
        
        int totalObjects = 0;
        int activeObjects = 0;
        int inactiveObjects = 0;
        
        using (StreamWriter writer = new StreamWriter(path))
        {
            // Header
            writer.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
            writer.WriteLine("║                    SCAN POSITION HIÉRARCHIE SCÈNE                          ║");
            writer.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
            writer.WriteLine();
            writer.WriteLine($"Scène: {activeScene.name}");
            writer.WriteLine($"Path: {activeScene.path}");
            writer.WriteLine($"Date: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            writer.WriteLine();
            writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
            writer.WriteLine();
            
            // Scan tous les root GameObjects
            GameObject[] rootObjects = activeScene.GetRootGameObjects();
            
            foreach (GameObject obj in rootObjects)
            {
                ScanRecursive(obj, writer, 0, ref totalObjects, ref activeObjects, ref inactiveObjects);
            }
            
            // Statistiques
            writer.WriteLine();
            writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
            writer.WriteLine("📊 STATISTIQUES");
            writer.WriteLine("═══════════════════════════════════════════════════════════════════════════════");
            writer.WriteLine($"Total objets: {totalObjects}");
            writer.WriteLine($"  • Actifs: {activeObjects}");
            writer.WriteLine($"  • Inactifs: {inactiveObjects}");
        }
        
        EditorUtility.DisplayDialog("Export terminé", 
            $"Positions exportées!\n\n{path}\n\nTotal: {totalObjects} objets", 
            "OK");
        
        // Ouvre le fichier
        System.Diagnostics.Process.Start(path);
    }
    
    void ScanRecursive(GameObject obj, StreamWriter writer, int depth, ref int total, ref int active, ref int inactive)
    {
        // Filtre selon options
        if (!exportActive && obj.activeSelf) return;
        if (!exportInactive && !obj.activeSelf) return;
        
        total++;
        if (obj.activeSelf) active++;
        else inactive++;
        
        string indent = new string(' ', depth * 2);
        Transform t = obj.transform;
        
        // Header objet
        string statusIcon = obj.activeSelf ? "✓" : "✗";
        writer.WriteLine($"{indent}┌─ {statusIcon} {obj.name}");
        
        // Layer et Tag
        if (exportLayer || exportTag)
        {
            if (exportLayer)
                writer.WriteLine($"{indent}│  Layer: {LayerMask.LayerToName(obj.layer)} ({obj.layer})");
            if (exportTag)
                writer.WriteLine($"{indent}│  Tag: {obj.tag}");
            writer.WriteLine($"{indent}│");
        }
        
        // Transform LOCAL
        if (exportLocalTransform)
        {
            writer.WriteLine($"{indent}├─ TRANSFORM LOCAL");
            writer.WriteLine($"{indent}│  Position: ({t.localPosition.x:F3}, {t.localPosition.y:F3}, {t.localPosition.z:F3})");
            
            // Rotation (Quaternion + Euler)
            Vector3 euler = t.localEulerAngles;
            writer.WriteLine($"{indent}│  Rotation:");
            writer.WriteLine($"{indent}│    Euler: ({euler.x:F2}°, {euler.y:F2}°, {euler.z:F2}°)");
            writer.WriteLine($"{indent}│    Quaternion: ({t.localRotation.x:F3}, {t.localRotation.y:F3}, {t.localRotation.z:F3}, {t.localRotation.w:F3})");
            
            writer.WriteLine($"{indent}│  Scale: ({t.localScale.x:F3}, {t.localScale.y:F3}, {t.localScale.z:F3})");
            writer.WriteLine($"{indent}│");
        }
        
        // Transform WORLD
        if (exportWorldTransform)
        {
            writer.WriteLine($"{indent}├─ TRANSFORM WORLD");
            writer.WriteLine($"{indent}│  Position: ({t.position.x:F3}, {t.position.y:F3}, {t.position.z:F3})");
            
            Vector3 worldEuler = t.eulerAngles;
            writer.WriteLine($"{indent}│  Rotation:");
            writer.WriteLine($"{indent}│    Euler: ({worldEuler.x:F2}°, {worldEuler.y:F2}°, {worldEuler.z:F2}°)");
            writer.WriteLine($"{indent}│    Quaternion: ({t.rotation.x:F3}, {t.rotation.y:F3}, {t.rotation.z:F3}, {t.rotation.w:F3})");
            
            writer.WriteLine($"{indent}│  Lossy Scale: ({t.lossyScale.x:F3}, {t.lossyScale.y:F3}, {t.lossyScale.z:F3})");
            writer.WriteLine($"{indent}│");
        }
        
        // Components
        if (exportComponents)
        {
            Component[] components = obj.GetComponents<Component>();
            writer.WriteLine($"{indent}├─ COMPONENTS ({components.Length})");
            
            foreach (Component comp in components)
            {
                if (comp == null) continue;
                
                string compName = comp.GetType().Name;
                writer.Write($"{indent}│  • {compName}");
                
                // Infos spéciales selon type
                if (comp is Renderer renderer)
                {
                    writer.Write($" (Enabled: {renderer.enabled})");
                }
                else if (comp is Collider collider)
                {
                    writer.Write($" (Enabled: {collider.enabled})");
                }
                else if (comp is MonoBehaviour behaviour)
                {
                    writer.Write($" (Enabled: {behaviour.enabled})");
                }
                
                writer.WriteLine();
            }
            writer.WriteLine($"{indent}│");
        }
        
        // Enfants
        int childCount = t.childCount;
        if (childCount > 0)
        {
            writer.WriteLine($"{indent}└─ ENFANTS ({childCount}):");
            writer.WriteLine();
            
            foreach (Transform child in t)
            {
                ScanRecursive(child.gameObject, writer, depth + 1, ref total, ref active, ref inactive);
            }
        }
        else
        {
            writer.WriteLine($"{indent}└─ (aucun enfant)");
        }
        
        writer.WriteLine();
    }
}
#endif