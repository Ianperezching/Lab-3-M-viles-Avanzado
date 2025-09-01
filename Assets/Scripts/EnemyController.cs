using Unity.Netcode;
using UnityEngine;

public class EnemyController : NetworkBehaviour
{
    public float speed = 3f;
    private GameObject targetPlayer;

    void Update()
    {
        if (!IsServer) return;  

        FindClosestPlayer();

        if (targetPlayer != null)
        {
            Vector3 direction = (targetPlayer.transform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float minDistance = Mathf.Infinity;
        GameObject closest = null;

        foreach (var player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = player;
            }
        }

        targetPlayer = closest;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; 

        if (other.CompareTag("Player"))
        {
            
            GetComponent<NetworkObject>().Despawn(true);
        }
    }
}
