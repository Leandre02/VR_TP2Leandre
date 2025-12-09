using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Represente un drone ennemi qui poursuit et impacte la vitesse du tank du joueur
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class Drone : MonoBehaviour
{
    public ControleTank cible;               // Le script du tank cible
    public float distanceDetection;   // Distance de détection

    public float rayonExplosion = 5f;        // Rayon des degats d'explosion
    public float degatsExplosion = 30f;

    private NavMeshAgent agent; // Le NavMeshAgent pour le déplacement
    private Animator animator; // L'Animator pour les animations
    private IDroneState etatCourant; // L'état courant du drone

    public Animator Animator => animator;

    void Awake()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponent<Animator>();
        ChangerEtat(new DroneIdleState(this));
        if (cible == null)
        {
            Debug.LogWarning("Le drone n'a pas de cible assignée !");
        }

    }

    void Update()
    {
        etatCourant?.Update();
    }

    /// <summary>
    /// Une méthode pour changer l'état du drone
    /// </summary>
    /// <param name="nouvelEtat">Le nouvel État du drone</param>
    public void ChangerEtat(IDroneState nouvelEtat)
    {
        etatCourant = nouvelEtat;
        etatCourant?.EntrerEtat(); // Appelle la méthode d'entrée de l'état pour initialiser l'état
    }

    /// <summary>
    /// Une méthode pour déplacer le drone vers une position cible
    /// </summary>
    /// <param name="position">La position cible</param>
    public void DeplacerVersCible(Vector3 position)
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false; // autorise le mouvement
            agent.SetDestination(position);
        }
        else
        {
            Debug.LogWarning("Le NavMeshAgent n'est pas configuré ou n'est pas sur un NavMesh.");
        }
    }

    /// <summary>
    /// Une méthode pour arrêter le déplacement du drone
    /// </summary>
    public void ArreterDeplacement()
    {
        if (agent != null)
        {
            agent.isStopped = true;
        }
    }

    public void Exploser()
    {
        // Verifie si la cible existe et est assez proche
        if (cible == null) return;

        float distanceAuTank = Vector3.Distance(transform.position, cible.transform.position);

        // Si le tank est dans le rayon d'explosion
        if (distanceAuTank <= rayonExplosion)
        {
            // Calcul degats proportionnels
            float facteurDistance = 1f - (distanceAuTank / rayonExplosion);
            float degatsFinaux = degatsExplosion * facteurDistance;

            // Applique degats directement au tank
            Recepteur recepteur = cible.GetComponent<Recepteur>();
            if (recepteur != null)
            {
                recepteur.RecevoirDegats(degatsFinaux);
            }
        }
    }

}
