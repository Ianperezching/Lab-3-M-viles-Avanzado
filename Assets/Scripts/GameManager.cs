using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject BuffPrefab;

    public float currentbuff;
    public float buffSpawnCount;

    public GameObject EnemyPrefab;
    public float enemySpawnInterval = 10f;
    private float enemySpawnTimer = 0;

    void Start()
    {
        
    }
    public override void OnNetworkSpawn()
    {
        print("CurrentPlayer" + NetworkManager.Singleton.ConnectedClients.Count);
        SpawnPlayerRpc(NetworkManager.Singleton.LocalClientId);
    }
    [Rpc(SendTo.Server)]
    public void SpawnPlayerRpc(ulong id)
    {
        GameObject player = Instantiate(PlayerPrefab);
       // player.GetComponent<NetworkObject>().Spawn(true);
       player.GetComponent<SimplePlayerController>().PlayerID.Value = id;
       player.GetComponent<NetworkObject>().SpawnWithOwnership(id);
    }

    void Update()
    {
        if (IsServer && NetworkManager.Singleton.ConnectedClients.Count >= 2)
        {
            currentbuff += Time.deltaTime;
            enemySpawnTimer += Time.deltaTime;
            if (currentbuff > buffSpawnCount)
            {
                Vector3 ramdompos = new Vector3(Random.Range(-8, 8), 0.5f, Random.Range(-8, 8));
                GameObject buff = Instantiate(BuffPrefab, ramdompos, Quaternion.identity);
                buff.GetComponent<NetworkObject>().Spawn(true);
                currentbuff = 0;
            }

            if (enemySpawnTimer > enemySpawnInterval)
            {
                Vector3 randomPos = new Vector3(Random.Range(-8, 8), 0.5f, Random.Range(-8, 8));
                GameObject enemy = Instantiate(EnemyPrefab, randomPos, Quaternion.identity);
                enemy.GetComponent<NetworkObject>().Spawn(true);
                enemySpawnTimer = 0;
            }
        }

    }
}
