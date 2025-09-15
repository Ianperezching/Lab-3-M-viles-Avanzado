using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    public int damage = 1;

    void Start()
    {
        if (IsServer)
        {
            Invoke(nameof(DespawnSelf), 5f);
        }
    }

    private void DespawnSelf()
    {
        if (IsServer)
        {
            GetComponent<NetworkObject>().Despawn(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Player"))
        {
            Playerlab4 player = other.GetComponent<Playerlab4>();
            if (player != null)
            {
                player.TakeDamageServerRpc(damage);
            }
            GetComponent<NetworkObject>().Despawn(true);
        }
        else
        {
            // Destruye el proyectil si choca con objetos que no sean jugadores
            GetComponent<NetworkObject>().Despawn(true);
        }
    }
}
