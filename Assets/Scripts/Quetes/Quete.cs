using UnityEngine;

/// <summary>
/// Represente une quête avec un objectif à atteindre
/// </summary>
[System.Serializable]
public class Quete
{
    public string nomQuete; // Le nom de la quête
    public string description; // La description de la quête
    public string nomObjectif; // Le nom de l'objectif à atteindre
    public int nombreRequis = 1; // Le nombre requis pour compléter l'objectif
    public int nombreActuel = 0; // Le nombre actuel atteint
    public bool terminee = false; // Indique si la quête est terminée

    /// <summary>
    /// Une méthode pour mettre à jour la progression de la quête
    /// </summary>
    /// <param name="nom">Le nom de la quete</param>
    public void MettreAJourProgression(string nom)
    {
        if (terminee) return;
        if (nom == nomObjectif)
        {
            nombreActuel++;
            if (nombreActuel >= nombreRequis)
            {
                terminee = true;
                Debug.Log("Quête terminée : " + nomQuete);
            }
            else
            {
                Debug.Log($"Progression : {nomQuete} ({nombreActuel}/{nombreRequis})");
            }
        }
    }
}
