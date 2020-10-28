using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class UIScript : MonoBehaviour {
    private Text cardText;
    private Text EnemycardText;
    //private Text text1;
    //private Text text2;
    private Text text3;
    private Text text4;
    private Text Name;
    private Text Status;

    public int PlayerNumber;
    private bool[] markRegulation = new bool[4];
    
    private GamePlayer player;
    private GamePlayer enemy;

    private GameObject GameController;
    private GameObject[] mark = new GameObject[4];
    private GameObject Turn;
    private Transform TurnTransform;
    private Game game;
    private UserCardScript userCardScript;
    // Use this for initialization
    void Start () {
        cardText = GameObject.Find("MyCardNum").GetComponent<Text>();
        EnemycardText = GameObject.Find("EnemyCardNum").GetComponent<Text>();

        text3 = GameObject.Find("Text3").GetComponent<Text>();
        text4 = GameObject.Find("Text4").GetComponent<Text>();
        Status = GameObject.Find("Status").GetComponent<Text>();
        if(GameObject.Find("Name") != null){
            Name = GameObject.Find("Name").GetComponent<Text>();
            Name.text = "名前: " + UserName.Name;
        }
        

        if (GameObject.Find("GameController") != null)
        {
            GameController = GameObject.Find("GameController");
            game = GameController.GetComponent<Game>();
        }
        else if (GameObject.Find("GameController(Clone)") != null)
        {
            GameController = GameObject.Find("GameController(Clone)");
            game = GameController.GetComponent<Game>();
        }
        Turn = GameObject.Find("Turn");
        TurnTransform = Turn.GetComponent<Transform>();
        
        /*
        if (GameObject.Find("UserController").GetComponent<UserPUN>() != null)
        {
            PlayerNumber = GameObject.Find("UserController").GetComponent<UserPUN>().PlayerNumber;
        }
        else
        {
            PlayerNumber = GameObject.Find("UserController").GetComponent<User>().PlayerNumber;
        }*/

        if(game != null)
        {
            player = game.Player[PlayerNumber];
            enemy = game.Player[(PlayerNumber + 1) % 2];
        }
        

        if(PlayerNumber == 0)
        {
            text4.text = "先攻" ;
        }
        else
        {
            text4.text = "後攻";
        }

        mark[0] = GameObject.Find("Spade");
        mark[1] = GameObject.Find("Club");
        mark[2] = GameObject.Find("Heart");
        mark[3] = GameObject.Find("Diamond");
        for(int i = 0;i < 4; i++)
        {
            mark[i].SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (userCardScript == null)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetComponent<PhotonView>().IsMine) userCardScript = players[i].GetComponent<UserCardScript>();
            }
        }

        if (game == null && GameObject.Find("GameController") != null)
        {
            GameController = GameObject.Find("GameController");
            game = GameObject.Find("GameController").GetComponent<Game>();
        }
        else if (GameObject.Find("GameController(Clone)") != null)
        {
            GameController = GameObject.Find("GameController(Clone)");
            game = GameObject.Find("GameController(Clone)").GetComponent<Game>();
        }

        if (PlayerNumber == 0)
        {
            text4.text = "先攻";
        }
        else
        {
            text4.text = "後攻";
        }
        if (game != null)
        {
            player = game.Player[PlayerNumber];
            enemy = game.Player[(PlayerNumber + 1) % 2];
        }
        if (enemy != null){
            cardText.text = "残り: " + player.Deck.Count;
            EnemycardText.text = "残り: " + enemy.Deck.Count;

            if (game.numberRegulation != -1)
            {
                text3.text = "縛り: " + (game.numberRegulation % 14 + game.numberRegulation / 14);
            }
            else
            {
                text3.text = "縛り:";
            }
        }

        markRegulation = game.markRegulation;
        for(int i = 0;i < markRegulation.Length; i++)
        { 
            mark[i].SetActive(markRegulation[i]);
        }

        if (player.MyTurn)
        {
            TurnTransform.position = new Vector3(-4, -3, 0);
        }
        else
        {
            TurnTransform.position = new Vector3(-4, 3, 0);
        }


        if (player != null && enemy != null) ChangeStatus();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //QuitApplication();
        }

        

        if (Input.GetKey(KeyCode.Escape)) Quit();
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

    void ChangeStatus()
    {
        if (player.Is7)
        {
            Status.text = "7渡しできます";
        }
        else if (player.Is10)
        {
            Status.text = "10捨てできます";
        }
        else if (enemy.Is7)
        {
            Status.text = "7渡しされます";
        }
        else if (enemy.Is10)
        {
            Status.text = "10捨てされます";
        }
        else if ((enemy.placeCard != null && enemy.placeCard.count8 > 0) || (player.placeCard != null && player.placeCard.count8 > 0))
        {
            Status.text = "8切り";
        }
        else
        {
            Status.text = "";
        }
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
