using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    // Sync this property across the network
    [Networked] public int PlayerId { get; set; }

    // Called when this object is spawned in the game
    public override void Spawned()
    {
        Debug.Log($"Player {PlayerId} spawned.");
    }

    // Additional player-related logic (like movement or actions) goes here
}