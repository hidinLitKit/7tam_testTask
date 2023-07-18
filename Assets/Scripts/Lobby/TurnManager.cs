using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class TurnManager : MonoBehaviour
{
    public List<PlayerMovement> players = new List<PlayerMovement>();
    public void AddPlayer(PlayerMovement player)
    {
        players.Add(player);
    }
}
