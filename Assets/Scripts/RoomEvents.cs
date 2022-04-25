using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEvents : MonoBehaviour
{
    // Input fields
    [SerializeField] TMPro.TMP_InputField usernameField;
    [SerializeField] TMPro.TMP_InputField passwordField;

    // The button that loads the scene
    [SerializeField] UnityEngine.UI.Button enterRoomButton;

    // Choose rooms
    [SerializeField] List<UnityEngine.UI.Toggle> rooms = new List<UnityEngine.UI.Toggle>();

    // Text object to log debugs
    [SerializeField] TMPro.TMP_Text debugText;

    private void Start()
    {
        
    }

    public void EnterRoom()
    {
        string roomName = "Room";

        foreach (var room in rooms)
        {
            if(room.isOn)
            {
                roomName += room.GetComponentInChildren<TMPro.TMP_Text>().text;
                break;
            }
        }

        SessionProps props = new SessionProps();
        props.RoomName = roomName;

        NetworkManager.Instance.StartSession(Fusion.GameMode.Single, props);
    }

    public void LogText(string msg)
    {
        if (debugText)
            debugText.text += "\n" + msg;

        Debug.Log(msg);
    }
}
