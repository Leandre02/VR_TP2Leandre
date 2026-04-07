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

public class UIVisualScanner : EditorWindow
{
    Vector2 scrollPos;
    bool includeInactive = true;
    bool detectAnomalies = true;
    bool generateASCII = true;
    
    // Panels prioritaires à scanner
    private readonly string[] priorityPanels = {
        "GestionnaireCanvas/FinDePartie",
        "FinDePartie/PanelVictoire",
        "FinDePartie/PanelDefaite",
        "GestionnaireCanvas/Minuteur",
        "GestionnaireCanvas/FenetreQuete",
        "MenuPrincipal/FenetreMenu"
    };
    
    [MenuItem("Tools/UI Visual Scanner")]
    public static void ShowWindow()
    {
        GetWindow<UIVisualScanner>("UI Visual Scanner");
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
        
        GUILayout.Label("UI Visual Scanner - Analyse Complète", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Scanner UI ultra-détaillé avec RectTransform, anomalies et visualisation ASCII", MessageType.Info);
        GUILayout.Space(10);
        
        includeInactive = EditorGUILayout.Toggle("Inclure objets inactifs", includeInactive);
        detectAnomalies = EditorGUILayout.Toggle("Détecter anomalies UI", detectAnomalies);
        generateASCII = EditorGUILayout.Toggle("Générer schémas ASCII", generateASCII);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("🔍 SCANNER L'UI COMPLÈTE", GUILayout.Height(40)))
        {
            ScanUIComplete();
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
    
    void ScanUIComplete()
    {
        string reportsDir = Path.Combine(Application.dataPath, "../UIReports");
        if (!Directory.Exists(reportsDir))
        {
            Directory.CreateDirectory(reportsDir);
        }
        
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filename = $"UI_Visual_Scan_{timestamp}.txt";
        string filepath = Path.Combine(reportsDir, filename);
        
        List<string> anomalies = new List<string>();
        
        using (StreamWriter writer = new StreamWriter(filepath))
        {
            WriteHeader(writer);
            
            // 1. TOUS LES CANVAS
            Canvas[] allCanvas = FindAll<Canvas>();
            writer.WriteLine("╔══════════════════════════════════════════════════════════════════════════╗");
            writer.WriteLine("║  PARTIE 1 : TOUS LES CANVAS");
            writer.WriteLine("╚══════════════════════════════════════════════════════════════════════════╝");
            writer.WriteLine();
            
            foreach (Canvas canvas in allCanvas)
            {
                ScanCanvasComplete(canvas, writer, anomalies);
            }
            
            // 2. PANELS PRIORITAIRES
            writer.WriteLine();
            writer.WriteLine("╔══════════════════════════════════════════════════════════════════════════╗");
            writer.WriteLine("║  PARTIE 2 : PANELS PRIORITAIRES (ANALYSE DÉTAILLÉE)");
            writer.WriteLine("╚══════════════════════════════════════════════════════════════════════════╝");
            writer.WriteLine();
            
            ScanPriorityPanels(writer, anomalies);
            
            // 3. HIÉRARCHIE ASCII
            if (generateASCII)
            {
                writer.WriteLine();
                writer.WriteLine("╔══════════════════════════════════════════════════════════════════════════╗");
                writer.WriteLine("║  PARTIE 3 : HIÉRARCHIE UI (SCHÉMA ASCII)");
                writer.WriteLine("╚══════════════════════════════════════════════════════════════════════════╝");
                writer.WriteLine();
                
                GenerateASCIIHierarchy(writer);
            }
            
            // 4. TOUS LES TEXTMESHPRO
            writer.WriteLine();
            writer.WriteLine("╔══════════════════════════════════════════════════════════════════════════╗");
            writer.WriteLine("║  PARTIE 4 : TOUS LES TEXTMESHPRO (DÉTAILS COMPLETS)");
            writer.WriteLine("╚══════════════════════════════════════════════════════════════════════════╝");
            writer.WriteLine();
            
            ScanAllTextMeshPro(writer, anomalies);
            
            // 5. TOUS LES BOUTONS
            writer.WriteLine();
            writer.WriteLine("╔══════════════════════════════════════════════════════════════════════════╗");
            writer.WriteLine("║  PARTIE 5 : TOUS LES BOUTONS (AVEC ONCLICK)");
            writer.WriteLine("╚══════════════════════════════════════════════════════════════════════════╝");
            writer.WriteLine();
            
            ScanAllButtons(writer);
            
            // 6. ANOMALIES DÉTECTÉES
            if (detectAnomalies && anomalies.Count > 0)
            {
                writer.WriteLine();
                writer.WriteLine("╔══════════════════════════════════════════════════════════════════════════╗");
                writer.WriteLine("║  PARTIE 6 : ⚠️  ANOMALIES DÉTECTÉES");
                writer.WriteLine("╚══════════════════════════════════════════════════════════════════════════╝");
                writer.WriteLine();
                
                foreach (string anomaly in anomalies)
                {
                    writer.WriteLine($"⚠️  {anomaly}");
                }
            }
            else if (detectAnomalies)
            {
                writer.WriteLine();
                writer.WriteLine("╔══════════════════════════════════════════════════════════════════════════╗");
                writer.WriteLine("║  PARTIE 6 : ✅ AUCUNE ANOMALIE DÉTECTÉE");
                writer.WriteLine("╚══════════════════════════════════════════════════════════════════════════╝");
                writer.WriteLine();
            }
            
            // 7. STATISTIQUES FINALES
            writer.WriteLine();
            writer.WriteLine("╔══════════════════════════════════════════════════════════════════════════╗");
            writer.WriteLine("║  PARTIE 7 : STATISTIQUES FINALES");
            writer.WriteLine("╚══════════════════════════════════════════════════════════════════════════╝");
            writer.WriteLine();
            
            WriteStatistics(writer, anomalies.Count);
            
            writer.WriteLine();
            writer.WriteLine("════════════════════════════════════════════════════════════════════════════");
            writer.WriteLine($"FIN DU SCAN - {System.DateTime.Now}");
            writer.WriteLine("════════════════════════════════════════════════════════════════════════════");
        }
        
        EditorUtility.DisplayDialog("Succès", 
            $"Scan UI complet généré!\n\n{filepath}\n\nAnomalies trouvées: {anomalies.Count}", 
            "OK");
        EditorUtility.RevealInFinder(filepath);
    }
    
    void WriteHeader(StreamWriter writer)
    {
        writer.WriteLine("╔════════════════════════════════════════════════════════════════════════════╗");
        writer.WriteLine("║                    UI VISUAL SCANNER - RAPPORT COMPLET                     ║");
        writer.WriteLine("║                    RECTTRANSFORM + ANOMALIES + ASCII                       ║");
        writer.WriteLine("╚════════════════════════════════════════════════════════════════════════════╝");
        writer.WriteLine();
        writer.WriteLine($"Scène: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        writer.WriteLine($"Date: {System.DateTime.Now}");
        writer.WriteLine($"Résolution écran: {Screen.width}x{Screen.height}");
        writer.WriteLine();
        writer.WriteLine("════════════════════════════════════════════════════════════════════════════");
        writer.WriteLine();
    }
    
    void ScanCanvasComplete(Canvas canvas, StreamWriter writer, List<string> anomalies)
    {
        writer.WriteLine("┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓");
        writer.WriteLine($"┃ CANVAS: {canvas.gameObject.name}");
        writer.WriteLine("┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛");
        writer.WriteLine();
        
        writer.WriteLine($"  Actif: {canvas.gameObject.activeSelf}");
        writer.WriteLine($"  Render Mode: {canvas.renderMode}");
        writer.WriteLine($"  Sort Order: {canvas.sortingOrder}");
        
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
        {
            writer.WriteLine($"  World Camera: {(canvas.worldCamera != null ? canvas.worldCamera.name : "⚠️  NULL")}");
            if (canvas.worldCamera == null)
            {
                anomalies.Add($"Canvas '{canvas.name}' : Render Mode = {canvas.renderMode} mais World Camera = NULL");
            }
        }
        
        writer.WriteLine();
        
        // Canvas Scaler
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            writer.WriteLine("  [CANVAS SCALER]");
            writer.WriteLine($"    UI Scale Mode: {scaler.uiScaleMode}");
            
            if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                writer.WriteLine($"    Reference Resolution: {scaler.referenceResolution.x} x {scaler.referenceResolution.y}");
                writer.WriteLine($"    Screen Match Mode: {scaler.screenMatchMode}");
                writer.WriteLine($"    Match Width/Height: {scaler.matchWidthOrHeight:F2}");
                
                if (scaler.referenceResolution.x == 0 || scaler.referenceResolution.y == 0)
                {
                    anomalies.Add($"Canvas '{canvas.name}' : Reference Resolution est 0 !");
                }
            }
        }
        else
        {
            writer.WriteLine("  ⚠️  Aucun Canvas Scaler trouvé");
            anomalies.Add($"Canvas '{canvas.name}' : Pas de CanvasScaler (recommandé pour UI responsive)");
        }
        
        writer.WriteLine();
        writer.WriteLine("─────────────────────────────────────────────────────────────────────────────");
        writer.WriteLine();
    }
    
    void ScanPriorityPanels(StreamWriter writer, List<string> anomalies)
    {
        foreach (string panelPath in priorityPanels)
        {
            GameObject panel = GameObject.Find(panelPath);
            
            if (panel == null)
            {
                string panelName = panelPath.Split('/').Last();
                GameObject[] allObjects = FindObjectsByType<GameObject>(
                    FindObjectsInactive.Include, FindObjectsSortMode.None);
                panel = allObjects.FirstOrDefault(obj => obj.name == panelName);
            }
            
            if (panel != null)
            {
                writer.WriteLine($"▼▼▼ PANEL PRIORITAIRE: {panelPath} ▼▼▼");
                writer.WriteLine();
                ScanPanelDetailed(panel, writer, anomalies);
                
                if (generateASCII)
                {
                    GeneratePanelASCIIVisual(panel, writer);
                }
            }
            else
            {
                writer.WriteLine($"⚠️  PANEL NON TROUVÉ: {panelPath}");
                writer.WriteLine();
                anomalies.Add($"Panel prioritaire introuvable: {panelPath}");
            }
        }
    }
    
    void ScanPanelDetailed(GameObject panel, StreamWriter writer, List<string> anomalies)
    {
        RectTransform rect = panel.GetComponent<RectTransform>();
        
        writer.WriteLine($"  Nom: {panel.name}");
        writer.WriteLine($"  Chemin: {GetHierarchyPath(panel)}");
        writer.WriteLine($"  Actif: {panel.activeSelf}");
        writer.WriteLine();
        
        if (rect != null)
        {
            writer.WriteLine("  [RECTTRANSFORM]");
            writer.WriteLine($"    Anchors Min: ({rect.anchorMin.x:F3}, {rect.anchorMin.y:F3})");
            writer.WriteLine($"    Anchors Max: ({rect.anchorMax.x:F3}, {rect.anchorMax.y:F3})");
            writer.WriteLine($"    Pivot: ({rect.pivot.x:F3}, {rect.pivot.y:F3})");
            writer.WriteLine($"    Position: ({rect.anchoredPosition.x:F1}, {rect.anchoredPosition.y:F1})");
            writer.WriteLine($"    Size: {rect.sizeDelta.x:F1} x {rect.sizeDelta.y:F1}");
            writer.WriteLine($"    Scale: ({rect.localScale.x:F2}, {rect.localScale.y:F2}, {rect.localScale.z:F2})");
            writer.WriteLine($"    Rotation: {rect.localRotation.eulerAngles}");
            writer.WriteLine();
            
            if (rect.sizeDelta.x <= 0 || rect.sizeDelta.y <= 0)
            {
                anomalies.Add($"Panel '{panel.name}' : Taille 0x0 ou négative ! ({rect.sizeDelta.x:F0} x {rect.sizeDelta.y:F0})");
            }
            
            if (rect.localScale.x == 0 || rect.localScale.y == 0)
            {
                anomalies.Add($"Panel '{panel.name}' : Scale = 0 (invisible) !");
            }
            
            Canvas parentCanvas = panel.GetComponentInParent<Canvas>();
            if (parentCanvas != null && parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                Vector3[] corners = new Vector3[4];
                rect.GetWorldCorners(corners);
                
                bool isOffScreen = corners.All(c => c.x < 0 || c.x > Screen.width || c.y < 0 || c.y > Screen.height);
                if (isOffScreen && panel.activeSelf)
                {
                    anomalies.Add($"Panel '{panel.name}' : Complètement hors écran !");
                }
            }
            
            string anchorType = GetAnchorType(rect);
            writer.WriteLine($"    Type d'ancrage: {anchorType}");
            writer.WriteLine();
        }
        
        Canvas canvas = panel.GetComponentInParent<Canvas>();
        if (canvas != null && !canvas.gameObject.activeSelf && panel.activeSelf)
        {
            anomalies.Add($"Panel '{panel.name}' : Actif mais Canvas parent '{canvas.name}' est inactif !");
        }
        
        writer.WriteLine("  [COMPOSANTS]");
        Component[] components = panel.GetComponents<Component>();
        foreach (Component comp in components)
        {
            if (comp == null || comp is RectTransform || comp is CanvasRenderer) continue;
            
            writer.WriteLine($"    • {comp.GetType().Name}");
            
            if (comp is Image img)
            {
                writer.WriteLine($"      - Color: {img.color}");
                writer.WriteLine($"      - Sprite: {(img.sprite != null ? img.sprite.name : "NULL")}");
                writer.WriteLine($"      - Raycast Target: {img.raycastTarget}");
                
                if (img.color.a == 0)
                {
                    anomalies.Add($"Panel '{panel.name}' : Image avec alpha = 0 (invisible)");
                }
            }
        }
        
        writer.WriteLine();
        writer.WriteLine("─────────────────────────────────────────────────────────────────────────────");
        writer.WriteLine();
    }
    
    void GeneratePanelASCIIVisual(GameObject panel, StreamWriter writer)
    {
        RectTransform rect = panel.GetComponent<RectTransform>();
        if (rect == null) return;
        
        writer.WriteLine("  [VISUALISATION ASCII]");
        writer.WriteLine();
        
        int width = Mathf.Clamp((int)(rect.sizeDelta.x / 20), 20, 60);
        int height = Mathf.Clamp((int)(rect.sizeDelta.y / 40), 5, 15);
        
        writer.Write("  ");
        writer.Write("┌");
        writer.Write(new string('─', width - 2));
        writer.WriteLine("┐");
        
        TextMeshProUGUI[] texts = panel.GetComponentsInChildren<TextMeshProUGUI>(includeInactive);
        Button[] buttons = panel.GetComponentsInChildren<Button>(includeInactive);
        
        int currentLine = 0;
        
        if (currentLine < height - 2)
        {
            string title = $"[ {panel.name} ]";
            int padding = (width - title.Length - 2) / 2;
            writer.Write("  │");
            writer.Write(new string(' ', padding));
            writer.Write(title);
            writer.Write(new string(' ', width - title.Length - padding - 2));
            writer.WriteLine("│");
            currentLine++;
        }
        
        if (currentLine < height - 2)
        {
            writer.Write("  │");
            writer.Write(new string(' ', width - 2));
            writer.WriteLine("│");
            currentLine++;
        }
        
        foreach (var text in texts.Take(3))
        {
            if (currentLine >= height - 2) break;
            
            string content = text.text.Length > width - 6 ? text.text.Substring(0, width - 9) + "..." : text.text;
            int padding = (width - content.Length - 2) / 2;
            
            writer.Write("  │");
            writer.Write(new string(' ', padding));
            writer.Write(content);
            writer.Write(new string(' ', width - content.Length - padding - 2));
            writer.WriteLine("│");
            currentLine++;
        }
        
        if (buttons.Length > 0 && currentLine < height - 2)
        {
            writer.Write("  │");
            writer.Write(new string(' ', width - 2));
            writer.WriteLine("│");
            currentLine++;
        }
        
        if (buttons.Length > 0 && currentLine < height - 2)
        {
            StringBuilder btnLine = new StringBuilder();
            foreach (var btn in buttons.Take(3))
            {
                Text btnText = btn.GetComponentInChildren<Text>();
                TextMeshProUGUI btnTMP = btn.GetComponentInChildren<TextMeshProUGUI>();
                string btnLabel = btnText != null ? btnText.text : (btnTMP != null ? btnTMP.text : btn.name);
                btnLine.Append($"[ {btnLabel} ] ");
            }
            
            string buttonContent = btnLine.ToString();
            if (buttonContent.Length > width - 4)
                buttonContent = buttonContent.Substring(0, width - 7) + "...";
            
            int btnPadding = (width - buttonContent.Length - 2) / 2;
            writer.Write("  │");
            writer.Write(new string(' ', btnPadding));
            writer.Write(buttonContent);
            writer.Write(new string(' ', width - buttonContent.Length - btnPadding - 2));
            writer.WriteLine("│");
            currentLine++;
        }
        
        while (currentLine < height - 2)
        {
            writer.Write("  │");
            writer.Write(new string(' ', width - 2));
            writer.WriteLine("│");
            currentLine++;
        }
        
        writer.Write("  ");
        writer.Write("└");
        writer.Write(new string('─', width - 2));
        writer.WriteLine("┘");
        
        writer.WriteLine($"  Taille réelle: {rect.sizeDelta.x:F0}x{rect.sizeDelta.y:F0}");
        writer.WriteLine();
    }
    
    void GenerateASCIIHierarchy(StreamWriter writer)
    {
        Canvas[] canvases = FindAll<Canvas>();
        
        foreach (Canvas canvas in canvases)
        {
            writer.WriteLine($"{canvas.name} {(canvas.gameObject.activeSelf ? "" : "[INACTIF]")}");
            GenerateHierarchyRecursive(canvas.transform, writer, "", true);
            writer.WriteLine();
        }
    }
    
    void GenerateHierarchyRecursive(Transform parent, StreamWriter writer, string prefix, bool isLast)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            bool isLastChild = (i == parent.childCount - 1);
            
            string connector = isLastChild ? "└─ " : "├─ ";
            string childPrefix = isLast ? "   " : "│  ";
            
            string icon = "";
            if (child.GetComponent<Button>() != null) icon = "🔘 ";
            else if (child.GetComponent<TextMeshProUGUI>() != null || child.GetComponent<Text>() != null) icon = "📝 ";
            else if (child.GetComponent<Image>() != null) icon = "🖼️  ";
            else if (child.GetComponent<Slider>() != null) icon = "🎚️  ";
            else if (IsPanel(child.gameObject)) icon = "📦 ";
            
            string status = child.gameObject.activeSelf ? "" : " [OFF]";
            
            writer.WriteLine($"{prefix}{connector}{icon}{child.name}{status}");
            
            if (child.childCount > 0)
                GenerateHierarchyRecursive(child, writer, prefix + childPrefix, isLastChild);
        }
    }
    
    void ScanAllTextMeshPro(StreamWriter writer, List<string> anomalies)
    {
        TextMeshProUGUI[] allTMPs = FindAll<TextMeshProUGUI>();
        writer.WriteLine($"Total TextMeshPro trouvés: {allTMPs.Length}");
        writer.WriteLine();
        
        foreach (TextMeshProUGUI tmp in allTMPs)
        {
            writer.WriteLine($"┌─ {tmp.gameObject.name}");
            writer.WriteLine($"│  Chemin: {GetHierarchyPath(tmp.gameObject)}");
            writer.WriteLine($"│  Actif: {tmp.gameObject.activeSelf}");
            writer.WriteLine($"│");
            writer.WriteLine($"│  Texte: \"{tmp.text}\"");
            writer.WriteLine($"│  Font: {(tmp.font != null ? tmp.font.name : "NULL")}");
            writer.WriteLine($"│  Font Size: {tmp.fontSize}");
            writer.WriteLine($"│  Auto Size: {tmp.enableAutoSizing} {(tmp.enableAutoSizing ? $"(Min: {tmp.fontSizeMin}, Max: {tmp.fontSizeMax})" : "")}");
            writer.WriteLine($"│  Alignment: {tmp.alignment}");
            writer.WriteLine($"│  Color: {tmp.color}");
            writer.WriteLine($"│  Overflow Mode: {tmp.overflowMode}");
            writer.WriteLine($"│  Margins: L:{tmp.margin.x:F1} R:{tmp.margin.y:F1} T:{tmp.margin.z:F1} B:{tmp.margin.w:F1}");
            writer.WriteLine($"│  Raycast Target: {tmp.raycastTarget}");
            writer.WriteLine($"│");
            
            RectTransform rect = tmp.GetComponent<RectTransform>();
            if (rect != null)
                writer.WriteLine($"│  Size: {rect.sizeDelta.x:F1} x {rect.sizeDelta.y:F1}");
            
            writer.WriteLine($"└─");
            writer.WriteLine();
            
            if (tmp.color.a == 0 && tmp.gameObject.activeSelf)
                anomalies.Add($"TextMeshPro '{tmp.gameObject.name}' : Alpha = 0 (invisible)");
            
            Canvas parentCanvas = tmp.GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                Image[] parentImages = tmp.GetComponentsInParent<Image>();
                foreach (Image img in parentImages)
                {
                    if (img.color == tmp.color && img.color.a > 0.5f)
                    {
                        anomalies.Add($"TextMeshPro '{tmp.gameObject.name}' : Même couleur que background (invisible)");
                        break;
                    }
                }
            }
            
            if (rect != null && (rect.sizeDelta.x <= 0 || rect.sizeDelta.y <= 0))
                anomalies.Add($"TextMeshPro '{tmp.gameObject.name}' : Taille 0 ou négative");
        }
    }
    
    void ScanAllButtons(StreamWriter writer)
    {
        Button[] buttons = FindAll<Button>();
        writer.WriteLine($"Total boutons trouvés: {buttons.Length}");
        writer.WriteLine();
        
        foreach (Button button in buttons)
        {
            writer.WriteLine($"┌─ BOUTON: {button.gameObject.name}");
            writer.WriteLine($"│  Chemin: {GetHierarchyPath(button.gameObject)}");
            writer.WriteLine($"│  Actif: {button.gameObject.activeSelf}");
            writer.WriteLine($"│  Interactable: {button.interactable}");
            
            Text btnText = button.GetComponentInChildren<Text>();
            TextMeshProUGUI btnTMP = button.GetComponentInChildren<TextMeshProUGUI>();
            
            if (btnText != null)
                writer.WriteLine($"│  Texte: \"{btnText.text}\"");
            else if (btnTMP != null)
                writer.WriteLine($"│  Texte: \"{btnTMP.text}\"");
            else
                writer.WriteLine($"│  Texte: [Aucun]");
            
            int eventCount = button.onClick.GetPersistentEventCount();
            writer.WriteLine($"│  OnClick Events: {eventCount}");
            
            if (eventCount > 0)
            {
                for (int i = 0; i < eventCount; i++)
                {
                    UnityEngine.Object target = button.onClick.GetPersistentTarget(i);
                    string methodName = button.onClick.GetPersistentMethodName(i);
                    
                    if (target != null)
                    {
                        string targetInfo = target is Component comp ? 
                            $"{comp.gameObject.name}.{comp.GetType().Name}" : 
                            target.name;
                        writer.WriteLine($"│    → {targetInfo}.{methodName}()");
                    }
                    else
                    {
                        writer.WriteLine($"│    → NULL.{methodName}() ⚠️");
                    }
                }
            }
            
            writer.WriteLine($"└─");
            writer.WriteLine();
        }
    }
    
    void WriteStatistics(StreamWriter writer, int anomalyCount)
    {
        Canvas[] canvases = FindAll<Canvas>();
        TextMeshProUGUI[] tmps = FindAll<TextMeshProUGUI>();
        Text[] texts = FindAll<Text>();
        Button[] buttons = FindAll<Button>();
        Slider[] sliders = FindAll<Slider>();
        Image[] images = FindAll<Image>();
        
        writer.WriteLine($"Canvas:                {canvases.Length}");
        writer.WriteLine($"TextMeshPro:           {tmps.Length}");
        writer.WriteLine($"Text (Legacy):         {texts.Length}");
        writer.WriteLine($"Boutons:               {buttons.Length}");
        writer.WriteLine($"Sliders:               {sliders.Length}");
        writer.WriteLine($"Images:                {images.Length}");
        writer.WriteLine();
        writer.WriteLine($"Anomalies détectées:   {anomalyCount}");
    }
    
    string GetAnchorType(RectTransform rect)
    {
        Vector2 min = rect.anchorMin;
        Vector2 max = rect.anchorMax;
        
        if (min == max)
        {
            if (min == new Vector2(0.5f, 0.5f)) return "Center";
            if (min == new Vector2(0, 0)) return "Bottom-Left";
            if (min == new Vector2(1, 0)) return "Bottom-Right";
            if (min == new Vector2(0, 1)) return "Top-Left";
            if (min == new Vector2(1, 1)) return "Top-Right";
            if (min.x == 0.5f && min.y == 0) return "Bottom-Center";
            if (min.x == 0.5f && min.y == 1) return "Top-Center";
            if (min.x == 0 && min.y == 0.5f) return "Middle-Left";
            if (min.x == 1 && min.y == 0.5f) return "Middle-Right";
            return "Custom Point";
        }
        
        if (min == Vector2.zero && max == Vector2.one) return "Stretch (Full)";
        if (min.x == 0 && max.x == 1 && min.y == max.y) return "Stretch Horizontal";
        if (min.y == 0 && max.y == 1 && min.x == max.x) return "Stretch Vertical";
        
        return "Custom Area";
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
    
    bool IsPanel(GameObject obj)
    {
        string name = obj.name.ToLower();
        return name.Contains("panel") || 
               name.Contains("hud") || 
               name.Contains("menu") ||
               name.Contains("popup") ||
               name.Contains("window") ||
               name.Contains("victoire") ||
               name.Contains("defaite") ||
               name.Contains("pause") ||
               name.Contains("fin");
    }
}
#endif