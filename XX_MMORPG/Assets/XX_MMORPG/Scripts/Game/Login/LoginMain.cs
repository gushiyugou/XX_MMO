using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginMain : MonoBehaviour
{
    [SerializeField]private string host = "127.0.0.2";
    [SerializeField]private int port = 32510;
    public TMP_InputField userName;
    public TMP_InputField password;
    public Button loginButton;
    private void Start()
    {
        NetClient.ConnectToServer(host, port);
        loginButton.onClick.AddListener(Login);
    }

    private void Update()
    {
        
    }

    public void Login()
    {
        var message = new Proto.UserLoginRequest();
        message.Username = userName.text;
        message.Password = password.text;
        NetClient.SendRequest(message);
    }
}
