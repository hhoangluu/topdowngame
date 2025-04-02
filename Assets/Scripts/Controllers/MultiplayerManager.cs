using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerManager : MonoBehaviour
{
    [SerializeField] private NetworkRunner _runner;
    public async void Connect(string roomName)
    {
        var sceneInfo = new NetworkSceneInfo();
        sceneInfo.AddSceneRef(SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex));
        // Create the startup arguments for the NetworkRunner
        StartGameArgs startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Shared, // Shared mode for Fusion 2.0
            SessionName = roomName,
            Scene = sceneInfo,
        };

        // Start the game
        await _runner.StartGame(startGameArgs);

        Debug.Log($"Connected to room: {roomName}");
    }

    public async void Disconnect()
    {
        await _runner.Shutdown();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}