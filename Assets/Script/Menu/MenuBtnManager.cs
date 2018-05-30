using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuBtnManager : Photon.MonoBehaviour {

    public GameObject signinMenu;
    public GameObject MainMenuBtns;
    public GameObject book;
    public GameObject[] bookInside = new GameObject[2];
    //public InputField nameInputField;
    //public InputField passwordInputField;

    //bool IsConnected = false;

    #region Btns Method
    public void SignIn()
    {
        Debug.Log("Sign In Clicked");
        signinMenu.SetActive(false);
        MainMenuBtns.SetActive(true);
    }

    public void StartGame()
    {
        BtnMatchUI(0);
    }

    public void TeachingMode()
    {
        SceneManager.LoadScene("TeachingModeScene");
    }

    public void ScoreBoard()
    {
        BtnMatchUI(1);
    }

    public void SetGame()
    {
        BtnMatchUI(2);
    }

    public void SignoutGame()
    {
        Debug.Log("登出");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    #endregion

    void BtnMatchUI(int _btn)
    {
        if (book.activeSelf == false)
        {
            book.SetActive(true);
        }
        
        for (int i = 0; i < bookInside.Length; i++)
        {
            if (i == _btn) continue;

            bookInside[i].SetActive(false);
        }
        bookInside[_btn].SetActive(true);
    }
}
