using UnityEngine;

public class CelebrationTrigger : MonoBehaviour
{
    [SerializeField] private Animator npcAnimator;
    [SerializeField] private string triggerName = "Celebrate";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (npcAnimator == null)
        {
            Debug.LogError("No se ha asignado el Animator del NPC.");
            return;
        }

        npcAnimator.SetTrigger(triggerName);

        Debug.Log("NPC celebrando: " + npcAnimator.gameObject.name);
    }
}