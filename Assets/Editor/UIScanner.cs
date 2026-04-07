#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;

public class UIScanner : EditorWindow
{
    Vector2 scrollPos;
    bool includeInactive = true;
    bool scanScripts = true;
    
    [MenuItem("Tools/UI Scene Scanner")]
    public static void ShowWindow()
    {
        GetWindow<UIScanner>("UI Scanner");
    }

    // Helper pour simplifier les appels FindObjectsByType
    T[] FindAll<T>() where T : Object
    {
        return FindObjectsByType<T>(
            includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );
    }
    
    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        GUILayout.Label("UI Scene Scanner", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Scanne toute l'UI de la scène et génère un rapport détaillé", MessageType.Info);
        GUILayout.Space(10);
        
        includeInactive = EditorGUILayout.Toggle("Inclure objets inactifs", includeInactive);
        scanScripts = EditorGUILayout.Toggle("Scanner les scripts UI", scanScripts);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("🔍 SCANNER LA SCÈNE", GUILayout.Height(40)))
        {
            ScanScene();
        }
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("📂 Ouvrir dossier rapports"))
        {
            string path = Path.Combine(Application.dataPath, "../UIReports");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            System.Diagnostics.Process.Start(path);
        }
        
        EditorGUILayout.EndScrollView();
    }
    
    void ScanScene()
    {
        string reportsDir = Path.Combine(Application.dataPath, "../UIReports");
        if (!Directory.Exists(reportsDir))
        {
            Directory.CreateDirectory(reportsDir);
        }
        
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filename = $"UI_Scan_{timestamp}.txt";
        string filepath = Path.Combine(reportsDir, filename);
        
        using (StreamWriter writer = new StreamWriter(filepath))
        {
            WriteHeader(writer);
            
            Canvas[] allCanvas = FindAll<Canvas>();
            writer.WriteLine($"╔══════════════════════════════════════════════════════════════════════════╗");
            writer.WriteLine($"║  CANVAS TROUVÉS: {allCanvas.Length}");
            writer.WriteLine($"╚══════════════════════════════════════════════════════════════════════════╝");
            writer.WriteLine();
            
            foreach (Canvas canvas in allCanvas)
            {
                ScanCanvas(canvas, writer);
                writer.WriteLine();
            }
            
            writer.WriteLine();
            writer.WriteLine($"╔══════════════════════════════════════════════════════════════════════════╗");
            writer.WriteLine($"║  TOUS LES TEXTES (UI Text + TextMeshPro)");
            writer.WriteLine($"╚══════════════════════════════════════════════════════════════════════════╝");
            writer.WriteLine();
            ScanAllTexts(writer);
            
            writer.WriteLine();
            writer.WriteLine($"╔══════════════════════════════════════════════════════════════════════════╗");
            writer.WriteLine($"║  TOUS LES BOUTONS");
            writer.WriteLine($"╚══════════════════════════════════════════════════════════════════════════╝");
            writer.WriteLine();
            ScanAllButtons(writer);
            
            writer.WriteLine();
            writer.WriteLine($"╔══════════════════════════════════════════════════════════════════════════╗");
            writer.WriteLine($"║  TOUS LES SLIDERS");
            writer.WriteLine($"╚══════════════════════════════════════════════════════════════════════════╝");
            writer.WriteLine();
            ScanAllSliders(writer);
            
            if (scanScripts)
            {
                writer.WriteLine();
                writer.WriteLine($"╔══════════════════════════════════════════════════════════════════════════╗");
                writer.WriteLine($"║  SCRIPTS UI ET LEURS RÉFÉRENCES");
                writer.WriteLine($"╚══════════════════════════════════════════════════════════════════════════╝");
                writer.WriteLine();
                ScanUIScripts(writer);
            }
            
            writer.WriteLine();
            writer.WriteLine($"╔══════════════════════════════════════════════════════════════════════════╗");
            writer.WriteLine($"║  RÉSUMÉ STATISTIQUES");
            writer.WriteLine($"╚══════════════════════════════════════════════════════════════════════════╝");
            writer.WriteLine();
            WriteStatistics(writer);
            
            writer.WriteLine();
            writer.WriteLine("════════════════════════════════════════════════════════════════════════════");
            writer.WriteLine($"FIN DU SCAN - {System.DateTime.Now}");
            writer.WriteLine("════════════════════════════════════════════════════════════════════════════");
        }
        
        EditorUtility.DisplayDialog("Succès", $"Rapport UI généré avec succès!\n\n{filepath}", "OK");
        EditorUtility.RevealInFinder(filepath);
    }
    
    void WriteHeader(StreamWriter writer)
    {
        writer.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
        writer.WriteLine("║                       RAPPORT COMPLET UI - SCÈNE UNITY                     ║");
        writer.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
        writer.WriteLine();
        writer.WriteLine($"Scène: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        writer.WriteLine($"Date: {System.DateTime.Now}");
        writer.WriteLine($"Inclure inactifs: {includeInactive}");
        writer.WriteLine();
        writer.WriteLine("════════════════════════════════════════════════════════════════════════════");
        writer.WriteLine();
    }
    
    void ScanCanvas(Canvas canvas, StreamWriter writer)
    {
        writer.WriteLine("┌───────────────────────────────────────────────────────────────────────────┐");
        writer.WriteLine($"│ CANVAS: {canvas.gameObject.name}");
        writer.WriteLine($"│ Actif: {canvas.gameObject.activeSelf}");
        writer.WriteLine("└───────────────────────────────────────────────────────────────────────────┘");
        writer.WriteLine();
        
        writer.WriteLine("  [CANVAS]");
        writer.WriteLine($"    • Render Mode: {canvas.renderMode}");
        writer.WriteLine($"    • Sort Order: {canvas.sortingOrder}");
        writer.WriteLine($"    • Override Sorting: {canvas.overrideSorting}");
        
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
            writer.WriteLine($"    • World Camera: {(canvas.worldCamera != null ? canvas.worldCamera.name : "NULL")}");
        
        if (canvas.renderMode == RenderMode.WorldSpace)
            writer.WriteLine($"    • Sorting Layer: {canvas.sortingLayerName}");
        
        writer.WriteLine();
        
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            writer.WriteLine("  [CANVAS SCALER]");
            writer.WriteLine($"    • UI Scale Mode: {scaler.uiScaleMode}");
            
            if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                writer.WriteLine($"    • Reference Resolution: {scaler.referenceResolution}");
                writer.WriteLine($"    • Screen Match Mode: {scaler.screenMatchMode}");
                writer.WriteLine($"    • Match: {scaler.matchWidthOrHeight}");
            }
            else if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ConstantPixelSize)
            {
                writer.WriteLine($"    • Scale Factor: {scaler.scaleFactor}");
            }
            
            writer.WriteLine($"    • Reference Pixels Per Unit: {scaler.referencePixelsPerUnit}");
            writer.WriteLine();
        }
        
        GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
        if (raycaster != null)
        {
            writer.WriteLine("  [GRAPHIC RAYCASTER]");
            writer.WriteLine($"    • Ignore Reversed Graphics: {raycaster.ignoreReversedGraphics}");
            writer.WriteLine($"    • Blocking Objects: {raycaster.blockingObjects}");
            writer.WriteLine();
        }
        
        writer.WriteLine("  [PANELS IMPORTANTS]");
        Transform canvasTransform = canvas.transform;
        int panelCount = 0;
        
        foreach (Transform child in canvasTransform)
        {
            if (IsPanel(child.gameObject))
            {
                panelCount++;
                ScanPanel(child.gameObject, writer, "    ");
            }
        }
        
        if (panelCount == 0)
            writer.WriteLine("    • Aucun panel direct trouvé");
        
        writer.WriteLine();
    }
    
    bool IsPanel(GameObject obj)
    {
        string name = obj.name.ToLower();
        return name.Contains("panel") || 
               name.Contains("hud") || 
               name.Contains("menu") ||
               name.Contains("popup") ||
               name.Contains("dialog") ||
               name.Contains("window") ||
               name.Contains("victoire") ||
               name.Contains("defaite") ||
               name.Contains("pause") ||
               name.Contains("options") ||
               name.Contains("fin");
    }
    
    void ScanPanel(GameObject panel, StreamWriter writer, string indent)
    {
        RectTransform rect = panel.GetComponent<RectTransform>();
        
        writer.WriteLine($"{indent}┌─ {panel.name}");
        writer.WriteLine($"{indent}│  Actif: {panel.activeSelf}");
        
        if (rect != null)
        {
            writer.WriteLine($"{indent}│  Position: {rect.anchoredPosition}");
            writer.WriteLine($"{indent}│  Size: {rect.sizeDelta.x} x {rect.sizeDelta.y}");
            writer.WriteLine($"{indent}│  Anchors: Min({rect.anchorMin.x:F2}, {rect.anchorMin.y:F2}) Max({rect.anchorMax.x:F2}, {rect.anchorMax.y:F2})");
            writer.WriteLine($"{indent}│  Pivot: ({rect.pivot.x:F2}, {rect.pivot.y:F2})");
            writer.WriteLine($"{indent}│  Scale: ({rect.localScale.x:F2}, {rect.localScale.y:F2}, {rect.localScale.z:F2})");
        }
        
        Image image = panel.GetComponent<Image>();
        if (image != null)
        {
            writer.WriteLine($"{indent}│  Image: {(image.sprite != null ? image.sprite.name : "NULL")}");
            writer.WriteLine($"{indent}│  Color: {image.color}");
            writer.WriteLine($"{indent}│  Raycast Target: {image.raycastTarget}");
        }
        
        writer.WriteLine($"{indent}└─");
        writer.WriteLine();
    }
    
    void ScanAllTexts(StreamWriter writer)
    {
        Text[] uiTexts = FindAll<Text>();
        writer.WriteLine($"━━ UI TEXT (Legacy): {uiTexts.Length} trouvé(s) ━━");
        writer.WriteLine();
        
        foreach (Text text in uiTexts)
        {
            string path = GetHierarchyPath(text.gameObject);
            writer.WriteLine($"  • {text.gameObject.name}");
            writer.WriteLine($"    Chemin: {path}");
            writer.WriteLine($"    Texte: \"{text.text}\"");
            writer.WriteLine($"    Font: {(text.font != null ? text.font.name : "NULL")}");
            writer.WriteLine($"    Font Size: {text.fontSize}");
            writer.WriteLine($"    Alignment: {text.alignment}");
            writer.WriteLine($"    Color: {text.color}");
            writer.WriteLine($"    Raycast Target: {text.raycastTarget}");
            writer.WriteLine($"    Best Fit: {text.resizeTextForBestFit}");
            writer.WriteLine($"    Actif: {text.gameObject.activeSelf}");
            writer.WriteLine();
        }
        
        TextMeshProUGUI[] tmpTexts = FindAll<TextMeshProUGUI>();
        writer.WriteLine($"━━ TEXTMESHPRO: {tmpTexts.Length} trouvé(s) ━━");
        writer.WriteLine();
        
        foreach (TextMeshProUGUI tmp in tmpTexts)
        {
            string path = GetHierarchyPath(tmp.gameObject);
            writer.WriteLine($"  • {tmp.gameObject.name}");
            writer.WriteLine($"    Chemin: {path}");
            writer.WriteLine($"    Texte: \"{tmp.text}\"");
            writer.WriteLine($"    Font: {(tmp.font != null ? tmp.font.name : "NULL")}");
            writer.WriteLine($"    Font Size: {tmp.fontSize}");
            writer.WriteLine($"    Alignment: {tmp.alignment}");
            writer.WriteLine($"    Color: {tmp.color}");
            writer.WriteLine($"    Raycast Target: {tmp.raycastTarget}");
            writer.WriteLine($"    Auto Size: {tmp.enableAutoSizing}");
            writer.WriteLine($"    Actif: {tmp.gameObject.activeSelf}");
            writer.WriteLine();
        }
    }
    
    void ScanAllButtons(StreamWriter writer)
    {
        Button[] buttons = FindAll<Button>();
        writer.WriteLine($"Nombre total de boutons: {buttons.Length}");
        writer.WriteLine();
        
        foreach (Button button in buttons)
        {
            string path = GetHierarchyPath(button.gameObject);
            string parentPanel = GetParentPanel(button.gameObject);
            
            writer.WriteLine($"┌─ BOUTON: {button.gameObject.name}");
            writer.WriteLine($"│  Chemin: {path}");
            writer.WriteLine($"│  Panel parent: {parentPanel}");
            writer.WriteLine($"│  Actif: {button.gameObject.activeSelf}");
            writer.WriteLine($"│  Interactable: {button.interactable}");
            
            Text buttonText = button.GetComponentInChildren<Text>();
            TextMeshProUGUI buttonTMP = button.GetComponentInChildren<TextMeshProUGUI>();
            
            if (buttonText != null)
                writer.WriteLine($"│  Texte affiché: \"{buttonText.text}\"");
            else if (buttonTMP != null)
                writer.WriteLine($"│  Texte affiché: \"{buttonTMP.text}\"");
            else
                writer.WriteLine($"│  Texte affiché: [Aucun texte]");
            
            int eventCount = button.onClick.GetPersistentEventCount();
            writer.WriteLine($"│  OnClick Events: {eventCount}");
            
            if (eventCount > 0)
            {
                for (int i = 0; i < eventCount; i++)
                {
                    Object target = button.onClick.GetPersistentTarget(i);
                    string methodName = button.onClick.GetPersistentMethodName(i);
                    string targetName = "NULL";
                    
                    if (target != null)
                    {
                        if (target is Component comp)
                            targetName = $"{comp.gameObject.name}.{comp.GetType().Name}";
                        else
                            targetName = target.name;
                    }
                    
                    writer.WriteLine($"│    [{i}] {targetName}.{methodName}()");
                }
            }
            
            writer.WriteLine($"└─");
            writer.WriteLine();
        }
    }
    
    void ScanAllSliders(StreamWriter writer)
    {
        Slider[] sliders = FindAll<Slider>();
        writer.WriteLine($"Nombre total de sliders: {sliders.Length}");
        writer.WriteLine();
        
        foreach (Slider slider in sliders)
        {
            string path = GetHierarchyPath(slider.gameObject);
            
            writer.WriteLine($"┌─ SLIDER: {slider.gameObject.name}");
            writer.WriteLine($"│  Chemin: {path}");
            writer.WriteLine($"│  Actif: {slider.gameObject.activeSelf}");
            writer.WriteLine($"│  Interactable: {slider.interactable}");
            writer.WriteLine($"│  Min Value: {slider.minValue}");
            writer.WriteLine($"│  Max Value: {slider.maxValue}");
            writer.WriteLine($"│  Current Value: {slider.value}");
            writer.WriteLine($"│  Whole Numbers: {slider.wholeNumbers}");
            writer.WriteLine($"│  Direction: {slider.direction}");
            
            int eventCount = slider.onValueChanged.GetPersistentEventCount();
            writer.WriteLine($"│  OnValueChanged Events: {eventCount}");
            
            if (eventCount > 0)
            {
                for (int i = 0; i < eventCount; i++)
                {
                    Object target = slider.onValueChanged.GetPersistentTarget(i);
                    string methodName = slider.onValueChanged.GetPersistentMethodName(i);
                    string targetName = "NULL";
                    
                    if (target != null)
                    {
                        if (target is Component comp)
                            targetName = $"{comp.gameObject.name}.{comp.GetType().Name}";
                        else
                            targetName = target.name;
                    }
                    
                    writer.WriteLine($"│    [{i}] {targetName}.{methodName}(float)");
                }
            }
            
            writer.WriteLine($"└─");
            writer.WriteLine();
        }
    }
    
    void ScanUIScripts(StreamWriter writer)
    {
        string[] scriptNames = {
            "GestionnaireCanvas",
            "FinDePartie",
            "SystemeJeu",
            "UIManager",
            "GameManager",
            "MenuManager",
            "HUDManager",
            "CanvasManager"
        };
        
        MonoBehaviour[] allScripts = FindAll<MonoBehaviour>();
        
        foreach (string scriptName in scriptNames)
        {
            bool found = false;
            
            foreach (MonoBehaviour script in allScripts)
            {
                if (script.GetType().Name.Contains(scriptName))
                {
                    if (!found)
                    {
                        writer.WriteLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                        writer.WriteLine($"SCRIPT: {script.GetType().Name}");
                        writer.WriteLine($"GameObject: {script.gameObject.name}");
                        writer.WriteLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                        writer.WriteLine();
                        found = true;
                    }
                    
                    ScanScriptUIReferences(script, writer);
                }
            }
        }
        
        writer.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        writer.WriteLine("AUTRES SCRIPTS AVEC RÉFÉRENCES UI");
        writer.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        writer.WriteLine();
        
        foreach (MonoBehaviour script in allScripts)
        {
            if (HasUIReferences(script) && !scriptNames.Any(name => script.GetType().Name.Contains(name)))
            {
                writer.WriteLine($"▸ {script.GetType().Name} (sur {script.gameObject.name})");
                ScanScriptUIReferences(script, writer);
            }
        }
    }
    
    bool HasUIReferences(MonoBehaviour script)
    {
        System.Type type = script.GetType();
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        
        foreach (FieldInfo field in fields)
        {
            if (IsUIType(field.FieldType))
                return true;
        }
        
        return false;
    }
    
    void ScanScriptUIReferences(MonoBehaviour script, StreamWriter writer)
    {
        System.Type type = script.GetType();
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        
        int refCount = 0;
        
        foreach (FieldInfo field in fields)
        {
            bool isSerializable = field.IsPublic || field.GetCustomAttributes(typeof(SerializeField), false).Length > 0;
            if (!isSerializable) continue;
            
            if (IsUIType(field.FieldType))
            {
                refCount++;
                object value = field.GetValue(script);
                
                writer.WriteLine($"  • {field.Name} ({field.FieldType.Name})");
                
                if (value != null)
                {
                    if (value is Component comp)
                    {
                        writer.WriteLine($"    → GameObject: {comp.gameObject.name}");
                        writer.WriteLine($"    → Chemin: {GetHierarchyPath(comp.gameObject)}");
                        
                        if (value is Text text)
                            writer.WriteLine($"    → Texte actuel: \"{text.text}\"");
                        else if (value is TextMeshProUGUI tmp)
                            writer.WriteLine($"    → Texte actuel: \"{tmp.text}\"");
                        else if (value is Button button)
                            writer.WriteLine($"    → Interactable: {button.interactable}");
                        else if (value is Slider slider)
                            writer.WriteLine($"    → Value: {slider.value} (min: {slider.minValue}, max: {slider.maxValue})");
                    }
                    else if (value is GameObject go)
                    {
                        writer.WriteLine($"    → GameObject: {go.name}");
                        writer.WriteLine($"    → Chemin: {GetHierarchyPath(go)}");
                        writer.WriteLine($"    → Actif: {go.activeSelf}");
                    }
                }
                else
                {
                    writer.WriteLine($"    → NULL (non assigné dans l'Inspector)");
                }
                
                writer.WriteLine();
            }
        }
        
        if (refCount == 0)
        {
            writer.WriteLine("  Aucune référence UI trouvée");
            writer.WriteLine();
        }
    }
    
    bool IsUIType(System.Type type)
    {
        return type == typeof(Canvas) ||
               type == typeof(Text) ||
               type == typeof(TextMeshProUGUI) ||
               type == typeof(Button) ||
               type == typeof(Slider) ||
               type == typeof(Toggle) ||
               type == typeof(Image) ||
               type == typeof(RawImage) ||
               type == typeof(InputField) ||
               type == typeof(TMP_InputField) ||
               type == typeof(ScrollRect) ||
               type == typeof(Dropdown) ||
               type == typeof(TMP_Dropdown) ||
               type == typeof(GameObject);
    }
    
    void WriteStatistics(StreamWriter writer)
    {
        Canvas[] canvases = FindAll<Canvas>();
        Text[] texts = FindAll<Text>();
        TextMeshProUGUI[] tmps = FindAll<TextMeshProUGUI>();
        Button[] buttons = FindAll<Button>();
        Slider[] sliders = FindAll<Slider>();
        Image[] images = FindAll<Image>();
        
        writer.WriteLine($"Canvas:                {canvases.Length}");
        writer.WriteLine($"Textes (UI Text):      {texts.Length}");
        writer.WriteLine($"Textes (TextMeshPro):  {tmps.Length}");
        writer.WriteLine($"Boutons:               {buttons.Length}");
        writer.WriteLine($"Sliders:               {sliders.Length}");
        writer.WriteLine($"Images:                {images.Length}");
        writer.WriteLine();
        
        int activeCanvas = canvases.Count(c => c.gameObject.activeSelf);
        int activeButtons = buttons.Count(b => b.gameObject.activeSelf);
        
        writer.WriteLine($"Canvas actifs:         {activeCanvas}/{canvases.Length}");
        writer.WriteLine($"Boutons actifs:        {activeButtons}/{buttons.Length}");
    }
    
    string GetHierarchyPath(GameObject obj)
    {
        string path = obj.name;
        Transform parent = obj.transform.parent;
        
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        
        return path;
    }
    
    string GetParentPanel(GameObject obj)
    {
        Transform parent = obj.transform.parent;
        
        while (parent != null)
        {
            if (IsPanel(parent.gameObject))
                return parent.name;
            parent = parent.parent;
        }
        
        return "[Aucun panel parent]";
    }
}
#endif