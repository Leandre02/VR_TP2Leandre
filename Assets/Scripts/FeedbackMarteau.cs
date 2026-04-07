using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FeedbackMarteau : MonoBehaviour
{
    [Header("Haptique")]
    public float amplitudeGrab = 0.5f;
    public float dureeGrab = 0.1f;
    public float amplitudeImpact = 0.8f;
    public float dureeImpact = 0.2f;

    [Header("Audio")]
    public AudioClip sonImpact;

    private XRGrabInteractable grabInteractable;
    private AudioSource audioSource;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f; // son spatial
    }

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        var interactor = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor;
        if (interactor != null)
        {
            interactor.SendHapticImpulse(amplitudeGrab, dureeGrab);
        }
    }

    // Appel�e par le script Cible quand impact d�tect�
    public void DeclencherImpact(UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor interactor)
    {
        if (interactor != null)
        {
            interactor.SendHapticImpulse(amplitudeImpact, dureeImpact);
        }
        if (sonImpact != null)
        {
            audioSource.PlayOneShot(sonImpact);
        }
    }
}