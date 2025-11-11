using UnityEngine;

/// <summary>
/// Represente le tir d un canon 
/// Definit la sequence de tir avant chaque nouvel instance de nouvel balle 
/// Source : https://www.youtube.com/watch?v=6oDjyMqWYiI
/// </summary>
public class TirCanon : MonoBehaviour
{
    public GameObject prefabProjectile; // Le prefab du projectile à instancier
    public Transform sortie;            // Le point de sortie du canon
    public float cadence = 6f;          // Tirs par seconde

    private float delaiAvantProchainTir = 0f; // Temps restant avant de pouvoir tirer à nouveau

    void Update()
    {
        delaiAvantProchainTir -= Time.deltaTime;

        // Clique gauche pour tirer
        if (Input.GetMouseButton(0) && delaiAvantProchainTir <= 0f && prefabProjectile && sortie)
        {
            delaiAvantProchainTir = 1f / Mathf.Max(0.01f, cadence);
            Instantiate(prefabProjectile, sortie.position, sortie.rotation);
        }
    }
}
