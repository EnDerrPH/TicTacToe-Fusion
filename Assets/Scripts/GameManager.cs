using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class GameManager : NetworkBehaviour
{
    List<Button> _cellButtonList = new List<Button>();
    TMP_Text _playerText;
    TMP_Text _turnText;
    NetworkRunner _networkRunner;
    ChatHandler _chatHandler;
    private List<int> _playerOneMoves = new List<int>();
    private List<int> _playerTwoMoves = new List<int>();
    [Networked] public  PlayerRef PlayerOneRef { get; set; }
    [Networked] public  PlayerRef PlayerTwoRef { get; set; }
    [Networked] private PlayerRef _playerTurn { get; set; }

    public override void Spawned()
    {
        InitializeGameManager();
        AddListener();
    }

    public void OnCellClick(int index)
    {
        if (_networkRunner.LocalPlayer != _playerTurn)
            return;
        AudioManager.instance.OnCellSFX();
        UpdateCollectedCells(_playerTurn, index);
        if (_networkRunner.IsServer)
        {
            bool isCross = _playerTurn == PlayerOneRef;
            RPC_UpdateCellVisual(index, isCross);

            if (CheckWin(_playerTurn == PlayerOneRef ? _playerOneMoves : _playerTwoMoves))
            {
                RPC_AnnounceWinner(isCross ? "Player One Wins!" : "Player Two Wins!");
                return;
            }

            if (_playerOneMoves.Count + _playerTwoMoves.Count == 9)
            {
                RPC_AnnounceWinner("It's a Draw!");
                return;
            }

            ChangeTurn();
        }
        else
        {
            bool isCross = _playerTurn == PlayerOneRef;
            RPC_ClientDoneWithTurn(index,isCross);  
        }
    }

    private void ChangeTurn()
    {
        if (!_networkRunner.IsServer)
            return;
        _playerTurn = _playerTurn == PlayerOneRef ? PlayerTwoRef : PlayerOneRef;

        RPC_UpdateTurn(_playerTurn);
    }

    private void UpdatePlayersLabel()
    {
        if (_networkRunner.IsServer)
        {
            var players = _networkRunner.ActivePlayers.ToList();

            if (players.Count >= 1)
                PlayerOneRef = players[0];

            if (players.Count >= 2)
                PlayerTwoRef = players[1];
        }

        if (_networkRunner.LocalPlayer == PlayerOneRef)
        {
            _playerText.text = "Player One";
        }
        else if (_networkRunner.LocalPlayer == PlayerTwoRef)
        {
            _playerText.text = "Player Two";
        }
    }

    private void InitializeGameManager()
    {
        NetworkManager networkManager = NetworkManager.instance;
        _networkRunner = networkManager.GetNetworkRunner();
        _cellButtonList.Clear();
        _cellButtonList = new List<Button>(networkManager.CellButtonList);
        _playerText = networkManager.PlayerText;
        _turnText = networkManager.TurnText;
        InitializePlayerTurn();
        UpdatePlayersLabel();
        _chatHandler = networkManager.GetChatHandler();
        _chatHandler.SetGameManager(this);
    }

    private void AddListener()
    {
        for (int i = 0; i < _cellButtonList.Count; i++)
        {
            int index = i;
            _cellButtonList[i].onClick.RemoveListener(() => OnCellClick(index));
            _cellButtonList[i].onClick.AddListener(() => OnCellClick(index));
        }
    }

    private void InitializePlayerTurn()
    {
        if (_networkRunner.IsServer)
        {
            _playerTurn = _networkRunner.LocalPlayer;
            RPC_UpdateTurn(_playerTurn);
        }
        else
        {
            _playerTurn = PlayerRef.None;
        }
        UpdateTurnUI();
    }

    private void UpdateTurnUI()
    {
        if (_playerTurn == _networkRunner.LocalPlayer)
        {
            _turnText.text = "Your turn!";
        }
        else
        {
            _turnText.text = "Waiting for opponent...";
        }
    }

    private readonly List<int[]> winningCombos = new List<int[]>
    {
        new int[] {0, 1, 2}, 
        new int[] {3, 4, 5},
        new int[] {6, 7, 8}, 
        new int[] {0, 3, 6}, 
        new int[] {1, 4, 7}, 
        new int[] {2, 5, 8},
        new int[] {0, 4, 8}, 
        new int[] {2, 4, 6} 
    };

    private void UpdateCollectedCells(PlayerRef player, int index)
    {
        if (player == PlayerOneRef)
            _playerOneMoves.Add(index);
        else if (player == PlayerTwoRef)
            _playerTwoMoves.Add(index);
    }

    private bool CheckWin(List<int> playerMoves)
    {
        foreach (var combo in winningCombos)
        {
            if (combo.All(playerMoves.Contains))
            {
                Debug.Log("Wins");
                return true; 
            }
        }
        return false; 
    }

    #region RPC
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateCellVisual(int buttonIndex, bool isCross)
    {
        var cellHandler = _cellButtonList[buttonIndex].GetComponent<CellHandler>();
        if (isCross)
            cellHandler.OnActivateCross();
        else
            cellHandler.OnActivateCircle();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateTurn(PlayerRef player)
    {
        _playerTurn = player;
        UpdateTurnUI();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ClientDoneWithTurn(int index, bool isCross)
    {
        if (!_networkRunner.IsServer)
            return;

        UpdateCollectedCells(_playerTurn, index);
        RPC_UpdateCellVisual(index, isCross);

        if (CheckWin(_playerTurn == PlayerOneRef ? _playerOneMoves : _playerTwoMoves))
        {
            RPC_AnnounceWinner(_playerTurn == PlayerOneRef ? "Player One Wins!" : "Player Two Wins!");
            return; // Stop turn change if someone won
        }

        // Check if all buttons are clicked and announce a draw if no winner
        if (_playerOneMoves.Count + _playerTwoMoves.Count == 9)
        {
            RPC_AnnounceWinner("It's a Draw!");
            return; // Stop turn change if it's a draw
        }

        ChangeTurn();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_AnnounceWinner(string winnerMessage)
    {
        _turnText.text = winnerMessage;
        
        foreach (var button in _cellButtonList)
        {
            button.interactable = false;
        }
    }
    #endregion
}