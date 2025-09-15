using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System;

public class GameManager2 : NetworkBehaviour
{
    public GameObject playerPrefab;
    public static GameManager2 Instance;
    public Dictionary<string, PlayerData> playersStatesByAccountId = new();

    public Action OnConnection;

    public void Awake()
    {
        if (Instance == null)
        {
           Instance = this;
           DontDestroyOnLoad(gameObject);
        }
        else {
        Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleDisconnect;

        OnConnection?.Invoke(); 
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleDisconnect;
        base.OnNetworkDespawn();
    }
  
    private void HandleDisconnect(ulong clientID)
    {
        print("el jugador " + clientID + "se a desconectado");
    }

    [Rpc(SendTo.Server)]
    public void RegisterPlayerServerRpc(string accoundID, ulong ID)
    {
        if (!playersStatesByAccountId.TryGetValue(accoundID, out PlayerData data))
        {
            PlayerData NewData = new PlayerData(accoundID, Vector3.zero, 100, 5);
            playersStatesByAccountId[accoundID] = NewData;
            SpawnPlayerServer(ID, NewData);
            print("se encontro cuenta" + accoundID);
        }
        else
        {
            print("se encontro cuenta" + accoundID);
            SpawnPlayerServer(ID, data);
        }
    }

    public void SpawnPlayerServer(ulong ID,PlayerData data)
    {
        if (!IsServer) return;
        GameObject player= Instantiate(playerPrefab);
        
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(ID, true);
        player.GetComponent<Playerlab4>().Setdata(data);
    }

}
