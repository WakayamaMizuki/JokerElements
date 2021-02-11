//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UIButton : MonoBehaviour {

    private void Awake()
    {
        Screen.SetResolution(1880, 1080, false, 60);
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) Quit();
    }

    public void Lose()
    {
        SceneManager.LoadScene("Lose");
    }

    public void Win()
    {
        SceneManager.LoadScene("Win");
    }

    public void GameNPC()
    {
        GamePlayerNumber.SetNum(Random.Range(0, 2));
        SceneManager.LoadScene("GameNPC");
    }

    public void Title()
    {
        SceneManager.LoadScene("Title");
    }

    public void GamePUN()
    {
        SceneManager.LoadScene("Lobby");
        //SceneManager.LoadScene("MIJISSOU");
    }

    void Quit()
    {
        
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
      UnityEngine.Application.Quit();
#endif
    }
}
