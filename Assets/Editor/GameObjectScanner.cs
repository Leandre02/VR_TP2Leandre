using UnityEditor;
using UnityEngine;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

public class GameObjectScanner : EditorWindow
{
    GameObject cible;
    Vector2 scrollPos;
    bool scanChildren = true;
    bool includePrivateFields = false;
    bool includeProperties = false;
    int maxDepth = 5;

    [MenuItem("Tools/Scanner GameObject")]
    public static void ShowWindow()
    {
        GetWindow<GameObjectScanner>("Scanner GameObject");
    }

    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUILayout.Label("Scanner GameObject", EditorStyles.boldLabel);
        GUILayout.Space(5);

        cible = EditorGUILayout.ObjectField("Objet a scanner", cible, typeof(GameObject), true) as GameObject;

        GUILayout.Space(10);
        GUILayout.Label("Options:", EditorStyles.miniLabel);
        scanChildren = EditorGUILayout.Toggle("Scanner les enfants", scanChildren);
        includePrivateFields = EditorGUILayout.Toggle("Champs prives (lent!)", includePrivateFields);
        includeProperties = EditorGUILayout.Toggle("Proprietes (tres lent!)", includeProperties);
        maxDepth = EditorGUILayout.IntSlider("Profondeur max", maxDepth, 1, 10);

        GUILayout.Space(10);

        if (GUILayout.Button("Scan Rapide Grille TicTacToe", GUILayout.Height(40)))
        {
            if (cible == null)
            {
                EditorUtility.DisplayDialog("Erreur", "Selectionne ton TicTacToeBoard!", "OK");
                return;
            }
            ExportQuickGridReport();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Scan Complet", GUILayout.Height(40)))
        {
            if (cible == null)
            {
                EditorUtility.DisplayDialog("Erreur", "Selectionne un GameObject!", "OK");
                return;
            }
            ExportFullReport();
        }

        EditorGUILayout.EndScrollView();
    }

    // Scan specialement pour le TicTacToe
    void ExportQuickGridReport()
    {
        string path = EditorUtility.SaveFilePanel(
            "Sauvegarder scan grille", "",
            cible.name + "_grille_scan.txt", "txt");

        if (string.IsNullOrEmpty(path)) return;

        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("=== SCAN GRILLE TIC TAC TOE ===");
            writer.WriteLine("Date: " + System.DateTime.Now);
            writer.WriteLine("Objet: " + cible.name);
            writer.WriteLine();

            writer.WriteLine("--- PARENT ---");
            writer.WriteLine("Position: " + FormatVec3(cible.transform.position));
            writer.WriteLine("Rotation: " + FormatVec3(cible.transform.rotation.eulerAngles));
            writer.WriteLine("Scale: " + FormatVec3(cible.transform.localScale));
            writer.WriteLine("Nombre d enfants: " + cible.transform.childCount);
            writer.WriteLine();

            writer.WriteLine("--- CASES ---");
            int casesFound = 0;
            ScanChildrenForBoard(cible.transform, writer, ref casesFound, 0);

            writer.WriteLine();
            writer.WriteLine("Total cases avec Board.cs: " + casesFound);

            if (casesFound != 9)
                writer.WriteLine("ATTENTION: Il devrait y avoir exactement 9 cases!");
            else
                writer.WriteLine("Parfait! 9 cases trouvees.");
        }

