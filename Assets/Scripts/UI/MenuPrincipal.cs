using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Gere le menu principal et la pause du jeu
/// </summary>
public class MenuPrincipal : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject gestionCanvas;

    [Header("References Systeme")]
    [SerializeField] private PlayerInput playerInput; // Le PlayerInput du SystemeJeu

    [Header("Panels Options")]
    [SerializeField] private GameObject panelOptions;
    [SerializeField] private GameObject panelParam;
    [SerializeField] private GameObject fenetreMenu;

    // Events pour notifier le changement d'etat
    public System.Action<bool> OnChangementEtatMenu;

    private bool menuOuvert = true;

    private static bool demarrerSansMenu = false; // Indique si le jeu doit demarrer sans menu

    void Awake()
    {
        if (demarrerSansMenu)
        {
            
            demarrerSansMenu = false; // Reset pour les prochaines fois
            FermerMenu();
        }
        else
        {
            OuvrirMenu(); // Commence avec le menu ouvert
        }
    }

    
    public void BoutonDemarrer()
    {
        RedemarrerJeu();
    }

    public void BoutonContinuer()
    {
        FermerMenu();
    }

    public void BoutonOptions()
    {
        if (panelOptions != null)
        {
            panelOptions.SetActive(true);
        }
        else
        {
            Debug.LogWarning("PanelOptions non assigne");
        }
        if (fenetreMenu != null)
        {
            fenetreMenu.SetActive(false);
        }
        else
        {
            Debug.LogWarning("FenetreMenu non assigne");
        }
    }

    public void BoutonParametres()
    {
        if (panelParam != null)
        {
            panelParam.SetActive(true);
        }
        else
        {
            Debug.LogWarning("PanelParam non assigne");
        }
        if (fenetreMenu != null)
        {
            fenetreMenu.SetActive(false);
        }
        else
        {
            Debug.LogWarning("FenetreMenu non assigne");
        }
    }

    public void BoutonRetourMenu()
    {
        // ferme les panels
        if (panelOptions != null)
        {
            panelOptions.SetActive(false);
        }

        if (panelParam != null)
        {
            panelParam.SetActive(false);
        }

        // reaffiche le menu principal
        if (fenetreMenu != null)
        {
            fenetreMenu.SetActive(true);
        }
        else
        {
            Debug.LogWarning("FenetreMenu non assigne");
        }
    }

    public void OuvrirMenu()
    {
        menuOuvert = true;

        if (menuCanvas) menuCanvas.SetActive(true);
        if (gestionCanvas) gestionCanvas.SetActive(false);

        // Desactive les inputs du jeu
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Notifie les autres systemes
        OnChangementEtatMenu?.Invoke(true);

        // Joue la musique du menu
        if (GestionAudio.Instance != null)
        {
            GestionAudio.Instance.JouerMusiqueMenu();
        }
    }

    public void FermerMenu()
    {
        menuOuvert = false;

        if (menuCanvas) menuCanvas.SetActive(false);
        if (gestionCanvas) gestionCanvas.SetActive(true);

        // Cache les panels d'options
        if (panelOptions) panelOptions.SetActive(false);
        if (panelParam) panelParam.SetActive(false);

        // Reactive les inputs du jeu
        if (playerInput != null)
        {
            playerInput.enabled = true;
        }

        Time.timeScale = 1f;
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;


        // Notifie les autres systemes
        OnChangementEtatMenu?.Invoke(false);

        // Joue la musique du jeu
        if (GestionAudio.Instance != null)
        {
            GestionAudio.Instance.JouerMusiqueJeu();
        }
    }

    public bool EstMenuOuvert()
    {
        return menuOuvert;
    }

    public void RedemarrerJeu()
    {

        // On s'assure que le temps est normal avant de recharger
        Time.timeScale = 1f;

        // On demande au prochain chargement de scène de démarrer sans menu
        demarrerSansMenu = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}