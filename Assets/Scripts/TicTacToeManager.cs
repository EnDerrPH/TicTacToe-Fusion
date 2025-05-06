using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using Fusion;
using System.Linq;

public class TicTacToeManager : NetworkBehaviour
{
    [SerializeField] TMP_Text _playerText;
    [SerializeField] TMP_Text _turnText;
    [SerializeField] List<Button> _cellButtons = new List<Button>();
    [SerializeField] int _playerNumber;
    [SerializeField] private List<int> _collectedNumbers = new List<int>();
    [SerializeField] GameObject _gameOverPanel;
    private bool _isPlayerOneTurn = true;
    private bool _gameOver;
    [SerializeField] int _turnCount;
    NetworkRunner _networkRunner;
    GameManager _gameManager;
    private string _symbol;
    void Start()
    {
        AddListener();
    }

    public void InitializeComponents()
    {
        _networkRunner = NetworkManager.instance.networkRunner;
        _gameManager = GameManager.instance;
    }

    public void SetPlayerText(string player, int number)
    {
        _playerText.text = player;
        _playerNumber = number;
    }

    public void SetTurnText(string turn)
    {
        _turnText.text = "Turn : " + turn;
    }

    private void AddListener()
    {
        int index = 0;
        foreach (Button button in _cellButtons)
        {
            int currentIndex = index;
            button.onClick.AddListener(() => OnCellClicked(currentIndex));
            index++;
        }
    }

    void OnCellClicked(int index)
    {
        var players = _networkRunner.ActivePlayers.ToList();
        if (_playerNumber == 1 && _gameManager.playersTurn == players[0])
        {
            _symbol = "O";
        }
        else if (_playerNumber == 2 && _gameManager.playersTurn == players[1])
        {
            _symbol = "X";
        }
        else
        {
            return;
        }

        if (index >= 0 && index < _cellButtons.Count)
        {
            _collectedNumbers.Add(index);
            // Call RPC so everyone sees the same update
            RPC_UpdateCell(index, _symbol);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_UpdateCell(int index, string symbol)
    {
        _turnCount++;
        if (_turnCount >= 9)
        {
            RPC_ShowGameOverPanel();
        }
        CheckWinState(_collectedNumbers);
        if (index >= 0 && index < _cellButtons.Count)
        {
            _cellButtons[index].interactable = false;
            TextMeshProUGUI text = _cellButtons[index].GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = symbol;
            }
        }
        _gameManager.ToggleTurn();
        ToggleTurnText();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ShowGameOverPanel()
    {
        TextMeshProUGUI text = _gameOverPanel.GetComponentInChildren<TextMeshProUGUI>();
        if (CheckWinState(_collectedNumbers))
        {
            text.text = "Game is Over, Thank you for playing!";
        }
        _gameOverPanel.SetActive(true);

        if (_turnCount >= 9 && !CheckWinState(_collectedNumbers))
        {
            text.text = "Draw";
            _gameOverPanel.SetActive(true);
            return;
        }

    }

    private void ToggleTurnText()
    {
        if (_gameOverPanel.activeSelf)
        {
            return;
        }
        _isPlayerOneTurn = !_isPlayerOneTurn;
        _turnText.text = _isPlayerOneTurn ? "Player One" : "Player Two";
    }

    public bool CheckWinState(List<int> playerMoves)
    {
        if (_gameOver)
        {
            return true;
        }
               
        foreach (var combo in WinningCombinations)
            {
                if (combo.All(move => playerMoves.Contains(move)))
                {
                    if (_gameOver)
                    {
                        break;
                    }
                    RPC_SetGameOverState(true);
                    RPC_ShowGameOverPanel();
                    return true;
                }
            }
        return false;
    }



    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetGameOverState(bool gameOver)
    {
        _gameOver = gameOver;
    }

    private static readonly int[][] WinningCombinations = new int[][]
    {
        new int[] { 0, 1, 2 }, // Row 1
        new int[] { 3, 4, 5 }, // Row 2
        new int[] { 6, 7, 8 }, // Row 3
        new int[] { 0, 3, 6 }, // Column 1
        new int[] { 1, 4, 7 }, // Column 2
        new int[] { 2, 5, 8 }, // Column 3
        new int[] { 0, 4, 8 }, // Diagonal \
        new int[] { 2, 4, 6 }  // Diagonal /
    };
}
