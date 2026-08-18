using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator npcAnimator;
    [SerializeField] private string triggerName = "Celebrate";

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip defaultAudio;
    [SerializeField] private AudioClip celebrateAudio;

    private bool alreadyInteracted = false;

    private void Start()
    {
        // Reproducir el audio predeterminado al comenzar
        if (audioSource != null && defaultAudio != null)
        {
            audioSource.clip = defaultAudio;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Evita contar/interactuar nuevamente con el mismo NPC
        if (alreadyInteracted)
            return;

        alreadyInteracted = true;

        // Detener audio predeterminado
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        // Activar animación de celebración
        if (npcAnimator != null)
        {
            npcAnimator.SetTrigger(triggerName);
        }
        else
        {
            Debug.LogWarning("No se ha asignado el Animator del NPC.");
        }

        // Reproducir audio de celebración
        if (audioSource != null && celebrateAudio != null)
        {
            audioSource.clip = celebrateAudio;
            audioSource.loop = false;
            audioSource.Play();
        }

        // Avisar al contador de NPC
        if (NPCInteractionCounter.Instance != null)
        {
            NPCInteractionCounter.Instance.NPCInteracted();
        }
    }
}