using UnityEngine;

/// <summary>
/// Script de controle du deplacement de mon tank
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class ControleTank : MonoBehaviour
{
    // Source - pour le deplacement de mon tank : https://docs.unity3d.com/ScriptReference/CharacterController.Move.html

    public float vitesse = 5f;          // Vitesse du tank en m/s
    public float vitesseRotation = 150f; // Vitesse de rotation en degrés/s
    private float multiplicateurVitesse = 1f;
    private float timerRalentissement = 0f;


    private CharacterController controller; // Le controller de mon tank
    private float vitesseVerticale;
    
    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    /// <summary>
    /// Récupere l'entrée de mes Players inputs et assigne l'axe correspondant
    /// </summary>
    void Update()
    {
        // Source - gestion des axes : https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Input.GetAxis.html

        float avance = Input.GetAxis("Vertical");    // Retourne l'axe vertical
        float tourne = Input.GetAxis("Horizontal");  // Retourne l'axe horizontal

        if (timerRalentissement > 0)
        {
            timerRalentissement -= Time.deltaTime;
            if (timerRalentissement <= 0)
            {
                multiplicateurVitesse = 1f; // Revenir à la vitesse normale
            }
        }

        float vitesseEffective = vitesse * multiplicateurVitesse;

        // Calcul de la rotation sur l’axe Y
        if (Mathf.Abs(tourne) > 0.1f)
        {
            transform.Rotate(0, tourne * vitesseRotation * Time.deltaTime, 0);
        }

        // Direction avant du tank
        Vector3 deplacement = transform.forward * avance * vitesseEffective;

        // Source - Pour la gestion de la gravité : https://docs.unity3d.com/6000.0/Documentation/ScriptReference/CharacterController-isGrounded.html
        if (controller.isGrounded)
        {
            vitesseVerticale = -1f; // Force pour rester collé au sol
        }
        else
        {
            vitesseVerticale += Physics.gravity.y * Time.deltaTime; // applique la gravité 
        }

        // Déplacement vertical
        deplacement.y = vitesseVerticale;

        // Appliquer le déplacement
        controller.Move(deplacement * Time.deltaTime);
    }

    /// <summary>
    /// Applique un facteur de ralentissement sur la vitesse du tank pour une durée donnée.
    /// </summary>
    /// <param name="facteur">Multiplicateur de vitesse (ex : 0.5f pour -50%)</param>
    /// <param name="duree">Durée du ralentissement en secondes</param>
    public void AppliquerRalentissement(float facteur, float duree)
    {
        multiplicateurVitesse = facteur;
        timerRalentissement = duree;
    }
}
