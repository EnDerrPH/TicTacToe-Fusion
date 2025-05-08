
using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] List<Button> _cellButtonList = new List<Button>();
    [SerializeField] ChatHandler _chatHandler;
    [SerializeField] TMP_Text _playerText;
    [SerializeField] TMP_Text _turnText;
    [SerializeField] GameObject _gameManagerPrefab;
    public static NetworkManager instance;
    public NetworkRunner _networkRunner;
    public List<Button> CellButtonList => _cellButtonList;
    public TMP_Text PlayerText => _playerText;
    public TMP_Text TurnText => _turnText;

    private void Awake()
    {
        if (_networkRunner == null)
        {
            _networkRunner = gameObject.AddComponent<NetworkRunner>();
        }
    }

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        _chatHandler.SetNetworkRunner(_networkRunner);
        StartGame();
    }

    public NetworkRunner GetNetworkRunner()
    {
        return _networkRunner;
    }

    public ChatHandler GetChatHandler()
    {
        return _chatHandler;
    }

    async void StartGame()
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _networkRunner.ProvideInput = true;
        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Start or join (depends on gamemode) a session with a specific name
        await _networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "TestRoom",
            Scene = scene,
            PlayerCount = 2,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }


    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            
        }
        else
        {

        }
        InitializeGame();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        
    }

    #region UnusedMethods

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
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

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
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
    #endregion
    private void InitializeGame()
    {
        int playerCount = _networkRunner.ActivePlayers.Count();

        if (playerCount <= 1)
        {
            return;
        }
        InitializeGameManager();
    }

    private void InitializeGameManager()
    {
        if (_networkRunner.IsServer)
        {
            Vector3 spawnPosition = Vector3.zero;
            Quaternion spawnRotation = Quaternion.identity;
            _networkRunner.Spawn(_gameManagerPrefab, spawnPosition, spawnRotation);
        }
    }
}