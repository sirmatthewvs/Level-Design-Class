using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [SerializeField] private Animator npcAnimator;
    [SerializeField] private string triggerName = "Celebrate";

    private bool alreadyInteracted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Evita contar al mismo NPC más de una vez
        if (alreadyInteracted)
            return;

        alreadyInteracted = true;

        // Activar animación del NPC
        if (npcAnimator != null)
        {
            npcAnimator.SetTrigger(triggerName);
        }
        else
        {
            Debug.LogWarning("No se ha asignado el Animator del NPC.");
        }

        // Avisar al contador
        if (NPCInteractionCounter.Instance != null)
        {
            NPCInteractionCounter.Instance.NPCInteracted();
        }
    }
}