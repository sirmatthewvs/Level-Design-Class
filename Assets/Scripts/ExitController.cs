using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Zona de salida. Mientras el jugador esta dentro muestra un panel u otro
/// segun cuantos NPCs haya rescatado. Al salir del collider se apagan los dos.
///
/// Los enemigos del nivel van como HIJOS de este objeto: al empezar se guarda
/// la lista y los que ya no existen cuentan como derrotados.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ExitController : MonoBehaviour
{
    [Header("Cuantos companeros hay que rescatar")]
    [SerializeField] private int objetivo = 4;

    [Header("Paneles")]
    [SerializeField] private GameObject panelFaltan;   // "aun te faltan companeros"
    [SerializeField] private GameObject panelSalida;   // panel final con el resumen

    [Header("Texto del panel de faltan (opcional)")]
    [SerializeField] private TMP_Text textoFaltan;

    [Header("Textos del panel final (opcionales)")]
    [SerializeField] private TMP_Text textoTiempo;
    [SerializeField] private TMP_Text textoRescatados;
    [SerializeField] private TMP_Text textoEnemigos;

    [Header("Tag del jugador")]
    [SerializeField] private string tagJugador = "Player";

    private readonly List<Transform> enemigos = new List<Transform>();

    private bool jugadorDentro;
    private bool nivelTerminado;
    private float tiempoFinal;

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

    private void Awake()
    {
        // Guarda todos los hijos que existen al empezar el nivel (los enemigos).
        foreach (Transform hijo in transform)
            enemigos.Add(hijo);
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

        if (completo)
        {
            // El cronometro se congela la primera vez que se completa el nivel.
            if (!nivelTerminado)
            {
                nivelTerminado = true;
                tiempoFinal = Time.timeSinceLevelLoad;
            }

            LlenarResumen(rescatados);
        }
        else if (textoFaltan != null)
        {
            int faltan = Mathf.Max(0, objetivo - rescatados);
            textoFaltan.text = "Te faltan " + faltan + (faltan == 1 ? " companero" : " companeros");
        }
    }

    private void LlenarResumen(int rescatados)
    {
        if (textoTiempo != null)
            textoTiempo.text = "Tiempo: " + FormatearTiempo(tiempoFinal);

        if (textoRescatados != null)
            textoRescatados.text = "Companeros rescatados: " + rescatados + " / " + objetivo;

        if (textoEnemigos != null)
            textoEnemigos.text = "Enemigos derrotados: " + EnemigosDerrotados() + " / " + enemigos.Count;
    }

    /// <summary>Cuenta los hijos que fueron destruidos o desactivados.</summary>
    private int EnemigosDerrotados()
    {
        int derrotados = 0;

        for (int i = 0; i < enemigos.Count; i++)
        {
            if (enemigos[i] == null || !enemigos[i].gameObject.activeSelf)
                derrotados++;
        }

        return derrotados;
    }

    private string FormatearTiempo(float segundosTotales)
    {
        int minutos = Mathf.FloorToInt(segundosTotales / 60f);
        int segundos = Mathf.FloorToInt(segundosTotales % 60f);
        return string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    private void Ocultar()
    {
        if (panelFaltan != null) panelFaltan.SetActive(false);
        if (panelSalida != null) panelSalida.SetActive(false);
    }
}