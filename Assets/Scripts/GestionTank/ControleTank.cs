using UnityEngine;

/// <summary>
/// Script de controle du deplacement de mon tank
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class ControleTank : MonoBehaviour
{
    public float vitesse = 5f;
    public float vitesseRotation = 150f;

   
    private CharacterController controller;
    private float vitesseVerticale;

    // Stocke les inputs recus des events
    private Vector2 inputDeplacement;

    private float vitesseActuelle = 0f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        if (SystemeJeu.Instance != null)
            SystemeJeu.Instance.OnDeplacement += RecevoirDeplacement;
    }

    void OnDisable()
    {
        if (SystemeJeu.Instance != null)
            SystemeJeu.Instance.OnDeplacement -= RecevoirDeplacement;
    }

    void RecevoirDeplacement(Vector2 mouvement)
    {
        inputDeplacement = mouvement;
    }

    void Update()
    {
        // Rotation
        if (Mathf.Abs(inputDeplacement.x) > 0.1f)
        {
            transform.Rotate(0, inputDeplacement.x * vitesseRotation * Time.deltaTime, 0);
        }

        // Déplacement avant/arrière
        Vector3 deplacement = transform.forward * inputDeplacement.y * vitesse;

        // Vitesse actuelle (pour le son)
        vitesseActuelle = Mathf.Abs(inputDeplacement.y) * vitesse;

        // Gravité
        if (controller.isGrounded)
            vitesseVerticale = -1f;
        else
            vitesseVerticale += Physics.gravity.y * Time.deltaTime;

        deplacement.y = vitesseVerticale;
        controller.Move(deplacement * Time.deltaTime);

        // Mise à jour du son moteur via le gestionnaire audio
        if (GestionAudio.Instance != null)
        {
            GestionAudio.Instance.MettreAJourMoteur(vitesseActuelle);
        }
    }

   
}
