using UnityEngine;
using System.Collections;

/// <summary>
/// Classe pour gérer le spawn des cibles à des points aléatoires, avec un délai entre chaque spawn, et les faire disparaître après un certain temps
/// </summary>
public class GestionnaireSpawn : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject ciblePrefab;

    [Header("Paramètres de spawn")]
    [SerializeField] private float tempsAvantDisparition = 2f;
    [SerializeField] private float delaiEntreSpawns = 1.5f;
    

    [Header("Points de spawn")]
    [SerializeField] private Transform[] pointsDeSpawn;

    /// <summary>
    /// Appelée par GestionnaireJeu quand la partie commence
    /// </summary>
    public void DemarrerSpawn()
    {
        StartCoroutine(SpawnCibles());
    }

    /// <summary>
    /// Appelée par GestionnaireJeu quand la partie se termine
    /// </summary>
    public void ArreterSpawn()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// Permet de faire spawn des cibles à des points aléatoires, avec un délai entre chaque spawn, et les détruit après un certain temps
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnCibles()
    {
        while (true)
        {
            int index = Random.Range(0, pointsDeSpawn.Length);
            Transform point = pointsDeSpawn[index];

            GameObject cible = Instantiate(ciblePrefab, point.position, Quaternion.identity);
            Destroy(cible, tempsAvantDisparition);

            yield return new WaitForSeconds(delaiEntreSpawns);
        }
    }
}