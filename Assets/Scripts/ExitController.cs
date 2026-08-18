using UnityEngine;
using TMPro;

/// <summary>
/// Zona de salida. Mientras el jugador esta dentro muestra un panel u otro
/// segun cuantos NPCs haya rescatado. Al salir del collider se apagan los dos.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ExitController : MonoBehaviour
{
    [Header("Cuantos companeros hay que rescatar")]
    [SerializeField] private int objetivo = 4;

    [Header("Paneles")]
    [SerializeField] private GameObject panelFaltan;   // "aun te faltan companeros"
    [SerializeField] private GameObject panelSalida;   // "puedes escapar"

    [Header("Opcional: texto dentro del panel de faltan")]
    [SerializeField] private TMP_Text textoFaltan;

    [Header("Tag del jugador")]
    [SerializeField] private string tagJugador = "Player";

    private bool jugadorDentro;

    /// <summary>True cuando ya se cumplio el objetivo.</summary>
    public bool PuedeSalir
    {
        get
        {
            return NPCInteractionCounter.Instance != null
                && NPCInteractionCounter.Instance.interactedNPCs >= objetivo;
        }
    }

    // Al agregar el script en el editor deja el collider como trigger.
    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void Start()
    {
        Ocultar();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagJugador)) return;

        jugadorDentro = true;
        Refrescar();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(tagJugador)) return;

        jugadorDentro = false;
        Ocultar();
    }

    private void Update()
    {
        // Se revisa cada frame por si el contador cambia estando dentro de la zona.
        if (jugadorDentro)
            Refrescar();
    }

    private void Refrescar()
    {
        int rescatados = 0;
        if (NPCInteractionCounter.Instance != null)
            rescatados = NPCInteractionCounter.Instance.interactedNPCs;

        bool completo = rescatados >= objetivo;

        if (panelFaltan != null) panelFaltan.SetActive(!completo);
        if (panelSalida != null) panelSalida.SetActive(completo);

        if (!completo && textoFaltan != null)
        {
            int faltan = Mathf.Max(0, objetivo - rescatados);
            textoFaltan.text = "Te faltan " + faltan + (faltan == 1 ? " compañero" : " compañeros");
        }
    }

    private void Ocultar()
    {
        if (panelFaltan != null) panelFaltan.SetActive(false);
        if (panelSalida != null) panelSalida.SetActive(false);
    }
}