using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
using System;
using System.Security.Cryptography;
using System.Text;

[System.Serializable]
public class Match: NetworkBehaviour
{
    private string id;
    public string ID
    {
        get{return id;}
        set{id = value;}
    }
    public readonly List<GameObject> players = new List<GameObject>();
    public Match(string ID, GameObject player)
    {
        this.ID = ID;
        players.Add(player);
    }
}

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager instance;
    public readonly SyncList<Match> matches = new SyncList<Match>(); 
    public readonly SyncList<string> matchIDs = new SyncList<string>();
    public TMP_InputField JoinInput;
    public Button HostButton;
    public Button JoinButton;
    public Canvas LobbyCanvas;
    public Transform UIPlayerParent;
    public GameObject UIPlayerPrefab;
    public TMP_Text IDText;
    public Button BeginGameButton;
    public GameObject TurnManager;
    public GameObject CoinSpawner;
    public GameObject WinnerDetector;
    public bool inGame;
    [SyncVar] public string DisplayName;
    private void Start()
    {
        instance = this;
    }
    private void Update()
    {
        if(!inGame)
        {
            PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
            foreach(PlayerMovement player in players)
            {
                Debug.Log("Weapon Disabled");
                player.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
            }
        }
    }
    public void HostBttClick()
    {
        JoinInput.interactable = false;
        HostButton.interactable = false;
        JoinButton.interactable = false;
        PlayerMovement.localPlayer.HostGame();
    }


    public void HostSuccess(bool success, string matchID)
    {
        if(success)
        {
            LobbyCanvas.enabled = true;
            SpawnPlayerUIPrefab(PlayerMovement.localPlayer);
            IDText.text = matchID;
            BeginGameButton.interactable = true;
        }
        else
        {
            JoinInput.interactable = true;
            HostButton.interactable = true;
            JoinButton.interactable = true;
        }
    }

    public void JoinBttClick()
    {
        JoinInput.interactable = false;
        HostButton.interactable = false;
        JoinButton.interactable = false;
        PlayerMovement.localPlayer.JoinGame(JoinInput.text.ToUpper());
    }


    public void JoinSuccess(bool success, string matchID)
    {
        if(success)
        {
            LobbyCanvas.enabled = true;
            SpawnPlayerUIPrefab(PlayerMovement.localPlayer);
            IDText.text = matchID;
            BeginGameButton.interactable = false;
        }
        else
        {
            JoinInput.interactable = true;
            HostButton.interactable = true;
            JoinButton.interactable = true;
        }
    }
    public bool HostGame(string matchID, GameObject player)
    {
        if(!matchIDs.Contains(matchID))
        {
            matchIDs.Add(matchID);
            matches.Add(new Match(matchID,player));
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool JoinGame(string matchID, GameObject player)
    {
        if(matchIDs.Contains(matchID))
        {
            foreach(Match mtch in matches)
            {
                if (mtch.ID == matchID)
                {
                    mtch.players.Add(player);
                    break;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }
    public static string GenerateRandomID()
    {
        string ID = string.Empty;
        for(int i = 0;i<5;i++)
        {
            int rand = UnityEngine.Random.Range(0,36);
            if(rand<26) ID += (char)(rand+65);
            else ID+=(rand-26).ToString();
        }
        return ID;
    }
    public void SpawnPlayerUIPrefab(PlayerMovement player)
    {
        GameObject newUIPlayer = Instantiate(UIPlayerPrefab,UIPlayerParent);
        newUIPlayer.GetComponent<PlayerUI>().SetPlayer(player.GetComponent<PlayerInfoDisplayer>().playerDisplayName);
    }
    public void StartGame()
    {
        PlayerMovement.localPlayer.BeginGame();
    }
    public void BeginGame(string matchID)
    {
        GameObject newTurnManager = Instantiate(TurnManager);
        NetworkServer.Spawn(newTurnManager);
        newTurnManager.GetComponent<NetworkMatch>().matchId = matchID.ToGuid();
        TurnManager turnManager = newTurnManager.GetComponent<TurnManager>();
        if(isServer)
        {
            GameObject newCoinSpawner = Instantiate(CoinSpawner);
            NetworkServer.Spawn(newCoinSpawner);
            newCoinSpawner.GetComponent<NetworkMatch>().matchId = matchID.ToGuid();
            newCoinSpawner.GetComponent<CoinSpawner>().Spawn(matchID);

            
        }
        foreach(Match mtch in matches)
        {
            if (mtch.ID == matchID)
            {
                foreach(var player in mtch.players)
                {
                    PlayerMovement _player = player.GetComponent<PlayerMovement>();
                    turnManager.AddPlayer(_player);
                    _player.StartGame();
                }
                break;
            }
        }
        if(!isServer) return;
            GameObject winnerDetector = Instantiate(WinnerDetector);
            NetworkServer.Spawn(winnerDetector);
            winnerDetector.GetComponent<NetworkMatch>().matchId = matchID.ToGuid();
            winnerDetector.GetComponent<Winner>().players = turnManager.players;

    }
}
public static class MatchExtension
{
    public static Guid ToGuid(this string id)
    {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
        byte[] inputBytes = Encoding.Default.GetBytes(id);
        byte[] hashBytes = provider.ComputeHash(inputBytes);

        return new Guid(hashBytes);

    }
}
