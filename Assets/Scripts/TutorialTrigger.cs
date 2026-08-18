using UnityEngine;

/// <summary>
/// Zona invisible que muestra un panel de tutorial cuando el jugador entra.
/// Se activa una sola vez.
/// </summary>
[RequireComponent(typeof(Collider))]
public class TutorialTrigger : MonoBehaviour
{
    [Header("Panel que se muestra al entrar")]
    [SerializeField] private GameObject panel;

    [Header("Tag del jugador")]
    [SerializeField] private string tagJugador = "Player";

    private bool yaActivado;

    // Se ejecuta al agregar el script en el editor: deja el collider como trigger.
    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (yaActivado) return;
        if (!other.CompareTag(tagJugador)) return;

        yaActivado = true;

        if (TutorialManager.Instance != null)
            TutorialManager.Instance.Mostrar(panel);

        // Ya cumplio su funcion, apagamos la zona.
        gameObject.SetActive(false);
    }
}
