using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    private CharacterController2D controller;
    private Rigidbody2D rb;
    private float speed = 60f;
    private float horizontalMove = 0f;
    private bool jump = false;
    private Camera mainCam;
    private int money = 0;
    private int HP = 100;
    float parentRotation;
    float bulletRotation;
    Vector3 moveVector;

    [Header("Joysticks")]

    private GameObject ControllerCanvas;
    private FixedJoystick MoveJoystick;
    //private FixedJoystick ShootJoyStick;
    [SerializeField] private GameObject RotateWeapon;
    private Transform firePoint;
    private bool canShoot = true;
    [SerializeField] private GameObject bulletPrefab;
    private bool isAiming = false;
    private bool isDead = false;
    private bool isInGame = false;
    [Header("MatchMaking")]
    public static PlayerMovement localPlayer;
    [SyncVar] public string matchID;
    private NetworkMatch networkMatch;
    void Start()
    {
        networkMatch = GetComponent<NetworkMatch>();
        controller = GetComponent<CharacterController2D>();
        rb = GetComponent<Rigidbody2D>();
        ControllerCanvas = GameObject.Find("ControllerCanvas");
        MoveJoystick = ControllerCanvas.transform.GetChild(0).GetComponent<FixedJoystick>();
        //ShootJoyStick = ControllerCanvas.transform.GetChild(1).GetComponent<FixedJoystick>();
        firePoint = RotateWeapon.transform.GetChild(0).transform;
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
        //parentRotation = RotateWeapon.transform.rotation.eulerAngles.z;
        //bulletRotation = parentRotation + 180f;
        horizontalMove = MoveJoystick.Horizontal*speed;
        if(MoveJoystick.Vertical>0) jump = true;
        if (mainCam==null) mainCam = Camera.main;
        CameraMovement();
        //isAiming = (ShootJoyStick.Horizontal != 0 || ShootJoyStick.Vertical != 0);
        //moveVector = (Vector3.up * ShootJoyStick.Vertical - Vector3.left * ShootJoyStick.Horizontal);
        //if (isAiming) RotateWeapon.transform.rotation = Quaternion.LookRotation(Vector3.forward, moveVector);
        //if (isAiming && canShoot)
        //{
        //    Debug.Log("SHOOT");
        //    CmdShoot(Quaternion.Euler(0f, 0f, bulletRotation));
        //}
        if(Input.GetButtonDown("Fire1")) CmdShoot();
        if(!isDead && HP<=0)
        {
            HP=0;
            GetComponent<PlayerInfoDisplayer>().CmdSendHp(HP);
            GetComponent<Rigidbody2D>().gravityScale = 0f;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
            GetComponent<Collider2D>().enabled = false;
            isDead= true;
            GameObject.Find("WinnerScreen(Clone)").GetComponent<Winner>().playerCount--;
        }
        if(!isDead && isInGame && GameObject.Find("WinnerScreen(Clone)").GetComponent<Winner>().playerCount==1)
        {
            GameObject.Find("WinnerScreen(Clone)").GetComponent<Winner>().CmdShowWinner(GetComponent<PlayerMovement>());
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(!isLocalPlayer) return;
        if (col.gameObject.tag=="Ammunition")
        {
            HP-=15;
            GetComponent<PlayerInfoDisplayer>().CmdSendHp(HP);
        }
        if(col.gameObject.tag == "Money")
        {
            Destroy(col.gameObject);
            money+=1;
            GetComponent<PlayerInfoDisplayer>().CmdSendCoins(money);
        }
    }
    [Command]
    public void CmdShoot()
    {
        GameObject Bullet = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bc = Bullet.GetComponent<Bullet>();
        NetworkServer.Spawn(Bullet);
        Bullet.GetComponent<NetworkMatch>().matchId = GetComponent<NetworkMatch>().matchId;
        //StartCoroutine(weaponCD());
    }
    //IEnumerator weaponCD()
    //{
    //    canShoot = false;
    //    yield return new WaitForSeconds(0.5f);
    //    canShoot = true;
    //}
    private void FixedUpdate()
    {
        controller.Move(horizontalMove*Time.fixedDeltaTime,false,jump);
        jump = false;
    }
    private void CameraMovement()
    {
        mainCam.transform.localPosition = new Vector3(transform.position.x,transform.position.y,-1f);
        transform.position = Vector2.MoveTowards(transform.position,mainCam.transform.localPosition,Time.deltaTime);
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
        PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
        foreach(PlayerMovement pl in players)
        {
            DontDestroyOnLoad(pl);
        }
        LobbyManager.instance.inGame = true;
        Debug.Log("Weapon Enabled");
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        SceneManager.LoadScene("Game");
        mainCam= null;
        isInGame = true;

    }
}
