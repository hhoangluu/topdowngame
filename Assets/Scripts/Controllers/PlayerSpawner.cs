using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private NetworkObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private Dictionary<PlayerRef, NetworkObject> _spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();

    public void SpawnPlayer(NetworkRunner runner, PlayerRef player)
    {
        // Choose a spawn point
        Transform spawnPoint = GetRandomSpawnPoint();
        Vector3 spawnPosition = (spawnPoint != null ? spawnPoint.position : Vector3.zero) +
                                (Vector3)Random.insideUnitCircle * 5  + Vector3.up * 5;
        Quaternion spawnRotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;
        // Spawn the player
        NetworkObject playerObject = runner.Spawn(
            playerPrefab,
            spawnPosition,
            spawnRotation,
            player);
        // Store the reference
        _spawnedPlayers[player] = playerObject;

        // Set the network runner on the input collector
        PlayerInputCollector inputCollector = playerObject.GetComponent<PlayerInputCollector>();
        if (inputCollector != null)
        {
            inputCollector.SetRunner(runner);
        
            // Register the input collector with the network runner
            runner.AddCallbacks(inputCollector);
        }
    }

    private Transform GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return null;
        }

        return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
    }

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            SpawnPlayer(Runner, player);
        }
    }
}