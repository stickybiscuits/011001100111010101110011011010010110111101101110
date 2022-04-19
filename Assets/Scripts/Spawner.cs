using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner;

    [SerializeField] List<NetworkPrefabRef> _playerPrefabs;
    List<int> _spawnedPrefabs = new List<int>();
    
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    [SerializeField] GameObject buttonPanel;

    public void StartGameAsHost()
    {
        if(_runner == null)
            StartGame(GameMode.Host);
    }

    public void StartGameAsShared()
    {
        if (_runner == null)
            StartGame(GameMode.Shared);
    }

    public void StartGameAsClient()
    {
        if (_runner == null)
            StartGame(GameMode.Client);
    }

    async void StartGame(GameMode mode)
    {
        buttonPanel.SetActive(false);

        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneObjectProvider = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server.");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("Connection failed due to: " + reason.ToString());
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("Disconnected from server.");
        buttonPanel.SetActive(true);
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector3.right;

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if(_spawnedPrefabs.Count == _playerPrefabs.Count)
        {
            Debug.Log("Ran out of prefabs to spawn");
            _runner.Disconnect(player);
            return;
        }

        // Create a unique position for the player
        Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 3, 1, 0);

        // Spawn random prefab
        int random;
        int iteration = 0;
        do
        {
            random = UnityEngine.Random.Range(0, _playerPrefabs.Count);
            iteration++;
            Debug.Log("Iteration: " + iteration);
        }
        while (_spawnedPrefabs.Count != _playerPrefabs.Count && _spawnedPrefabs.Contains(random));


        _spawnedPrefabs.Add(random);

        NetworkPrefabRef _playerPrefab = _playerPrefabs[random];
        NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            
        // Keep track of the player avatars so we can remove it when they disconnect
        _spawnedCharacters.Add(player, networkPlayerObject);
        Debug.Log("Character count: " + _spawnedCharacters.Count);

        if (player == _runner.LocalPlayer)
        {
            Debug.Log("Local player");
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // Find and remove the players avatar
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }
}
