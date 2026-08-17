using UnityEngine;
using TMPro;

public class NPCInteractionCounter : MonoBehaviour
{
    public static NPCInteractionCounter Instance;

    [SerializeField] private TMP_Text counterText;

    private int interactedNPCs = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    public void NPCInteracted()
    {
        interactedNPCs++;

        UpdateUI();

        Debug.Log("NPC interactuados: " + interactedNPCs);
    }

    private void UpdateUI()
    {
        if (counterText != null)
        {
            counterText.text = "Compañeros rescatados: " + interactedNPCs;
        }
    }
}