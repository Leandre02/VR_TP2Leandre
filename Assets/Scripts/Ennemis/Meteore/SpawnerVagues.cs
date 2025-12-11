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
    public float delaiEntreSpawns = 2f; // Délai entre chaque spawn

    [SerializeField] private float dureeVieMeteorite = 15f; // Temps avant destruction auto


    void Start()
    {
        StartCoroutine(Generateur());
    }

    public void ChangerDifficulte(float valeurSlider)
    {
        delaiEntreSpawns = 3.5f - valeurSlider;
        Debug.Log("[SpawnerVagues] Nouveau délai: " + delaiEntreSpawns + "s");
    }

    /// <summary>
    /// Coroutine qui génère des météorites à intervalles réguliers
    /// </summary>
    private IEnumerator Generateur()
    {
        while (true)
        {
            Vector3 cible = cibleVisee ? cibleVisee.position : transform.position;
            Vector2 offset2D = Random.insideUnitCircle * rayonZone;
            Vector3 pointAleatoire = new Vector3(cible.x + offset2D.x, cible.y, cible.z + offset2D.y);
            Vector3 positionSpawn = pointAleatoire + Vector3.up * 30f;

            GameObject meteore = Instantiate(prefabMeteorite, positionSpawn, Quaternion.identity);

            Destroy(meteore, dureeVieMeteorite);
            yield return new WaitForSeconds(delaiEntreSpawns);
        }
    }
}

