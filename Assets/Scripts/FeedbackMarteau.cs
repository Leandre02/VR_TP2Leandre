using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Classe attachée au marteau pour gérer les feedbacks haptique et audio lors de la prise en main et de l'impact avec une cible
/// S’inspire de l’exercice sur les feedbacks VR (haptiques et audio spatial) et de l’utilisation du XR Grab Interactable / événements
/// du XR Interaction Toolkit (selectEntered / selectExited).
/// Références :
///  - Cégep de Victoriaville. Exercice 4.1 — Feedback VR : haptiques et audio spatial. Environnements Immersifs, 2026. https://envimmersif-cegepvicto.github.io/exercice_feedback_vr/
///  - Cégep de Victoriaville. Exercice 4 — Tri spatial VR : Grab & Socket. Environnements Immersifs, 2026. https://envimmersif-cegepvicto.github.io/exercice_tri_vr/
///  - Unity Technologies. XR Interaction Toolkit Documentation. https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.0/manual/upgrade-guide-3.0.html
/// </summary>
public class FeedbackMarteau : MonoBehaviour
{
    [Header("Haptique - Grab")]
    [SerializeField] private float amplitudeGrab = 0.5f;
    [SerializeField] private float dureeGrab = 0.1f;

    [Header("Haptique - Impact")]
    [SerializeField] private float amplitudeImpact = 0.8f;
    [SerializeField] private float dureeImpact = 0.2f;

    [Header("Audio")]
    [SerializeField] private AudioClip sonImpact;

    private XRGrabInteractable grabInteractable;
    private AudioSource audioSource;
    private XRBaseInputInteractor controller; // Nouveau component du XR interaction Toolkit


    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f; // son spatial en VR
    }

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelache);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelache);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // Garde une référence au contrôleur qui tient le marteau
        controller = args.interactorObject.transform.GetComponent<XRBaseInputInteractor>();

        if (controller != null)
        {
            controller.SendHapticImpulse(amplitudeGrab, dureeGrab); //  vibration Haptique
        }
    }

    private void OnRelache(SelectExitEventArgs args)
    {
        controller = null;
    }

    // Appelée par Cible.cs quand le marteau touche une cible
    public void DeclencherImpact()
    {
        // Vibration sur le contrôleur qui tient le marteau
        if (controller != null)
        {
            controller.SendHapticImpulse(amplitudeImpact, dureeImpact);
        }

        // Son d'impact à la position du marteau
        if (sonImpact != null)
        {
            audioSource.PlayOneShot(sonImpact);
        }
    }
}