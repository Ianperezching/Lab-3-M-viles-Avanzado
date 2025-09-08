using Unity.Netcode;
using UnityEngine;

public class EnemyController : NetworkBehaviour
{
    public NetworkVariable<int> Health = new NetworkVariable<int>(3);  // Vida inicial

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

        foreach (GameObject player in players)
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

    public void TakeDamage(int amount)
    {
        if (!IsServer) return;

        Health.Value -= amount;
        Debug.Log("Enemy took damage. Health now: " + Health.Value);

        if (Health.Value <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died.");
        GetComponent<NetworkObject>().Despawn(true);
    }
}
