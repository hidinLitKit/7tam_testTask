using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class PlayerNameDisplayer : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerNameField;
    [SerializeField][SyncVar(hook ="PlayerNameChanged")] public string playerDisplayName;
    private void Start()
    {
        if(!isLocalPlayer) return;
        CmdSendName(PlayerPrefs.GetString("PlayerName"));
    } 
    [Command]
    public void CmdSendName(string playerName)
    {
        playerDisplayName = playerName;
    }
    public void PlayerNameChanged(string oldName, string newName)
    {
        playerNameField.text = newName;
    }
}
