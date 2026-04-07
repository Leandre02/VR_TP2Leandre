using UnityEngine;
using System.Collections;

public class GestionnaireSpawn : MonoBehaviour
{
    public GameObject ciblePrefab;
    public float tempsAvantDisparition = 2f;
    public float delaiEntreSpawns = 1.5f;

    // Les positions où les cibles peuvent apparaître
    public Transform[] pointsDeSpawn;

    void Start()
    {
       
    }

    public void DemarrerSpawn()
    {
        StartCoroutine(SpawnCibles());
    }

    public void ArreterSpawn()
    {
        StopAllCoroutines();
    }

    IEnumerator SpawnCibles()
    {
        while (true)
        {
            // Choisir un point de spawn au hasard
            int index = Random.Range(0, pointsDeSpawn.Length);
            Transform point = pointsDeSpawn[index];

            // Créer la cible
            GameObject cible = Instantiate(ciblePrefab,
                                           point.position,
                                           Quaternion.identity);

            // La détruire après un délai si pas frappée
            Destroy(cible, tempsAvantDisparition);

            yield return new WaitForSeconds(delaiEntreSpawns);
        }
    }
}