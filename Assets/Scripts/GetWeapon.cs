using Gamekit3D;
using UnityEngine;

public class GetWeapon : MonoBehaviour
{
    [Header("Tag del jugador")]
    [SerializeField] private string tagJugador = "Player";
    [SerializeField] PlayerController playerController;
    [SerializeField] ParticleSystem parts;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagJugador)) return;
        playerController.canAttack = true;
        parts.Stop();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(tagJugador)) return;

        
    }
}
