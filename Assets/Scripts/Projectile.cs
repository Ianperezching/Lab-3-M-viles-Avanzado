using UnityEngine;
using Unity.Netcode;


public class Projectile : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Destroy(gameobject, 5);
        if (IsServer)
        {
            Invoke("SimpleDespawn", 5);
        }
    }

    public void SimpleSpawn()
    {
        GetComponent<NetworkObject>().Despawn(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
