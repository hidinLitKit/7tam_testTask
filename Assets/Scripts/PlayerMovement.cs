using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    private CharacterController2D controller;
    private Rigidbody2D rb;
    private float speed = 60f;
    private float horizontalMove = 0f;
    private bool jump = false;
    [Header("MatchMaking")]
    public static PlayerMovement localPlayer;
    [SyncVar] public string matchID;
    private NetworkMatch networkMatch;
    void Start()
    {
        networkMatch = GetComponent<NetworkMatch>();
        controller = GetComponent<CharacterController2D>();
        rb = GetComponent<Rigidbody2D>();
        if(isLocalPlayer)
        {
            localPlayer = this;
            //gameObject.GetComponent<PlayerNameDisplayer>().CmdSendName(LobbyManager.instance.DisplayName);
        }
        else
        {
            LobbyManager.instance.SpawnPlayerUIPrefab(this);
        }
    }
    
    void Update()
    {
        if(!isLocalPlayer) return;
        horizontalMove = Input.GetAxisRaw("Horizontal")*speed;
        if(Input.GetButtonDown("Jump")) jump = true;
    }
    private void FixedUpdate()
    {
        controller.Move(horizontalMove*Time.fixedDeltaTime,false,jump);
        jump = false;
    }

    public void HostGame()
    {
        string ID = LobbyManager.GenerateRandomID();
        CmdHostGame(ID);
    }
    [Command]
    public void CmdHostGame(string ID)
    {
        matchID = ID;
        if(LobbyManager.instance.HostGame(ID,gameObject))
        {
            networkMatch.matchId = ID.ToGuid();
            TargetHostGame(true,ID);
        }
        else
        {
            TargetHostGame(false, ID);
        }
    }
    [TargetRpc]
    void TargetHostGame(bool success, string ID)
    {
        matchID = ID;
        Debug.Log($"ID{matchID} == {ID}");
        LobbyManager.instance.HostSuccess(success,ID);
    }

    public void JoinGame(string inputID)
    {
        CmdJoinGame(inputID);
    }
    [Command]
    public void CmdJoinGame(string ID)
    {
        matchID = ID;
        if(LobbyManager.instance.JoinGame(ID,gameObject))
        {
            networkMatch.matchId = ID.ToGuid();
            TargetJoinGame(true,ID);
        }
        else
        {
            TargetJoinGame(false, ID);
        }
    }
    [TargetRpc]
    void TargetJoinGame(bool success, string ID)
    {
        matchID = ID;
        Debug.Log($"ID{matchID} == {ID}");
        LobbyManager.instance.JoinSuccess(success,ID);
    }

    public void BeginGame()
    {
        CmdBeginGame();
    }
    [Command]
    public void CmdBeginGame()
    {
        LobbyManager.instance.BeginGame(matchID);
    }
    public void StartGame()
    {
        TargetBeginGame();
    }
    [TargetRpc]
    void TargetBeginGame()
    {
        Debug.Log($"ID{matchID} | begin");
        DontDestroyOnLoad(gameObject);
        LobbyManager.instance.inGame = true;
        Debug.Log("Weapon Enabled");
        SceneManager.LoadScene("Game",LoadSceneMode.Additive);

    }
}
