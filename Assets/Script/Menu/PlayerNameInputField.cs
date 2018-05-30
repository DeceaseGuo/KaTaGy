using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : Photon.MonoBehaviour {

    public static string PlayerPrefName = "PlayerName";

    // Use this for initialization
    void Start () {
        InputField _inputField = gameObject.GetComponent<InputField>();
        if (_inputField != null)
        {
            if (PlayerPrefs.HasKey(PlayerPrefName))
            {
                _inputField.text = PlayerPrefs.GetString(PlayerPrefName);
            }
        }

    }
        
	public void SetPlayerName(Text value)
    {
        PhotonNetwork.playerName = value.text;
        //Move.playerName = value.text;
        PlayerPrefs.SetString(PlayerPrefName, value.text);
    }

}
