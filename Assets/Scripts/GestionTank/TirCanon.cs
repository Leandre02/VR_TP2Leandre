using UnityEngine;

/// <summary>
/// Represente le tir d un canon 
/// </summary>
public class TirCanon : MonoBehaviour
{
    [Header("Projectile")]
    public GameObject prefabProjectile;
    public Transform sortie;
    public float cadence = 6f;

    private float delaiAvantProchainTir = 0f;
    private bool tirActif = false;

    void Start()
    {
        // On s'abonne une fois que tout est bien initialisé
        if (SystemeJeu.Instance != null)
        {
            SystemeJeu.Instance.OnTir += RecevoirTir;
            Debug.Log("[TirCanon] Abonné à OnTir");
        }
        else
        {
            Debug.LogWarning("[TirCanon] SystemeJeu.Instance est null au Start");
        }
    }

    void OnDestroy()
    {
        if (SystemeJeu.Instance != null)
        {
            SystemeJeu.Instance.OnTir -= RecevoirTir;
        }
    }


    void OnEnable()
    {
        if (SystemeJeu.Instance != null)
            SystemeJeu.Instance.OnTir += RecevoirTir;
    }

    void OnDisable()
    {
        if (SystemeJeu.Instance != null)
            SystemeJeu.Instance.OnTir -= RecevoirTir;
    }

    void RecevoirTir(bool actif)
    {
        tirActif = actif;
        Debug.Log("[TirCanon] RecevoirTir -> " + tirActif);
    }

    void Update()
    {
        delaiAvantProchainTir -= Time.deltaTime;

        if (tirActif && delaiAvantProchainTir <= 0f && prefabProjectile != null && sortie != null)
        {
            delaiAvantProchainTir = 1f / Mathf.Max(0.01f, cadence);
            Instantiate(prefabProjectile, sortie.position, sortie.rotation);

            // Son de tir centralisé
            if (GestionAudio.Instance != null)
            {
                GestionAudio.Instance.JouerTir();
            }
            Debug.Log("[TirCanon] Tir !");
        }
    }
}
