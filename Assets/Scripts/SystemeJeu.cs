using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Gere le systeme de jeu central et distribue les events
/// Code inspiré de :https://www.youtube.com/watch?v=_Odo5C436hU
/// </summary>
public class SystemeJeu : MonoBehaviour
{
    private static SystemeJeu instance;
    public static SystemeJeu Instance => instance;

    [Header("References")]
    [SerializeField] private MenuPrincipal menuPrincipal;

    private PlayerInput playerInput;

    // Events pour le deplacement
    public System.Action<Vector2> OnDeplacement;
    public System.Action<bool> OnTir;

    public System.Action OnToggleJournal;
    public System.Action<bool> OnPauseMenu;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        playerInput = GetComponent<PlayerInput>();
    }


    void Start()
    {
        // Demarre avec la musique du menu
        if (GestionAudio.Instance != null)
        {
            GestionAudio.Instance.JouerMusiqueMenu();
        }
    }

    /// <summary>
    /// Appele par le PlayerInput
    /// </summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 mouvement = context.ReadValue<Vector2>();
        OnDeplacement?.Invoke(mouvement);
    }

    /// <summary>
    /// Appele par le PlayerInput
    /// </summary>
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

            // Bouton pressé
            OnTir?.Invoke(true);
        }
        else if (context.canceled)
        {
            // Bouton relâché
            OnTir?.Invoke(false);
        }
    }

    /// <summary>
    /// Appele par le PlayerInput
    /// </summary>
    public void OnPause(InputAction.CallbackContext context)
    {
        if (menuPrincipal != null)
        {
            if (menuPrincipal.EstMenuOuvert())
            {
                menuPrincipal.FermerMenu();
            }
            else
            {
                menuPrincipal.OuvrirMenu();
            }
        }
    }

    /// <summary>
    /// Appele par le PlayerInput
    /// </summary>
    public void OnJournal(InputAction.CallbackContext context)
    {
        
        if (!context.performed) return;

        OnToggleJournal?.Invoke();
    }
}