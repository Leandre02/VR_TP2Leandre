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

    [SerializeField] private float vitesseBase = 10f;
    private float multiplicateurVitesse = 1f;

    void Start()
    {
        StartCoroutine(Generateur());
    }
    public void ChangerVitesse(float valeurSlider)
    {
        multiplicateurVitesse = valeurSlider;
    }

    private void Update()
    {
        float vitesseReelle = vitesseBase * multiplicateurVitesse;
      
    }

    private IEnumerator Generateur()
    {
        for (int i = 0; i < quantite; i++)
        {
            Vector3 cible = cibleVisee ? cibleVisee.position : transform.position;
            Vector2 offset2D = Random.insideUnitCircle * rayonZone;
            Vector3 pointAleatoire = new Vector3(cible.x + offset2D.x, cible.y, cible.z + offset2D.y);
            Vector3 positionSpawn = pointAleatoire + Vector3.up * 30f;

            GameObject meteore = Instantiate(prefabMeteorite, positionSpawn, Quaternion.identity);


            Destroy(meteore, 15f);

            yield return new WaitForSeconds(delaiEntreSpawns);
        }
    }
}

