using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;



/// <summary>
/// Classe centrale pour g�rer le d�roulement du jeu : d�marrage, fin, score, minuterie, et transitions entre les diff�rents panneaux UI
/// S�appuie sur les patterns de GameManager et d�UI en World Space vus dans les notes et exercices (UI VR, GameManager)
/// et sur les consignes du travail pratique Whack-a-Mole VR.
/// R�f�rences :
///  - C�gep de Victoriaville. UI en VR. Environnements Immersifs, 2026.
///  - C�gep de Victoriaville. Exercice 4.2 � UI VR et GameManager. Environnements Immersifs, 2026. https://envimmersif-cegepvicto.github.io/exercice_ui_jeu_tri/
///  - C�gep de Victoriaville. Travail pratique � Whack-a-Mole VR. Environnements Immersifs, 2026. https://envimmersif-cegepvicto.github.io/tp_sommatif1_vr/
/// </summary>
public class GestionnaireJeu : MonoBehaviour
{
    // Singleton de GestionnaireJeu pour permettre � d'autres scripts d'y acc�der facilement
    public static GestionnaireJeu instance;

    [Header("Duree")]
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

    [Header("R�ferences")]
    [SerializeField] private GestionnaireSpawn gestionnaireSpawn;

    [Header("Controleurs pour vibration victoire")]
    [SerializeField] private XRBaseInputInteractor controleurGauche;
    [SerializeField] private XRBaseInputInteractor controleurDroit;

    private int score = 0;
    private float tempsRestant;
    private bool partieEnCours = false;

    void Awake()
    {
        instance = this;

        // �tat initial � on montre le menu
        panneauMenu.SetActive(true);
        panneauJeu.SetActive(false);
        panneauFin.SetActive(false);
    }

    void OnEnable()
    {
        // S'abonne � l'event C# de Cible
        Cible.OnCibleTouchee += AjouterPoints;
    }

    void OnDisable()
    {
        // Se d�sabonne pour �viter les fuites m�moire
        Cible.OnCibleTouchee -= AjouterPoints;
    }

    /// <summary>
    /// M�thode pour demarrer la partie, appel�e par le bouton "Jouer" du menu
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
    /// Appelee via l'event OnCibleTouchee
    /// Permet d'ajouter les points gagnes a chaque fois qu'une cible est touch�e, et de mettre � jour l'UI
    /// </summary>
    /// <param name="pointsGagnes"></param>
    public void AjouterPoints(int pointsGagnes)
    {
        if (!partieEnCours) return;

        score += pointsGagnes;
        MettreAJourUI();
    }

    /// <summary>
    /// Permet de terminer la partie, appel�e quand le temps est �coul�
    /// </summary>
    void TerminerPartie()
    {
        // Ajout d'une vibration haptique au moment de la victoire
        if (controleurGauche != null)
            controleurGauche.SendHapticImpulse(1f, 0.5f);
        if (controleurDroit != null)
            controleurDroit.SendHapticImpulse(1f, 0.5f);

        partieEnCours = false;
        gestionnaireSpawn.ArreterSpawn();

        panneauJeu.SetActive(false);
        panneauFin.SetActive(true);
        texteScoreFinal.text = "Score : " + score;
    }

    /// <summary>
    /// Permet de recommencer une nouvelle partie, appel�e par le bouton "Rejouer" du panneau de fin
    /// </summary>
    public void NouvellePartie()
    {
        CommencerPartie();
    }

    /// <summary>
    /// Permet de mettre a jour l'affichage du score et du temps restant dans l'UI pendant la partie
    /// </summary>
    void MettreAJourUI()
    {
        texteScore.text = "Score : " + score;
    }
}