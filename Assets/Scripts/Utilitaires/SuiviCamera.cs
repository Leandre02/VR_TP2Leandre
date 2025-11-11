using UnityEngine;

/// <summary>
/// Suivi intelligent de ma camera sur mon tank
/// Code inspiré de : https://gist.github.com/Hamcha/6096905 
/// @auteur : Alessandro Gatti 
/// </summary>
public class SuiviCamera : MonoBehaviour
{
    public Transform cible;
    public float distance = 10f;
    public float hauteur = 5f;

    void LateUpdate()
    {
        if (!cible) return;

        // Place la caméra directement derrière et au-dessus du tank
        Quaternion rotation = Quaternion.Euler(0, cible.eulerAngles.y, 0);
        Vector3 position = cible.position - rotation * Vector3.forward * distance;
        position.y = cible.position.y + hauteur;

        transform.position = position;
        transform.LookAt(cible);
    }
}
