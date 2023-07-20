using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Mirror;
public class TitleScreenManger : MonoBehaviour
{
    public static TitleScreenManger instance;

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject menuHostJoin;
    [SerializeField] private GameObject enterNamePanel;
    [SerializeField] private GameObject hostMenuPanel;
    [SerializeField] private GameObject joinMenuPanel;
    [Header("PlayerName UI")]
    [SerializeField] private TMP_InputField playerNameInputField;
    [Header("ID Fields")]
    [SerializeField] private TMP_InputField hostIDField;
    [SerializeField] private TMP_InputField clientIDField;
    [Header("Other")]
    [SerializeField] private Button mainMenuButton;
    private const string PlayerPrefsNameKey = "PlayerName";
    void Awake()
    {
        MakeInstance();
        ReturnToMainMenu();
    }
    public void ReturnToMainMenu()
    {
        mainMenuPanel.SetActive(true);
        enterNamePanel.SetActive(false);
        hostMenuPanel.SetActive(false);
        joinMenuPanel.SetActive(false);
        mainMenuButton.gameObject.SetActive(false);
    }
    //creates an instancce of titleScreenManager
    void MakeInstance()
    {
        if (instance == null) instance = this;
    }
    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        enterNamePanel.SetActive(true);
        GetSavedPlayerName();
        mainMenuButton.gameObject.SetActive(true);
    }
    private void GetSavedPlayerName()
    {
        if(PlayerPrefs.HasKey(PlayerPrefsNameKey))
        {
            playerNameInputField.text = PlayerPrefs.GetString(PlayerPrefsNameKey);
        }
    }
    public void SavePlayerName()
    {
        string playerName = null;
        if(!string.IsNullOrEmpty(playerNameInputField.text))
        {
            playerName = playerNameInputField.text;
            PlayerPrefs.SetString(PlayerPrefsNameKey,playerName);
            enterNamePanel.SetActive(false);
            menuHostJoin.SetActive(true);
        }
    }
    public void HostGame()
    {
        hostMenuPanel.SetActive(true);
        menuHostJoin.SetActive(false);
    }
    public void JoinGame()
    {
        joinMenuPanel.SetActive(true);
        menuHostJoin.SetActive(false);
    }
    
}
