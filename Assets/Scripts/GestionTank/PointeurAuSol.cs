using UnityEngine;

/// <summary>
/// Systeme de visée pour la tourelle de mon tank
/// Recupere la position exacte sur la carte de ma souris 
/// Source : https://docs.unity3d.com/ScriptReference/Physics.Raycast.html
/// </summary>
public class PointeurAuSol : MonoBehaviour
{
    public Camera cam;   // La caméra utilisée pour générer le rayon à partir de la position de la souris.            
    public LayerMask couchesSol; // définis les surfaces considérées comme "sol".

    void Awake()
    { 
            cam = Camera.main; // Charge la camera principale de la scene
    }

    /// <summary>
    /// Retourne vrai si la souris pointe un endroit au sol, et donne la position.
    /// </summary>
    /// <param name="point">La position exacte sur le sol où la souris pointe, si détectée.</param>
    /// <returns>Vrai si un point au sol est détecté, sinon faux.</returns>
    public bool AvoirPointAuSol(out Vector3 point)
    {
        // Position de la souris sur l'écran en pixels.
        Vector2 ecran = Input.mousePosition;

        // Crée un rayon partant de la caméra passant par la position de la souris.
        Ray ray = cam.ScreenPointToRay(ecran);

        // Effectue un Raycast pour détecter une collision avec les couches spécifiées.
        // <param name="ray">Le rayon généré à partir de la caméra et de la position de la souris.</param>
        // <param name="hit">Contient les informations sur l'objet touché si une collision est détectée.</param>
        // <param name="500f">La distance maximale que le rayon peut parcourir.</param>
        // <param name="couchesSol">Les couches à considérer pour la détection.</param>
        // Source : https://www.youtube.com/watch?v=9D244Wmeya8
        if (Physics.Raycast(ray, out RaycastHit hit, 500f, couchesSol))
        {
            // Si un objet est touché, retourne sa position.
            point = hit.point;
            return true;
        }

        // Si aucun objet n'est touché, retourne un vecteur nul.
        point = Vector3.zero;
        return false;
    }
}
