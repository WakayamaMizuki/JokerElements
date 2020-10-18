using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UIButton : MonoBehaviour {

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
        
        int rnd = Random.Range(0, 2);
        if(rnd == 1)
        {
        
        }
        else
        {

        }
        SceneManager.LoadScene("GameNPC");
    }

    public void Title()
    {
        SceneManager.LoadScene("Title");
    }

    public void GamePUN()
    {
        //string name = GameObject.Find("name").GetComponent<Text>().text;
        //GameObject.Find("UserNameObject").GetComponent<UserName>().NameSet(name);
        //SceneManager.LoadScene("GamePUN");
        SceneManager.LoadScene("MIJISSOU");
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
