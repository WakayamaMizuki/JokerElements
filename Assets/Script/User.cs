using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class User : MonoBehaviour {

    GameObject GameController;
    Game game;
    public int PlayerNumber;
    GamePlayer player;
    GamePlayer enemy;
    //Player Enemy;
	// Use this for initialization
	void Start () {
        PlayerNumber = Random.Range(0, 2);
        GameObject.Find("UIController").GetComponent<UIScript>().PlayerNumber = PlayerNumber;
        GameObject.Find("UIController").GetComponent<CardScript>().PlayerNumber = PlayerNumber;
        GameObject.Find("NPCController").GetComponent<NPCScript>().PlayerNumber = PlayerNumber + 1;

        GameController = GameObject.Find("GameController");
        game = GameController.GetComponent<Game>();
        player = game.Player[PlayerNumber];
        enemy = game.Player[(PlayerNumber + 1) % 2];
    }
	
	// Update is called once per frame
	void Update () {
        if(player == null) player = game.Player[PlayerNumber];
        if(enemy == null) enemy = game.Player[(PlayerNumber + 1) % 2];

        while (player != null && player.HandCard.Count < 5 && player.Deck.Count > 0)
        {
            player.DrawCard();
        }
        if (player != null && player.MyTurn && Input.GetMouseButtonDown(0)) Clicked();

        CheckWin();
    }

    void Clicked()
    {
        Ray ray = new Ray();
        RaycastHit hit = new RaycastHit();
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //マウスクリックした場所からRayを飛ばし、オブジェクトがあればtrue 
        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject != null && hit.collider.gameObject.CompareTag("HandCard"))
            {
                //カードをクリック
                CardChoice(hit.collider.gameObject);
            }else if (hit.collider.gameObject != null && hit.collider.gameObject.CompareTag("Field"))
            {
                //場をクリック
                CardPlace();
            }
        }

    }

    void CardChoice(GameObject ClickCard)
    {
        if(ClickCard.name.IndexOf("HandCard") >= 0)
        {
            int n = int.Parse(ClickCard.name.Remove(ClickCard.name.IndexOf("HandCard"), 8));
            player.HandCard[n].IsPrepare = !player.HandCard[n].IsPrepare;
        }
    }

    void CardPlace()
    {
        //カードを提出
        List<Card> p = new List<Card>();
        for (int i = 0; i < player.HandCard.Count; i++)
        {
            if (player.HandCard[i].IsPrepare)
            {
                p.Add(player.HandCard[i]);
            }
        }
        if (p.Count > 0)
        {
            player.CardRequest = new PlaceCard(p);
            player.CardChanged = true;

        }
    }

    void CheckWin()
    {
        //勝ちかどうか
        if (player != null && player.Deck.Count <= 0 && player.HandCard.Count <= 0)
        {
            SceneManager.LoadScene("Win");
        }

        if (enemy != null && enemy.Deck.Count <= 0 && enemy.HandCard.Count <= 0)
        {
            SceneManager.LoadScene("Lose");
        }
    }
}
