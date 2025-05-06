using Fusion;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    [SerializeField] TicTacToeManager _ticTacToeManager;
    public static GameManager instance;
    public PlayerRef playersTurn { get; set; }
    

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void StartGame()
    {
        SetInitialTurn();
    }

    public void EndTurn()
    {
        //check win state
        ToggleTurn();

        if (Runner.IsServer)  // Only call RPC from server
        {
            RPC_EndTurn();
        }

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_EndTurn()
    {
        // Code to execute when turn ends on all clients
        // This can include things like updating UI, switching player, etc.
        Debug.Log("Turn ended! Syncing with all clients.");
        // You can add additional logic for UI updates or anything that needs to be synced
    }

    public void ToggleTurn()
    {
        var players = NetworkManager.instance.networkRunner.ActivePlayers.ToList();
        if (playersTurn == players[0])
        {
            playersTurn = players[1];
        }
        else
        {
            playersTurn = players[0];
        }
    }
    
    void SetInitialTurn()
    {
        var players = NetworkManager.instance.networkRunner.ActivePlayers.ToList();

        if (players.Count > 0)
        {
            playersTurn = players[0];
            Debug.Log("Initial turn set to: " + playersTurn);
            // Optionally, update UI or notify players
        }
        _ticTacToeManager.SetTurnText("Player[1]");
    }
}