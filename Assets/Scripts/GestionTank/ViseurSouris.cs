using UnityEngine;

/// <summary>
/// Represente le viseur de mon canon 
/// Utilise le recttransform pour representer un rectangle dans le plan de mon interface
/// Utilise l image d un viseur definit par ma texture image de mon gameplay pour remplacer le curseur standard de ma souris
/// </summary>
public class ViseurSouris : MonoBehaviour
{
    // Source : https://docs.unity3d.com/6000.0/Documentation/ScriptReference/RectTransform.html
    private RectTransform rectTransform; // Le rectangle pour le viseur de ma souris sur le plan

    void Awake()
    {
        // Récupère la composante RectTransform du gameObject
        rectTransform = GetComponent<RectTransform>();
        Cursor.visible = false; // Pour cacher le curseur de ma souris
    }

    void Update()
    {
        // Récupère la position de la souris dans l'écran (pixels)
        Vector2 positionSouris = Input.mousePosition;

        // Place le viseur à la position de la souris
        rectTransform.position = positionSouris;
    }
}
