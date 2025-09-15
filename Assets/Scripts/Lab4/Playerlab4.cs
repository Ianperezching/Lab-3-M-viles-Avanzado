using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Playerlab4 : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> accoundID = new();
    public NetworkVariable<int> health = new();
    public NetworkVariable<int> attack = new();

    public void Setdata(PlayerData playerData)
    {
        accoundID.Value = playerData.accoundID;
        health.Value = playerData.health;
        attack.Value = playerData.attack;
        transform.position = playerData.position;
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage)
    {
        if (health.Value <= 0) return;

        health.Value -= damage;
        Debug.Log($"Jugador {accoundID.Value} recibió {damage} de daño, salud actual: {health.Value}");

        if (health.Value <= 0)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        health.Value = 100;
        Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-8, 8), 0.5f, UnityEngine.Random.Range(-8, 8));
        transform.position = randomPos;
        UpdatePlayerData();
        Debug.Log($"Jugador {accoundID.Value} ha reaparecido en {randomPos}");
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddAttackBuffServerRpc(int buffAmount)
    {
        attack.Value += buffAmount;
        UpdatePlayerData();
        Debug.Log($"Jugador {accoundID.Value} recibió buff de ataque +{buffAmount}. Ataque actual: {attack.Value}");
    }

    private void UpdatePlayerData()
    {
        if (GameManager2.Instance.playersStatesByAccountId.ContainsKey(accoundID.Value.ToString()))
        {
            GameManager2.Instance.playersStatesByAccountId[accoundID.Value.ToString()] =
                new PlayerData(accoundID.Value.ToString(), transform.position, health.Value, attack.Value);
        }
    }

    private void Update()
    {
        if (IsServer)
        {
            UpdatePlayerData();
        }
    }

    public override void OnNetworkDespawn()
    {
        UpdatePlayerData();
        base.OnNetworkDespawn();
        Debug.Log($"Jugador {accoundID.Value} se desconectó y guardó estado.");
    }
}
