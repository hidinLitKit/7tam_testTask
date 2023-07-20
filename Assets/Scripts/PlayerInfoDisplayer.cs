using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class PlayerInfoDisplayer : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerNameField;
    [SerializeField] private TMP_Text playerCoinField;
    [SerializeField] private TMP_Text playerHealthBar;
    [SerializeField][SyncVar(hook ="PlayerNameChanged")] public string playerDisplayName;
    [SerializeField][SyncVar(hook="CoinCountChanged")] public string playerCoinCount;
    [SerializeField][SyncVar(hook="PlayerHealthChanged")] public string playerHpTotal;
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
    [Command]
    public void CmdSendCoins(int coin)
    {
        playerCoinCount = coin.ToString();
    }
    [Command]
    public void CmdSendHp(int hp)
    {
        playerHpTotal = hp.ToString();
    }
    public void PlayerNameChanged(string oldName, string newName)
    {
        playerNameField.text = newName;
    }
    public void CoinCountChanged(string oldValue, string newValue)
    {
        playerCoinField.text = newValue;
    }
    public void PlayerHealthChanged(string oldValue, string newValue)
    {
        playerHealthBar.text = newValue;
    }
    
}
