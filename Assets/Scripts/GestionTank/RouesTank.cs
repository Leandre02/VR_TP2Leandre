using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represente les roues de mon tanks
/// Anime chaque roues de la liste pour lui appliqué une vitesse de rotation selon l'axe des y 
/// Utilise un Mathf.Abs pour recuperer la postion vertical de ma roue au moment du deplacement
/// </summary>
public class RouesTank : MonoBehaviour
{
    public List<Transform> roues;        // Liste des roues à animer
    public float vitesseRotationRoues = 360f; // degrés de rotation par seconde

    private ControleTank controleTank; 

    void Start()
    {
        controleTank = GetComponent<ControleTank>(); // Initialise mon controletank au gameobject
    }

    void Update()
    {
        if (controleTank == null) return;

        float vitesse = Mathf.Abs(Input.GetAxis("Vertical")); // Recupere la valeur de l'axe vertical pour la vitesse
        float rotation = vitesse * vitesseRotationRoues * Time.deltaTime; // Calcule la rotation en degrés pour cette frame

        foreach (var roue in roues)
        {
            roue.Rotate(rotation, 0, 0);
        }
    }
}
