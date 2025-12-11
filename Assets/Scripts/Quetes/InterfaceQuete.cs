using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Represente l'interface utilisateur pour afficher les quêtes.
/// NOTE : N'utilise pas d'events pour ouvrir/fermer le journal des quêtes.
/// </summary>
public class InterfaceQuete : MonoBehaviour
{
    public static InterfaceQuete Instance; // Un singleton pour accéder facilement à l'interface des quêtes
    public GameObject prefabQuete; // Le prefab UI pour une quête individuelle
    public Transform parentListeQuetes; // Le parent où les quêtes seront instanciées
    public GameObject panelQuetes; // Le panneau UI du journal des quêtes

    // Charge l'instance
    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        if (GestionQuetes.Instance != null)
        {
            InterfaceQuete.Instance.MettreAJourAffichage(GestionQuetes.Instance.quetesActives);
        }

        // On s'abonne à l'event
        if (SystemeJeu.Instance != null)
        {
            SystemeJeu.Instance.OnToggleJournal += BasculerFenetre;
        }
    }

    void OnDisable()
    {
        if (SystemeJeu.Instance != null)
        {
            SystemeJeu.Instance.OnToggleJournal -= BasculerFenetre;
        }
    }

    /// <summary>
    /// Une methode pour mettre à jour l'affichage des quêtes dans l'interface utilisateur.
    /// </summary>
    /// <param name="quetes">La quete en cours</param>
    public void MettreAJourAffichage(List<Quete> quetes)
    {
        foreach (Transform enfant in parentListeQuetes)
            Destroy(enfant.gameObject);

        foreach (Quete quete in quetes)
        {
            GameObject q = Instantiate(prefabQuete, parentListeQuetes);

            Debug.Log("Tentative de récupération du TMP...");
            var tmpText = q.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpText != null)
            {
                Debug.Log("TMP trouvé : " + tmpText.name);
                tmpText.text = quete.description + " - " +
                    (quete.terminee ? "Terminée" : quete.nombreActuel + "/" + quete.nombreRequis);
            }
            else
            {
                Debug.LogError("TextMeshProUGUI non trouvé dans le prefab QueteUI !");
            }
        }

    }

    /// <summary>
    /// Methode pour ouvrir le panneau des quêtes
    /// </summary>
    public void AfficherFenetre()
    {
        panelQuetes.SetActive(true);
    }

    /// <summary>
    /// Methode pour fermer le panneau des quêtes
    /// </summary>
    public void FermerFenetre()
    {
        panelQuetes.SetActive(false);
        
    }

    /// <summary>
    /// Methode pour basculer l'affichage du panneau des quêtes
    /// </summary>
    public void BasculerFenetre()
    {
        bool actif = panelQuetes.activeSelf;
        panelQuetes.SetActive(!actif);
    }

}
