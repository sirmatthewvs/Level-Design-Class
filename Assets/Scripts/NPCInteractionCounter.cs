using UnityEngine;
using TMPro;

public class NPCInteractionCounter : MonoBehaviour
{
    public static NPCInteractionCounter Instance;

    [SerializeField] private TMP_Text counterText;

    public int interactedNPCs = 0;

    [SerializeField] AudioSource audioSource;

    [SerializeField] AudioClip victoryClip;

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

        if(interactedNPCs >= 4)
        {
            audioSource.clip = victoryClip;
        }
    }

    private void UpdateUI()
    {
        if (counterText != null)
        {
            counterText.text = "Compañeros rescatados: " + interactedNPCs;
        }
    }
}