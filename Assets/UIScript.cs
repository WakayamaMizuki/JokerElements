using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class UIScript : MonoBehaviour {
    private Text cardText;
    private Text EnemycardText;
    private Text text1;
    private Text text2;
    private Text text3;
    private Text text4;
    private Text Name;

    public int PlayerNumber;
    
    private GamePlayer player;
    private GamePlayer enemy;

    private GameObject GameController;
    private Game game;
    // Use this for initialization
    void Start () {
        cardText = GameObject.Find("MyCardNum").GetComponent<Text>();
        EnemycardText = GameObject.Find("EnemyCardNum").GetComponent<Text>();
        text1 = GameObject.Find("Text1").GetComponent<Text>();
        text2 = GameObject.Find("Text2").GetComponent<Text>();
        text3 = GameObject.Find("Text3").GetComponent<Text>();
        text4 = GameObject.Find("Text4").GetComponent<Text>();
        if(GameObject.Find("Name") != null){
            Name = GameObject.Find("Name").GetComponent<Text>();
            Name.text = "名前: " + UserName.Name;
        }
        

        if (GameObject.Find("GameController") != null)
        {
            GameController = GameObject.Find("GameController");
        }
        else
        {
            GameController = GameObject.Find("GameController(Clone)");
        }
        game = GameController.GetComponent<Game>();
        if (GameObject.Find("UserController").GetComponent<UserPUN>() != null)
        {
            PlayerNumber = GameObject.Find("UserController").GetComponent<UserPUN>().PlayerNumber;
        }
        else
        {
            PlayerNumber = GameObject.Find("UserController").GetComponent<User>().PlayerNumber;
        }

        player = game.Player[PlayerNumber];
        enemy = game.Player[(PlayerNumber+1)%2];


        text4.text = "プレイヤーナンバー : " + PlayerNumber;
    }
	
	// Update is called once per frame
	void Update () {
        player = game.Player[PlayerNumber];
        enemy = game.Player[(PlayerNumber + 1) % 2];
        if (enemy != null){
            cardText.text = "残り: " + player.Deck.Count;
            EnemycardText.text = "残り: " + enemy.Deck.Count;
            text1.text = "手札: " + player.HandCard.Count;
            text2.text = "手札: " + enemy.HandCard.Count;

            if (game.numberRegulation != -1)
            {
                text3.text = "縛り: " + (game.numberRegulation % 14 + 1);
            }
            else
            {
                text3.text = "縛り: なし";
            }
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitApplication();
        }
    }

    public void passClicked()
    {
        if (enemy != null){
            game.pass(PlayerNumber);
            //game.GetComponent<PhotonView>().RPC("pass", RpcTarget.All);
        }
    }

    public void QuitApplication()
    {
        Application.Quit();
    }


}
