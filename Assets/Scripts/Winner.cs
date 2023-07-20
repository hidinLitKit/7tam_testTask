using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class Winner : NetworkBehaviour
{
    public List<PlayerMovement> players = new List<PlayerMovement>();
    public GameObject WinScreen;
    public static Winner instance;
    public GameObject[] winner;
    [SyncVar(hook ="playerCountChanged")] public int playerCount;
    [SerializeField] private TMP_Text winTxt;
    private List<bool> isChecked;
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(instance);
        winner = GameObject.FindGameObjectsWithTag("Player");
        playerCount = players.Count;       
        bool[] isChecked = new bool[playerCount];
        for(int i = 0;i<isChecked.Length;i++)
        {
            isChecked[i] = false;
        }
    }
    
    public void playerCountChanged(int oldValue, int newValue)
    {
        if (playerCount==1) 
        {
            GameObject trueWinner = winner[0];
            foreach(GameObject win in winner)
            {
                if(win.GetComponent<PlayerMovement>().isDead ==false) trueWinner = win;
            }
            WinScreen.SetActive(true);
            winTxt.text = "Winner is " + 
            trueWinner.GetComponent<PlayerInfoDisplayer>().playerDisplayName + "and his coin count is " + 
            trueWinner.GetComponent<PlayerInfoDisplayer>().playerCoinCount;
        }
    }
}
