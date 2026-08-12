using UnityEngine;

public class CelebrationTrigger : MonoBehaviour
{
    [Tooltip("Nombre del parámetro Trigger en el Animator")]
    [SerializeField] private string triggerName = "Celebrate";

    [Tooltip("Si quieres que solo se active una vez")]
    [SerializeField] private bool activateOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activateOnce && hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            Animator animator = other.GetComponent<Animator>();

            // Por si el Animator está en un hijo (ej. el modelo 3D)
            if (animator == null)
                animator = other.GetComponentInChildren<Animator>();

            if (animator != null)
            {
                animator.SetTrigger(triggerName);
                hasTriggered = true;
            }
            else
            {
                Debug.LogWarning("No se encontró un Animator en el jugador.");
            }
        }
    }
}