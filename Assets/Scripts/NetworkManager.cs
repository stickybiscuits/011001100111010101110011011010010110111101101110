using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ConnectionStatus
{
    Disconnected,
    Connecting,
    Connected,
    Failed,
    EnteringLobby,
    InLobby,
    Starting,
    Started
}

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner;
    private NetworkSceneManagerBase _loader;

    // Game session
    private Session _session;
    public Session Session
    {
        get => _session;
        set { _session = value; _session.transform.SetParent(_runner.transform); }
    }

    public ConnectionStatus ConnectionStatus { get; private set; }

    // Singleton manager
    private static NetworkManager _instance;
    public static NetworkManager Instance {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<NetworkManager>();
            return _instance;
        }
    }

    // Lobby
    [SerializeField] SceneReference _lobbyScene;
    // Rooms the user can enter
    public List<SceneReference> availableRooms = new List<SceneReference>();

    // Player and session prefabs
    [SerializeField] private Player _playerPrefab;
    [SerializeField] private Session _sessionPrefab;

    // Players in the game
    private readonly Dictionary<PlayerRef, Player> _players = new Dictionary<PlayerRef, Player>();
    public ICollection<Player> Players => _players.Values;

    // Text object to log debugs
    [SerializeField] TMPro.TMP_Text debugText;

    // idk what this does
    public bool IsMaster => _runner != null && (_runner.IsServer || _runner.IsSharedModeMasterClient);

    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        if (_instance != this)
            Destroy(gameObject);

        else if(_loader == null)
        {
            _loader = GetComponent<NetworkSceneManagerBase>();
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.LoadSceneAsync(_lobbyScene);
    }

    private void Start()
    {
        
    }

    public void Connect()
    {
        if (_runner == null)
        {
            SetConnectionStatus(ConnectionStatus.Connecting);

            GameObject go = new GameObject("Session");
            go.transform.SetParent(transform);

            _players.Clear();

            _runner = go.AddComponent<NetworkRunner>();
            _runner.AddCallbacks(this);
        }
    }

    public void StartSession(GameMode mode, SessionProps props, bool disableClientSessionCreation = true)
    {
        Connect();

        SetConnectionStatus(ConnectionStatus.Starting);

        LogText($"Starting game with session <color=orange><b>{props.RoomName}</b></color>, player limit <color=orange><b>{props.MaxPlayers}</b></color>");

        _runner.ProvideInput = mode != GameMode.Server;

        _runner.StartGame(new StartGameArgs
        {
            GameMode = mode,
            CustomLobbyName = "DefaultLobby",
            SceneObjectProvider = _loader,
            SessionName = props.RoomName,
            PlayerCount = props.MaxPlayers,
            SessionProperties = props.Properties,
            DisableClientSessionCreation = disableClientSessionCreation
        });
    }

    public void CreateSession(SessionProps props)
    {
        StartSession(GameMode.Host, props);
    }

    public void JoinSession(SessionInfo info)
    {
        SessionProps props = new SessionProps(info.Properties);
        props.MaxPlayers = info.MaxPlayers;
        props.RoomName = info.Name;

        StartSession(GameMode.Client, props);
    }

    public void SetPlayer(PlayerRef playerRef, Player player)
    {
        _players[playerRef] = player;
        player.transform.SetParent(_runner.transform);

        if (Session.Room != null)
        { 
            // Late join
            Session.Room.SpawnAvatar(player, true);
        }
    }

    public Player GetPlayer(PlayerRef playerRef = default)
    {
        if (!_runner)
            return null;

        if (playerRef == default)
            playerRef = _runner.LocalPlayer;

        _players.TryGetValue(playerRef, out Player player);

        return player;
    }

    public void Disconnect()
    {
        if (_runner != null)
        {
            SetConnectionStatus(ConnectionStatus.Disconnected);
            _runner.Shutdown();
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        LogText("Connected to server.");
        SetConnectionStatus(ConnectionStatus.Connected);
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        LogText("Disconnected from server.");
        Disconnect();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        LogText($"Connection failed due to <color=red><b>{reason}</b></color>");
        Disconnect();
        SetConnectionStatus(ConnectionStatus.Failed, reason.ToString());
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        LogText($"Player <color=pink><b>{player}</b></color> Joined!");

        if (_session == null && IsMaster)
        {
            Debug.Log("Spawning world");
            _session = runner.Spawn(_sessionPrefab, Vector3.zero, Quaternion.identity);
        }

        if (runner.IsServer || runner.Topology == SimulationConfig.Topologies.Shared && player == runner.LocalPlayer)
        {
            Debug.Log("Spawning player");
            runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity, player);
        }

        SetConnectionStatus(ConnectionStatus.Started);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"{player.PlayerId} disconnected.");

        if (_players.TryGetValue(player, out Player playerobj))
        {
            _session.Room.DespawnAvatar(playerobj);

            if (playerobj.Object != null && playerobj.Object.HasStateAuthority)
            {
                Debug.Log("Despawning Player");
                runner.Despawn(playerobj.Object);
            }
            _players.Remove(player);
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

    private void SetConnectionStatus(ConnectionStatus status, string reason = "")
    {
        if (ConnectionStatus == status)
            return;
        ConnectionStatus = status;

        LogText($"ConnectionStatus: <color=lime><b>{status} {reason}</b></color>");
    }

    public void LogText(string msg)
    {
        if (debugText)
            debugText.text += "\n" + msg;

        Debug.Log(msg);
    }
}
