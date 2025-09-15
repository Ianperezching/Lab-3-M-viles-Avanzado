using Unity.Netcode;
using UnityEngine;

public class RandomBuff : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Player"))
        {
            Playerlab4 player = other.GetComponent<Playerlab4>();
            if (player != null)
            {
                int buffAmount = UnityEngine.Random.Range(1, 4); // Entre 1 y 3 inclusive
                player.AddAttackBuffServerRpc(buffAmount);
                GetComponent<NetworkObject>().Despawn(true);
                Debug.Log($"Buff aplicado a jugador {player.accoundID.Value}, +{buffAmount} ataque");
            }
        }
    }
}
