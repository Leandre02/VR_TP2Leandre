using UnityEngine;
using System.Collections;

/// <summary>
/// Une classe qui applique la generation successive de meteorite
/// Instantie un nouvel objet meteorite a chaque spawn
/// </summary>
public class SpawnerVagues : MonoBehaviour
{
    // Source : https://www.youtube.com/watch?v=SELTWo1XZ0c

    public GameObject prefabMeteorite; // Le prefab de la météorite à instancier
    public Transform cibleVisee;       // La cible visée
    public float rayonZone;      // Rayon de dispersion autour du centre
    public int quantite;          // Nombre de météorites à faire tomber
    public float delaiEntreSpawns; // Délai entre chaque spawn

    void Start()
    {
        StartCoroutine(Spawn());
    }

   private IEnumerator Spawn()
    {
        for (int i = 0; i < quantite; i++)
        {
            Vector3 cible = cibleVisee ? cibleVisee.position : transform.position; // Recalcul la position de ma cibleVisee a chaque spawn
            Vector3 positionSpawn = new Vector3(cible.x, cible.y + 30f, cible.z); // La position d'impact

            // Instancie la météorite
            Instantiate(prefabMeteorite, positionSpawn, Quaternion.identity);

            yield return new WaitForSeconds(delaiEntreSpawns); // retoune le temps entre chaque spawns

        }
    }
}
