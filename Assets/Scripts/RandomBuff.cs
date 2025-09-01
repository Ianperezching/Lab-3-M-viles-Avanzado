using Unity.Netcode;
using UnityEngine;

public class RandomBuff : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
          
            AddBuffPlayerRpc(NetworkManager.Singleton.LocalClientId);
        }
        
    }

    [Rpc(SendTo.Server)]
    private void AddBuffPlayerRpc(ulong PlayerID)
    {
        print("Aplicar Buff a " + PlayerID);
        GetComponent<NetworkObject>().Despawn(true);
    }
}
