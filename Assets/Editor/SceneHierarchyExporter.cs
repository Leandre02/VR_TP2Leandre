#if UNITY_EDITOR
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Script pour exporter la hierarchie complete de la scene avec tous les details UI
// Cree par un etudiant pour debugger son UI Unity
public class SceneHierarchyExporter : EditorWindow
{
    [MenuItem("Tools/Exporter Hiérarchie UI Complète")]
    public static void ExportSceneHierarchy()
    {
        string path = EditorUtility.SaveFilePanel("Enregistrer la hiérarchie UI", "", "UIHierarchy_Complete.txt", "txt");
        
        if (string.IsNullOrEmpty(path))
        {
            // l'utilisateur a annule, on fait rien
            return;
        }
        else
        {
            // on commence l'export
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine("=== EXPORT HIERARCHIE UI UNITY ===");
                writer.WriteLine("Date: " + System.DateTime.Now.ToString());
                writer.WriteLine("Scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                writer.WriteLine("");
                
                // on parcours tous les objets racine de la scene
                foreach (GameObject obj in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                {
                    EcrireObjetRecursif(obj, writer, 0);
                }
            }

            EditorUtility.DisplayDialog("Export terminé", "Hiérarchie UI exportée dans :\n" + path, "OK");
        }
    }

    // fonction recursive pour ecrire un GameObject et ses enfants
    static void EcrireObjetRecursif(GameObject obj, StreamWriter writer, int profondeur)
    {
        // indentation pour montrer la hierarchie
        string indent = new string(' ', profondeur * 2);
        
        // nom + etat actif/inactif (SUPER IMPORTANT pour UI)
        writer.Write(indent + "- " + obj.name);
        
        if (obj.activeSelf)
        {
            writer.Write(" [ACTIF]");
        }
        else
        {
            writer.Write(" [INACTIF]"); // souvent la cause des bugs UI !
        }
        
        writer.WriteLine();

        // liste tous les composants
        Component[] composants = obj.GetComponents<Component>();
        
        foreach (Component comp in composants)
        {
            if (comp == null)
            {
                continue; // composant manquant (script supprime par ex)
            }
            else
            {
                writer.WriteLine(indent + "  → " + comp.GetType().Name);
                
                // details specifiques pour les composants UI importants
                EcrireDetailsComposant(comp, writer, indent + "    ");
            }
        }

        // on traite les enfants recursivement
        foreach (Transform enfant in obj.transform)
        {
            EcrireObjetRecursif(enfant.gameObject, writer, profondeur + 1);
        }
    }

    // fonction pour ecrire les details importants de chaque type de composant
    static void EcrireDetailsComposant(Component comp, StreamWriter writer, string indent)
    {
        // Canvas settings (critique pour comprendre le rendering)
        if (comp is Canvas)
        {
            Canvas canvas = comp as Canvas;
            writer.WriteLine(indent + "Render Mode: " + canvas.renderMode);
            
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                if (canvas.worldCamera == null)
                {
                    writer.WriteLine(indent + "Camera: NONE (BUG PROBABLE!)"); // erreur commune
                }
                else
                {
                    writer.WriteLine(indent + "Camera: " + canvas.worldCamera.name);
                }
            }
            else
            {
                // pas de camera necessaire pour overlay
            }
            
            writer.WriteLine(indent + "Sort Order: " + canvas.sortingOrder);
        }
        else if (comp is CanvasScaler)
        {
            CanvasScaler scaler = comp as CanvasScaler;
            writer.WriteLine(indent + "UI Scale Mode: " + scaler.uiScaleMode);
            writer.WriteLine(indent + "Reference Resolution: " + scaler.referenceResolution);
        }
        else if (comp is Button)
        {
            Button bouton = comp as Button;
            
            if (bouton.interactable)
            {
                writer.WriteLine(indent + "Interactable: OUI");
            }
            else
            {
                writer.WriteLine(indent + "Interactable: NON (desactive!)");
            }
            
            // on affiche les listeners du bouton (super utile!)
            int nbListeners = bouton.onClick.GetPersistentEventCount();
            writer.WriteLine(indent + "OnClick listeners: " + nbListeners);
            
            for (int i = 0; i < nbListeners; i++)
            {
                Object cible = bouton.onClick.GetPersistentTarget(i);
                string methode = bouton.onClick.GetPersistentMethodName(i);
                
                if (cible == null)
                {
                    writer.WriteLine(indent + "  [" + i + "] MISSING REFERENCE!"); // bug classique
                }
                else
                {
                    writer.WriteLine(indent + "  [" + i + "] " + cible.name + "." + methode + "()");
                }
            }
        }
        else if (comp is TextMeshProUGUI)
        {
            TextMeshProUGUI texte = comp as TextMeshProUGUI;
            writer.WriteLine(indent + "Text: \"" + texte.text + "\"");
            writer.WriteLine(indent + "Font: " + (texte.font != null ? texte.font.name : "NONE"));
            writer.WriteLine(indent + "Font Size: " + texte.fontSize);
        }
        else if (comp is Text)
        {
            // old UI Text (legacy, mais des fois les etudiants l'utilisent encore)
            Text texte = comp as Text;
            writer.WriteLine(indent + "Text: \"" + texte.text + "\"");
            writer.WriteLine(indent + "Font Size: " + texte.fontSize);
        }
        else if (comp is Image)
        {
            Image img = comp as Image;
            
            if (img.sprite == null)
            {
                writer.WriteLine(indent + "Sprite: NONE"); // peut etre normal ou un bug
            }
            else
            {
                writer.WriteLine(indent + "Sprite: " + img.sprite.name);
            }
            
            writer.WriteLine(indent + "Raycast Target: " + img.raycastTarget); // important pour les clicks
        }
        else if (comp is RectTransform)
        {
            RectTransform rect = comp as RectTransform;
            writer.WriteLine(indent + "Anchors: Min(" + rect.anchorMin + ") Max(" + rect.anchorMax + ")");
            writer.WriteLine(indent + "Position: " + rect.anchoredPosition);
            writer.WriteLine(indent + "Size: " + rect.sizeDelta);
        }
        else
        {
            // pour les scripts custom, on essaie d'afficher les champs publics
            EcrireChampsSerialized(comp, writer, indent);
        }
    }

