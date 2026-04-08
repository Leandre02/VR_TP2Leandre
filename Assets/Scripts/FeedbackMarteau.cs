using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Classe attachée au marteau pour gérer les feedbacks haptique et audio lors de la prise en main et de l'impact avec une cible
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
    private XRBaseInputInteractor interactorActuel; // garde en mémoire qui tient le marteau

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
        interactorActuel = args.interactorObject as XRBaseInputInteractor;

        if (interactorActuel != null)
        {
            interactorActuel.SendHapticImpulse(amplitudeGrab, dureeGrab); //  vibration Haptique
        }
    }

    private void OnRelache(SelectExitEventArgs args)
    {
        interactorActuel = null;
    }

    // Appelée par Cible.cs quand le marteau touche une cible
    public void DeclencherImpact()
    {
        // Vibration sur le contrôleur qui tient le marteau
        if (interactorActuel != null)
        {
            interactorActuel.SendHapticImpulse(amplitudeImpact, dureeImpact);
        }

        // Son d'impact à la position du marteau
        if (sonImpact != null)
        {
            audioSource.PlayOneShot(sonImpact);
        }
    }
}