using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button startButton;
    [SerializeField] private Text PlayerNameText;
    [SerializeField] private Button buttonDisconnect;
    
    [SerializeField] private MultiplayerManager multiplayerManager;
    public static UIManager Instance;
    public string PlayerName;
    void Start()
    {
        if (Instance == null)
            Instance = this;    
        startButton.onClick.AddListener(()=> StartButtonClicked());
        buttonDisconnect.onClick.AddListener(() => DisConnectButtonClicked());
        
    }

    public void DisConnectButtonClicked()
    {
        multiplayerManager.Disconnect();
        Time.timeScale = 1;
    }

    private void StartButtonClicked()
    {
        PlayerName = PlayerNameText.text;
        multiplayerManager.Connect("DefaultRoom");
        panel.SetActive(false);
        startButton.gameObject.SetActive(false);
        buttonDisconnect.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!panel.activeInHierarchy)
            {
                panel.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
            }
            else
            {
                panel.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1;
            }
           
        }
    }
}
