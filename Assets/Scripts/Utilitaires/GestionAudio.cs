using UnityEngine;

/// <summary>
/// Une classe singleton pour gérer l'audio global du jeu.
/// </summary>
public class GestionAudio : MonoBehaviour
{
    public static GestionAudio Instance { get; private set; }

    [Header("Sources Audio Globales")]
    [SerializeField] private AudioSource sourceMusique;
    [SerializeField] private AudioSource sourceSFX;
    [SerializeField] private AudioSource sourceMoteur;   // dédié au moteur du tank

    [Header("Clips Musique")]
    [SerializeField] private AudioClip musiqueMenu;
    [SerializeField] private AudioClip musiqueJeu;

    [Header("SFX Tank")]
    [SerializeField] private AudioClip sonMoteurIdle;
    [SerializeField] private AudioClip sonMoteurDeplacement;
    [SerializeField] private AudioClip sonTir;
    [SerializeField] private AudioClip sonImpact;

    [Header("Réglages moteur")]
    [SerializeField] private float vitesseMax = 10f;
    [SerializeField] private float pitchMin = 1.0f;
    [SerializeField] private float pitchMax = 1.3f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Une méthode pour jouer la musique de fond du menu.
    /// </summary>
    public void JouerMusiqueMenu()
    {
        if (sourceMusique == null || musiqueMenu == null) return;
        sourceMusique.clip = musiqueMenu;
        sourceMusique.loop = true;
        sourceMusique.Play();
    }

    /// <summary>
    /// Une méthode pour jouer la musique de fond du jeu.
    /// </summary>
    public void JouerMusiqueJeu()
    {
        if (sourceMusique == null || musiqueJeu == null) return;
        sourceMusique.clip = musiqueJeu;
        sourceMusique.loop = true;
        sourceMusique.Play();
    }

    /// <summary>
    /// Une méthode pour jouer le son de tir du tank.
    /// </summary>
    public void JouerTir()
    {
        if (sourceSFX == null || sonTir == null) return;
        sourceSFX.PlayOneShot(sonTir);
    }

    public void JouerImpact()
    {
        if (sourceSFX == null || sonImpact == null) return;
        sourceSFX.PlayOneShot(sonImpact);
    }

    /// <summary>
    /// Appelé depuis ControleTank avec la vitesse actuelle
    /// </summary>
    public void MettreAJourMoteur(float vitesseActuelle)
    {
        if (sourceMoteur == null) return;

        // Si rien ne joue, on démarre le moteur (idle)
        if (!sourceMoteur.isPlaying)
        {
            sourceMoteur.clip = sonMoteurIdle != null ? sonMoteurIdle : sonMoteurDeplacement;
            sourceMoteur.loop = true;
            sourceMoteur.Play();
        }

        /* On interpole le pitch en fonction de la vitesse
         * Code inspiré de : copilot by GitHub
         * */
        float t = Mathf.Clamp01(vitesseActuelle / Mathf.Max(0.01f, vitesseMax));
        float pitch = Mathf.Lerp(pitchMin, pitchMax, t);
        sourceMoteur.pitch = pitch;

        // Fin code inspirée
    }
}
