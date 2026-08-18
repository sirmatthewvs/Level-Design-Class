using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controla que panel de tutorial esta visible y lo cierra con click.
/// Ponlo en un GameObject vacio de la escena (por ejemplo "TutorialManager").
/// </summary>
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("Panel que aparece al iniciar el juego")]
    [SerializeField] private GameObject panelInicial;   // el de "como moverte"

    private GameObject panelActual;
    private int frameEnQueSeMostro = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (panelInicial != null)
            Mostrar(panelInicial);
    }

    private void Update()
    {
        if (panelActual == null) return;

        // Evita que se cierre en el mismo frame en que se abrio.
        if (Time.frameCount == frameEnQueSeMostro) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            Cerrar();
    }

    /// <summary>Muestra un panel (y esconde el que estuviera abierto).</summary>
    public void Mostrar(GameObject panel)
    {
        if (panel == null) return;

        if (panelActual != null && panelActual != panel)
            panelActual.SetActive(false);

        panelActual = panel;
        panelActual.SetActive(true);
        frameEnQueSeMostro = Time.frameCount;
    }

    /// <summary>Cierra el panel abierto.</summary>
    public void Cerrar()
    {
        if (panelActual == null) return;

        panelActual.SetActive(false);
        panelActual = null;
    }
}
