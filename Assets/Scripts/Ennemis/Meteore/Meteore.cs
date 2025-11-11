using UnityEngine;

/// <summary>
/// Represente un objet meteorite ayant un rigidbody pour les interactions avec d autres objets
/// Calcul la velicocité pour gerer les variations de la position dans l espace et applique une gravité à l'objet
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Meteore : MonoBehaviour
{

    // Source - gestion de la velocité (variation de la position dans le temps) :
    // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Rigidbody-linearVelocity.html
    public float vitesseChute = 15f;      // Vitesse de chute initiale
    public float vitesseRotation = 90f;   // Vitesse de rotation en degrés/sec

    private Rigidbody rigidBody; // Le rigidbody de ma meteorite 

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // Active la gravité
        rigidBody.useGravity = true;

        // Donne une vitesse de chute vers le bas
        rigidBody.linearVelocity = Vector3.down * vitesseChute;

        // Donne une rotation aléatoire pour l'effet visuel
        rigidBody.angularVelocity = Random.onUnitSphere * Mathf.Deg2Rad * vitesseRotation;
    }
}
