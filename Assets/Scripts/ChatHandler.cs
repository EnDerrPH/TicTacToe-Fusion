using UnityEngine;
using Fusion;
using UnityEngine.UI;
using TMPro;

public class ChatHandler : NetworkBehaviour
{
    [SerializeField] NetworkRunner _networkRunner;
    [SerializeField] string _playerLabel;
    [SerializeField] TMP_Text _messages;
    [SerializeField] TMP_Text _inputText;
    GameManager _gameManager;

    public void SetNetworkRunner(NetworkRunner networkRunner)
    {
        _networkRunner = networkRunner;
    }

    public void SetGameManager(GameManager gameManager)
    {
        _gameManager = gameManager;
        SetLabel();
    }

    public void SetLabel()
    {
        if (_gameManager.PlayerOneRef == _networkRunner.LocalPlayer)
        {
            _playerLabel = "Player One";
        }
        else if (_gameManager.PlayerTwoRef == _networkRunner.LocalPlayer)
        {
            _playerLabel = "Player Two";
        }
    }

    public void CallMessageRPC()
    {
        string message = _inputText.text;
        RPC_SendMessage(_playerLabel, message);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SendMessage(string name, string message, RpcInfo rpcInfo = default)
    {
        _messages.text += $"{name}: {message}\n";
    }
}
