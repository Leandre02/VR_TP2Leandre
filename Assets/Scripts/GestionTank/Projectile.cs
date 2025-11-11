using UnityEngine;

/// <summary>
/// Represente un projectile qui avance tout droit et se detruit 
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float vitesse = 80f;   // Vitesse du projectile
    public float dureeVie = 2f;   // Durée de vie avant auto-destruction

    private Rigidbody rigidBody; // Le rigidbody du projectile

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        rigidBody.useGravity = false;
        rigidBody.linearVelocity = transform.forward * vitesse;
        Invoke(nameof(Detruire), dureeVie);
    }

    /// <summary>
    /// Cette méthode est appelée automatiquement lorsqu'une collision est détectée.
    /// Elle détruit le projectile 
    /// </summary>
    /// <param name="collision">Les informations sur la collision détectée.</param>
    void OnCollisionEnter(Collision collision)
    {
        Detruire();
    }

    void Detruire()
    {
        Destroy(gameObject); // Detruit le projectile après la durée de vie
    }
}
