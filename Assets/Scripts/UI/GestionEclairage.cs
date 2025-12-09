using UnityEngine;

/// <summary>
/// Une classe pour gerer le changement de l'éclairage
/// </summary>
public class GestionEclairage : MonoBehaviour
{
    public Light lumiereDirectionnelle;

    [SerializeField] private float intensiteMin = 0.2f; // Intensite minimale (nuit)
    [SerializeField] private float intensiteMax = 2f;

    public Material skyboxJour;   // Skybox pour le jour
    public Material skyboxNuit;   // Skybox pour la nuit

    private bool estJour = true;

    void Start()
    {
        if (lumiereDirectionnelle == null)
        {
            Debug.LogError("Lumiere directionnelle manquante!");
        }
    }

    public void ChangerIntensiteLumiere(float valeurSlider)
    {
        if (lumiereDirectionnelle != null)
        {
            // Convertit la valeur du slider (0-1) en intensite
            lumiereDirectionnelle.intensity = Mathf.Lerp(intensiteMin, intensiteMax, valeurSlider);

            Debug.Log("[GestionEclairage] Nouvelle intensite: " + lumiereDirectionnelle.intensity);
        }

        if (valeurSlider < 0.5f && estJour)
        {
            // Passe en mode nuit
            ChangerSkybox(skyboxNuit);
            estJour = false;
            Debug.Log("[GestionEclairage] Mode NUIT active");
        }
        else if (valeurSlider >= 0.5f && !estJour)
        {
            // Passe en mode jour
            ChangerSkybox(skyboxJour);
            estJour = true;
            Debug.Log("[GestionEclairage] Mode JOUR active");
        }
    }

    private void ChangerSkybox(Material nouveauSkybox)
    {
        if (nouveauSkybox != null)
        {
            RenderSettings.skybox = nouveauSkybox;
            // Met à jour le skybox
            DynamicGI.UpdateEnvironment();
        }
    }
}
