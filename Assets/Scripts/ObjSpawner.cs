using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
public class ObjSpawner : NetworkBehaviour
{
    public GameObject coinspwn;
    private void Start()
    {
        DontDestroyOnLoad(this);
        GameObject newSpawner = Instantiate(coinspwn);
        NetworkServer.Spawn(newSpawner);
    }

}
