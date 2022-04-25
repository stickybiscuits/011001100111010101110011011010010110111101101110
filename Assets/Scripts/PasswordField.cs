using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasswordField : MonoBehaviour
{
    [SerializeField] TMPro.TMP_InputField passwordField;

    private void Start()
    {
        passwordField.text = string.Empty;
    }

    public void EnterRoom()
    {

    }
}
