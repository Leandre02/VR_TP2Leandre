using UnityEngine;

public class Recepteur : MonoBehaviour
{
    private Vie vie; // Référence au script Vie sur le même objet

    void Awake()
    {
        vie = GetComponent<Vie>();
    }

    // Appelé lors d'une collision ou d'un trigger avec un projectile
    public void RecevoirDegats(float valeur)
    {
        if (vie != null)
        {
            vie.PrendreDegats(valeur);
        }
    }

    // Pour les collisions
    void OnCollisionEnter(Collision col)
    {
        Degats degats = col.gameObject.GetComponent<Degats>();
        if (degats != null)
        {
            RecevoirDegats(degats.valeurDegats);
        }
    }

    // Variante pour les triggers
    void OnTriggerEnter(Collider other)
    {
        Degats degats = other.GetComponent<Degats>();
        if (degats != null)
        {
            RecevoirDegats(degats.valeurDegats);
        }
    }
}
