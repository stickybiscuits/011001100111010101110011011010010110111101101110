using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UsernameField : MonoBehaviour
{
    const string USERNAME_PREF_KEY = "Username";

    [SerializeField] TMPro.TMP_InputField usernameField;
    [SerializeField] Button enterRoomButton;
    [SerializeField] TMPro.TMP_Text usernameWarning;

    // Text object to log debugs
    [SerializeField] TMPro.TMP_Text debugText;

    private void Start()
    {
        usernameWarning.enabled = false;

        string defaultUsername = "user_" + Environment.TickCount % 99;

        if (PlayerPrefs.HasKey(USERNAME_PREF_KEY))
            defaultUsername = PlayerPrefs.GetString(USERNAME_PREF_KEY);

        if (usernameField != null)
            usernameField.text = defaultUsername;

        LogText("Username is set to <color=yellow><b>" + defaultUsername + "</b></color>.");
    }

    public void OnUsernameTextChanged(string value)
    {
        if (value.Length >= 3)
        {
            enterRoomButton.interactable = true;
            usernameWarning.enabled = false;
        }
        else
        {
            enterRoomButton.interactable = false;
            usernameWarning.enabled = true;
        }
    }

    public void SetPlayerName()
    {
        string nickname = usernameField.text;

        if (nickname == string.Empty)
            return;

        PlayerPrefs.SetString(USERNAME_PREF_KEY, nickname);
    }

    public void LogText(string msg)
    {
        if (debugText)
            debugText.text += "\n" + msg;

        Debug.Log(msg);
    }
}
