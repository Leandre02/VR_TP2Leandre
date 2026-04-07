using UnityEngine;
using TMPro;

public class GestionnaireJeu : MonoBehaviour
{
    public static GestionnaireJeu instance;

    public float dureePartie = 60f;
    public TextMeshProUGUI texteScore;
    public TextMeshProUGUI texteMinuterie;
    public GameObject panneauJeu;
    public GameObject panneauFin;
    public TextMeshProUGUI texteScoreFinal;
    public GameObject panneauMenu;
    public GestionnaireSpawn gestionnaireSpawn;

    private int score = 0;
    private float tempsRestant;
    private bool partieEnCours = false;

    void Awake()
    {
        instance = this;
        panneauMenu.SetActive(true);
        panneauJeu.SetActive(false);
        panneauFin.SetActive(false);
    }

    void OnEnable()
    {
        Cible.OnCibleTouchee += AjouterPoints;
    }

    void OnDisable()
    {
        Cible.OnCibleTouchee -= AjouterPoints;
    }

    public void CommencerPartie()
    {
        gestionnaireSpawn.DemarrerSpawn();
        panneauMenu.SetActive(false);
        score = 0;
        tempsRestant = dureePartie;
        partieEnCours = true;
        panneauJeu.SetActive(true);
        panneauFin.SetActive(false);
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

    public void AjouterPoints(int points)
    {
        if (!partieEnCours) return;
        score += points;
        MettreAJourUI();
    }

    void TerminerPartie()
    {
        gestionnaireSpawn.ArreterSpawn();
        partieEnCours = false;
        panneauJeu.SetActive(false);
        panneauFin.SetActive(true);
        texteScoreFinal.text = "Score : " + score;
    }

    public void NouvellePartie()
    {
        CommencerPartie();
    }

    void MettreAJourUI()
    {
        texteScore.text = "Score : " + score;
    }
}