using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Playerlab4 : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes>accoundID = new();
    public NetworkVariable<int> health = new();
    public NetworkVariable<int> attack = new();



    public void Setdata(PlayerData playerData)
    {
        accoundID.Value = playerData.accoundID;
        health.Value = playerData.health;
        attack.Value = playerData.attack;
        transform.position = playerData.position;
    }

    public override void OnNetworkDespawn()
    {
        GameManager2.Instance.playersStatesByAccountId[accoundID.Value.ToString()]
            = new PlayerData(accoundID.Value.ToString(),
            transform.position,
            health.Value,
            attack.Value);



        print("me e desconectado " + NetworkManager.Singleton.LocalClientId);
    }
}
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
