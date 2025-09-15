using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System;

public class GameManager2 : NetworkBehaviour
{
    public static GameManager2 Instance;

    public GameObject playerPrefab;  
    public GameObject BuffPrefab;

    public float currentBuffTimer;
    public float buffSpawnInterval = 15f;

    public Dictionary<string, PlayerData> playersStatesByAccountId = new();

    public Action OnConnection;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleDisconnect;
        }
        OnConnection?.Invoke();
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleDisconnect;
        }
        base.OnNetworkDespawn();
    }

    private void HandleDisconnect(ulong clientID)
    {
        Debug.Log("El jugador " + clientID + " se ha desconectado");
    }

    [Rpc(SendTo.Server)]
    public void RegisterPlayerServerRpc(string accountID, ulong clientId)
    {
        if (!playersStatesByAccountId.TryGetValue(accountID, out PlayerData data))
        {
            PlayerData newData = new PlayerData(accountID, Vector3.zero, 100, 5);
            playersStatesByAccountId[accountID] = newData;
            SpawnPlayerServer(clientId, newData);
            Debug.Log("Nueva cuenta registrada: " + accountID);
        }
        else
        {
            Debug.Log("Cuenta encontrada: " + accountID);
            SpawnPlayerServer(clientId, data);
        }
    }

    public void SpawnPlayerServer(ulong clientId, PlayerData data)
    {
        if (!IsServer) return;

        GameObject player = Instantiate(playerPrefab);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        player.GetComponent<Playerlab4>().Setdata(data);
    }

    private void Update()
    {
        if (IsServer && NetworkManager.Singleton.ConnectedClients.Count >= 2)
        {
            currentBuffTimer += Time.deltaTime;

            if (currentBuffTimer > buffSpawnInterval)
            {
                Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-8, 8), 0.5f, UnityEngine.Random.Range(-8, 8));
                GameObject buff = Instantiate(BuffPrefab, randomPos, Quaternion.identity);
                buff.GetComponent<NetworkObject>().Spawn(true);
                currentBuffTimer = 0;
            }
        }
    }
}

[Serializable]
public class PlayerData
{
    public string accoundID;
    public Vector3 position;
    public int health;
    public int attack;

    public PlayerData(string ID, Vector3 pos, int hp, int atk)
    {
        this.accoundID = ID;
        this.position = pos;
        this.health = hp;
        this.attack = atk;
    }
}