        EditorUtility.DisplayDialog("Succes", "Scan grille exporte!\n" + path, "OK");
        EditorUtility.RevealInFinder(path);
    }

    void ScanChildrenForBoard(Transform parent, StreamWriter writer, ref int count, int depth)
    {
        if (depth > maxDepth) return;

        foreach (Transform child in parent)
        {
            writer.WriteLine("Enfant: " + child.name);
            writer.WriteLine("  Pos locale:   " + FormatVec3(child.localPosition));
            writer.WriteLine("  Rot locale:   " + FormatVec3(child.localRotation.eulerAngles));
            writer.WriteLine("  Scale locale: " + FormatVec3(child.localScale));

            // Verifier collider
            Collider col = child.GetComponent<Collider>();
            writer.WriteLine("  Collider: " + (col != null ? col.GetType().Name : "AUCUN <- PROBLEME!"));

            // Chercher script Board
            MonoBehaviour[] scripts = child.GetComponents<MonoBehaviour>();
            bool hasBoard = false;

            foreach (MonoBehaviour script in scripts)
            {
                if (script == null) continue;
                if (script.GetType().Name != "Board") continue;

                hasBoard = true;
                count++;

                System.Type type = script.GetType();
                FieldInfo rowField = type.GetField("row") ?? type.GetField("lignes");
                FieldInfo colField = type.GetField("col") ?? type.GetField("colonnes");

                string rowVal = rowField != null ? rowField.GetValue(script).ToString() : "CHAMP INTROUVABLE";
                string colVal = colField != null ? colField.GetValue(script).ToString() : "CHAMP INTROUVABLE";

                writer.WriteLine("  Board.cs: OUI");
                writer.WriteLine("  row: " + rowVal);
                writer.WriteLine("  col: " + colVal);

                // Verifier si les valeurs sont valides
                if (rowField != null && colField != null)
                {
                    int r = (int)rowField.GetValue(script);
                    int c = (int)colField.GetValue(script);

                    if (r < 0 || r > 2 || c < 0 || c > 2)
                        writer.WriteLine("  VALEURS HORS LIMITES! r=" + r + " c=" + c + " <- PROBLEME!");
                    else
                        writer.WriteLine("  Indices valides [0-2] OK");
                }
            }

            if (!hasBoard)
                writer.WriteLine("  Board.cs: AUCUN");

            writer.WriteLine();

            if (child.childCount > 0)
                ScanChildrenForBoard(child, writer, ref count, depth + 1);
        }
    }

    // Scan complet
    void ExportFullReport()
    {
        string path = EditorUtility.SaveFilePanel(
            "Exporter rapport complet", "",
            cible.name + "_rapport_complet.txt", "txt");

        if (string.IsNullOrEmpty(path)) return;

        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("=== RAPPORT COMPLET ===");
            writer.WriteLine("Date: " + System.DateTime.Now);
            writer.WriteLine("Scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            writer.WriteLine();

            ScanObject(cible, writer, 0);

            writer.WriteLine();
            writer.WriteLine("=== FIN DU RAPPORT ===");
        }

        EditorUtility.DisplayDialog("Succes", "Rapport exporte!\n" + path, "OK");
        EditorUtility.RevealInFinder(path);
    }

    void ScanObject(GameObject obj, StreamWriter writer, int depth)
    {
        if (depth > maxDepth) return;

        string indent = new string(' ', depth * 2);

        writer.WriteLine(indent + "--- " + obj.name + " ---");
        writer.WriteLine(indent + "Actif: " + obj.activeSelf);
        writer.WriteLine(indent + "Tag: " + obj.tag);
        writer.WriteLine(indent + "Layer: " + LayerMask.LayerToName(obj.layer));

        Transform t = obj.transform;
        writer.WriteLine(indent + "Position locale: " + FormatVec3(t.localPosition));
        writer.WriteLine(indent + "Rotation locale: " + FormatVec3(t.localRotation.eulerAngles));
        writer.WriteLine(indent + "Scale locale:    " + FormatVec3(t.localScale));

        Component[] components = obj.GetComponents<Component>();
        writer.WriteLine(indent + "Components (" + components.Length + "):");

        foreach (Component comp in components)
        {
            if (comp == null)
            {
                writer.WriteLine(indent + "  [MANQUANT - script supprime?]");
                continue;
            }

            if (comp is Transform) continue;

            writer.WriteLine(indent + "  " + comp.GetType().Name);

            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            if (includePrivateFields)
                flags |= BindingFlags.NonPublic;

            FieldInfo[] fields = comp.GetType().GetFields(flags);
            foreach (FieldInfo field in fields)
            {
                if (IsProblematicField(field.Name)) continue;

                try
                {
                    object val = field.GetValue(comp);
                    writer.WriteLine(indent + "    " + field.Name + ": " + FormatValue(val));
                }
                catch
                {
                    writer.WriteLine(indent + "    " + field.Name + ": [erreur lecture]");
                }
            }

            if (includeProperties)
            {
                PropertyInfo[] props = comp.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in props)
                {
                    if (!prop.CanRead) continue;
                    if (IsProblematicProperty(prop.Name)) continue;

                    try
                    {
                        object val = prop.GetValue(comp);
                        writer.WriteLine(indent + "    [prop] " + prop.Name + ": " + FormatValue(val));
                    }
                    catch { }
                }
            }
        }

        if (scanChildren)
        {
            foreach (Transform child in obj.transform)
                ScanObject(child.gameObject, writer, depth + 1);
        }
    }

    bool IsProblematicField(string name)
    {
        string[] bad = { "mesh", "sharedMesh", "material", "materials", "sharedMaterial", "sharedMaterials" };
        foreach (string b in bad)
            if (name == b) return true;
        return false;
    }

    bool IsProblematicProperty(string name)
    {
        string[] bad = {
            "mesh", "material", "materials", "sharedMesh", "sharedMaterial", "sharedMaterials",
            "sprite", "texture", "mainTexture", "vertices", "triangles", "normals", "uv",
            "bones", "bindposes", "terrainData", "audioClip", "clip"
        };
        foreach (string b in bad)
            if (name == b) return true;
        return false;
    }

    string FormatVec3(Vector3 v)
    {
        return string.Format("({0:F2}, {1:F2}, {2:F2})", v.x, v.y, v.z);
    }

    string FormatValue(object val)
    {
        if (val == null) return "NULL";

        if (val is UnityEngine.Object uObj)
        {
            if (uObj == null) return "NULL (reference manquante)";
            return uObj.name + " (" + uObj.GetType().Name + ")";
        }

        if (val is Vector3 v3) return FormatVec3(v3);
        if (val is Vector2 v2) return string.Format("({0:F2}, {1:F2})", v2.x, v2.y);
        if (val is Quaternion q) return FormatVec3(q.eulerAngles);
        if (val is Color c) return string.Format("RGBA({0:F2},{1:F2},{2:F2},{3:F2})", c.r, c.g, c.b, c.a);
        if (val is System.Array arr) return "[Array " + arr.GetType().GetElementType().Name + ", Count: " + arr.Length + "]";

        string str = val.ToString();
        if (str.Length > 150)
            str = str.Substring(0, 150) + "...";

        return str;
    }
}