using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    public int damage = 1;

    void Start()
    {
        if (IsServer)
        {
            Invoke(nameof(DespawnSelf), 5f); // Auto destrucción después de 5 segundos
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

        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            GetComponent<NetworkObject>().Despawn(true);
        }
        else if (!other.CompareTag("Player"))
        {
            // Destruye el proyectil si choca con paredes u otros objetos (excepto jugador)
            GetComponent<NetworkObject>().Despawn(true);
        }
    }
}
