using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Coin : NetworkBehaviour
{   
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        GetComponent<SpriteRenderer>().enabled = true;
        RpcInitializeCoin();
    }
    [ClientRpc]
    void RpcInitializeCoin()
    {
        this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }
}