    // fonction pour afficher les champs serialized des scripts custom (SUPER UTILE!)
    static void EcrireChampsSerialized(Component comp, StreamWriter writer, string indent)
    {
        SerializedObject so = new SerializedObject(comp);
        SerializedProperty prop = so.GetIterator();
        
        bool aDesProprietes = false;
        
        // on parcours toutes les proprietes serialisees
        if (prop.NextVisible(true))
        {
            do
            {
                // on skip m_Script qui est toujours la
                if (prop.name == "m_Script")
                {
                    continue;
                }
                else
                {
                    if (!aDesProprietes)
                    {
                        writer.WriteLine(indent + "Propriétés:");
                        aDesProprietes = true;
                    }
                    else
                    {
                        // on continue a afficher
                    }
                    
                    // on affiche selon le type
                    if (prop.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if (prop.objectReferenceValue == null)
                        {
                            writer.WriteLine(indent + "  " + prop.name + ": NONE (reference manquante?)");
                        }
                        else
                        {
                            writer.WriteLine(indent + "  " + prop.name + ": " + prop.objectReferenceValue.name);
                        }
                    }
                    else if (prop.propertyType == SerializedPropertyType.String)
                    {
                        writer.WriteLine(indent + "  " + prop.name + ": \"" + prop.stringValue + "\"");
                    }
                    else if (prop.propertyType == SerializedPropertyType.Integer)
                    {
                        writer.WriteLine(indent + "  " + prop.name + ": " + prop.intValue);
                    }
                    else if (prop.propertyType == SerializedPropertyType.Float)
                    {
                        writer.WriteLine(indent + "  " + prop.name + ": " + prop.floatValue);
                    }
                    else if (prop.propertyType == SerializedPropertyType.Boolean)
                    {
                        writer.WriteLine(indent + "  " + prop.name + ": " + prop.boolValue);
                    }
                    else
                    {
                        // autre type, on affiche juste le nom
                        writer.WriteLine(indent + "  " + prop.name + ": (" + prop.propertyType + ")");
                    }
                }
            }
            while (prop.NextVisible(false));
        }
        else
        {
            // pas de proprietes, c'est ok
        }
    }
}
#endif