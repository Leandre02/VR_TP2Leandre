using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represente la gestion des quêtes dans le jeu.
/// </summary>
public class GestionQuetes : MonoBehaviour
{
    public static GestionQuetes Instance;
    public List<Quete> quetesActives = new List<Quete>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Precharge quelques quêtes au démarrage du jeu.
    /// </summary>
    void Start()
    {
        Quete q = new Quete();
        q.nomQuete = "Ma première quête !";
        q.description = "Traverser la vallée avant la fin du temps imparti.";
        q.nomObjectif = "Traverser la vallée";
        q.nombreRequis = 1; 
        AjouterQuete(q);

        Quete q2 = new Quete();
        q2.nomQuete = "Ma deuxième quête !";
        q2.description = "Va chercher la clé pour déverouiller la porte";
        q2.nomObjectif = "chercher la Clé";
        q2.nombreRequis = 1;
        AjouterQuete(q2);

        Quete q3 = new Quete();
        q3.nomQuete = "Ma troisième quête !";
        q3.description = "Tue 5 monstres.";
        q3.nomObjectif = "tuerMonstres";
        q3.nombreRequis = 5;
        AjouterQuete(q3);

    }

    /// <summary>
    /// Méthode pour ajouter une quête à la liste des quêtes actives.
    /// </summary>
    /// <param name="quete">La quête à ajouter</param>
    public void AjouterQuete(Quete quete)
    {
        foreach (var q in quetesActives)
        {
            if (q.nomQuete == quete.nomQuete)
            {
                Debug.LogWarning("La quête " + quete.nomQuete + " est déjà active.");
                return;
            }
        }
        quetesActives.Add(quete);
        InterfaceQuete.Instance.MettreAJourAffichage(quetesActives);
    }

    /// <summary>
    /// Méthode pour mettre à jour la progression des quêtes en fonction d'un objectif accompli.
    /// </summary>
    /// <param name="nomObjectif">Le nom de l'objectif accompli.</param>
    public void ProgressionQuete(string nomObjectif)
    {
        foreach (var quete in quetesActives)
            quete.MettreAJourProgression(nomObjectif);
        InterfaceQuete.Instance.MettreAJourAffichage(quetesActives);
    }
}
