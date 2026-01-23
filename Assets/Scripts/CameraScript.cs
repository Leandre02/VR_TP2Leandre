using UnityEngine;

/// <summary>
/// Une methode pour placer des cubes colores dans un espace 3D.
/// </summary>
public class CameraScript : MonoBehaviour
{
    [Header("Placement")]
    public GameObject cubePrefab; // Prefab du cube a instancier


    // Update is called once per frame
    void Update()
    {
         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Clic gauche pour placer un cube
            if (Input.GetMouseButtonDown(0))
            {
                PlaceCube(hit.point);
            }

            // Clic droit pour supprimer un cube
            if (Input.GetMouseButtonDown(1))
            {
                if (hit.collider != null && hit.collider.gameObject.CompareTag("Cube"))
                {
                    Destroy(hit.collider.gameObject);
                }
            }
        }

    }

    /// <summary>
    /// Une methode pour placer un cube colore a la position donnee
    /// </summary>
    /// <param name="position">La position de mon cube</param>
    public void PlaceCube(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit; // Variable pour stocker les informations du raycast
        if (Physics.Raycast(ray, out hit))
        {
            // Instancie le cube
            GameObject newCube = Instantiate(cubePrefab, hit.point, Quaternion.identity);

        }
    }
}
