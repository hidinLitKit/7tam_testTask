using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class Winner : NetworkBehaviour
{
    public List<PlayerMovement> players = new List<PlayerMovement>();
    public GameObject WinScreen;
    [SyncVar] public int playerCount;
    [SerializeField] private TMP_Text winTxt;
    void Start()
    {
        playerCount = players.Count;
        DontDestroyOnLoad(this.gameObject);
    }
    [Command]
    public void CmdShowWinner(PlayerMovement winner)
    {
            WinScreen.SetActive(true);
            winTxt.text = "Winner is " + 
            winner.gameObject.GetComponent<PlayerInfoDisplayer>().playerDisplayName + "and his coin count is" + 
            winner.gameObject.GetComponent<PlayerInfoDisplayer>().playerCoinCount;
    }
}
