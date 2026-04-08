using UnityEngine;
using TMPro;

/// <summary>
/// Classe centrale pour gérer le déroulement du jeu : démarrage, fin, score, minuterie, et transitions entre les différents panneaux UI
/// </summary>
public class GestionnaireJeu : MonoBehaviour
{
    // Singleton de GestionnaireJeu pour permettre à d'autres scripts d'y accéder facilement
    public static GestionnaireJeu instance;

    [Header("Durée")]
    [SerializeField] private float dureePartie = 60f;

    [Header("UI - Panneau Jeu")]
    [SerializeField] private TextMeshProUGUI texteScore;
    [SerializeField] private TextMeshProUGUI texteMinuterie;

    [Header("UI - Panneaux")]
    [SerializeField] private GameObject panneauMenu;
    [SerializeField] private GameObject panneauJeu;
    [SerializeField] private GameObject panneauFin;

    [Header("UI - Fin de partie")]
    [SerializeField] private TextMeshProUGUI texteScoreFinal;

    [Header("Références")]
    [SerializeField] private GestionnaireSpawn gestionnaireSpawn;

    private int score = 0;
    private float tempsRestant;
    private bool partieEnCours = false;

    void Awake()
    {
        instance = this;

        // État initial — on montre le menu
        panneauMenu.SetActive(true);
        panneauJeu.SetActive(false);
        panneauFin.SetActive(false);
    }

    void OnEnable()
    {
        // S'abonne à l'event C# de Cible
        Cible.OnCibleTouchee += AjouterPoints;
    }

    void OnDisable()
    {
        // Se désabonne pour éviter les fuites mémoire
        Cible.OnCibleTouchee -= AjouterPoints;
    }

    /// <summary>
    /// Méthode pour démarrer la partie, appelée par le bouton "Jouer" du menu
    /// </summary>
    public void CommencerPartie()
    {
        score = 0;
        tempsRestant = dureePartie;
        partieEnCours = true;

        panneauMenu.SetActive(false);
        panneauJeu.SetActive(true);
        panneauFin.SetActive(false);

        gestionnaireSpawn.DemarrerSpawn();
        MettreAJourUI();
    }

    void Update()
    {
        if (!partieEnCours) return;

        tempsRestant -= Time.deltaTime;
        texteMinuterie.text = Mathf.CeilToInt(tempsRestant).ToString();

        if (tempsRestant <= 0)
        {
            TerminerPartie();
        }
    }

    /// <summary>
    /// Appelée via l'event OnCibleTouchee
    /// Permet d'ajouter les points gagnés à chaque fois qu'une cible est touchée, et de mettre à jour l'UI
    /// </summary>
    /// <param name="pointsGagnes"></param>
    public void AjouterPoints(int pointsGagnes)
    {
        if (!partieEnCours) return;

        score += pointsGagnes;
        MettreAJourUI();
    }

    /// <summary>
    /// Permet de terminer la partie, appelée quand le temps est écoulé
    /// </summary>
    void TerminerPartie()
    {
        partieEnCours = false;
        gestionnaireSpawn.ArreterSpawn();

        panneauJeu.SetActive(false);
        panneauFin.SetActive(true);
        texteScoreFinal.text = "Score : " + score;
    }

    /// <summary>
    /// Permet de recommencer une nouvelle partie, appelée par le bouton "Rejouer" du panneau de fin
    /// </summary>
    public void NouvellePartie()
    {
        CommencerPartie();
    }

    /// <summary>
    /// Permet de mettre à jour l'affichage du score et du temps restant dans l'UI pendant la partie
    /// </summary>
    void MettreAJourUI()
    {
        texteScore.text = "Score : " + score;
    }
}