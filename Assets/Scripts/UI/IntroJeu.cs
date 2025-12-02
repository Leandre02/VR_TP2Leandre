using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class IntroJeu : MonoBehaviour
{
    [Header("Références UI")]
    [SerializeField] private GameObject panelIntro;
    [SerializeField] private TextMeshProUGUI texteIntro;

    [Header("Système")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private MenuPrincipal menuPrincipal;
    [SerializeField] private GameObject sliderVitesse;

    [TextArea(2, 4)]
    [SerializeField] private string[] messagesIntro;

    private int indexCourant = 0;
    private bool introDejaJouee = false;

    void Awake()
    {
        if (panelIntro != null)
            panelIntro.SetActive(false);
    }

    void OnEnable()
    {
        if (menuPrincipal != null)
        {
            menuPrincipal.OnChangementEtatMenu += SurChangementEtatMenu;
        }
    }

    void OnDisable()
    {
        if (menuPrincipal != null)
        {
            menuPrincipal.OnChangementEtatMenu -= SurChangementEtatMenu;
        }
    }

    private void SurChangementEtatMenu(bool menuOuvert)
    {
        // Quand le menu se ferme pour la première fois, on lance l'intro
        if (!menuOuvert && !introDejaJouee)
        {
            StartCoroutine(LancerIntroApresDelai(1.5f));
        }
    }

    private IEnumerator LancerIntroApresDelai(float delai)
    {
        // delai avant de lancer l'intro
        yield return new WaitForSeconds(delai);
        LancerIntro();
    }

    private void LancerIntro()
    {
        if (messagesIntro == null || messagesIntro.Length == 0) return;

        introDejaJouee = true;
        indexCourant = 0;

        AfficherMessageCourant();
        MettreJeuEnPause(true);
    }

    private void AfficherMessageCourant()
    {
        if (panelIntro != null)
            panelIntro.SetActive(true);

        if (texteIntro != null && indexCourant >= 0 && indexCourant < messagesIntro.Length)
        {
            texteIntro.text = messagesIntro[indexCourant];
        }
    }

    private void MettreJeuEnPause(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0f;
            if (playerInput != null)
                playerInput.enabled = false;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f;
            if (playerInput != null)
                playerInput.enabled = true;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    // Methode appelée par le bouton "Suivant"
    public void BoutonSuivant()
    {
        indexCourant++;

        if (indexCourant >= messagesIntro.Length)
        {
            // Fin de l'intro
            if (panelIntro != null)
                panelIntro.SetActive(false);

            MettreJeuEnPause(false);
            if (sliderVitesse != null)
            {
                sliderVitesse.SetActive(true);
            }
        }
        else
        {
            AfficherMessageCourant();
        }
    }

    // Appelé par le bouton "Passer"
    public void BoutonPasser()
    {
        if (panelIntro != null)
            panelIntro.SetActive(false);

        MettreJeuEnPause(false);
    }
}
