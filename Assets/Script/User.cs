//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class User : MonoBehaviour {

    GameObject GameController;
    Game game;
    public int PlayerNumber;
    UserCardScript userCardScript;
    GamePlayer player;
    //Player Enemy;
	// Use this for initialization
	void Start () {
        PlayerNumber = GamePlayerNumber.num;
        GameController = GameObject.Find("GameController");
        game = GameController.GetComponent<Game>();
        if (game.IsPhoton)userCardScript = GameObject.Find("UserCard").GetComponent<UserCardScript>();
        
        player = game.Player[PlayerNumber];

    }
	
	// Update is called once per frame
	void Update () {
        if (game.IsPhoton)
        {
            player = game.Player[PlayerNumber];
        }

        if (game.Player[PlayerNumber] != null && game.Player[PlayerNumber].HandCard.Count < 5 && game.Player[PlayerNumber].Deck.Count > 0)
        {
            game.DrawCard(PlayerNumber, 5 - game.Player[PlayerNumber].HandCard.Count);

            if (game.IsPhoton) userCardScript.SetPlayer(game.Player);
        }
        if (game.Player[PlayerNumber].MyTurn && Input.GetMouseButtonDown(0)) Clicked();

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
            game.Player[PlayerNumber].HandCard[n].setIsPrepare(!game.Player[PlayerNumber].HandCard[n].getIsPrepare());
        }
    }

    void CardPlace()
    {
        //カードを提出
        List<Card> p = new List<Card>();
        for (int i = 0; i < game.Player[PlayerNumber].HandCard.Count; i++)
        {
            if (game.Player[PlayerNumber].HandCard[i].getIsPrepare())
            {
                p.Add(game.Player[PlayerNumber].HandCard[i]);
            }
        }
        if (p.Count > 0)
        {
            game.CardChangeRequest(PlayerNumber, new PlaceCard(p));
        }
    }

    void CheckWin()
    {
        //勝ちかどうか
        if (game.Player[PlayerNumber] != null && game.Player[PlayerNumber].Deck.Count <= 0 && game.Player[PlayerNumber].HandCard.Count <= 0)
        {
            SceneManager.LoadScene("Win");
        }

        if (game.Player[(PlayerNumber + 1) % 2] != null && game.Player[(PlayerNumber + 1) % 2].Deck.Count <= 0 && game.Player[(PlayerNumber + 1) % 2].HandCard.Count <= 0)
        {
            SceneManager.LoadScene("Lose");
        }
    }
}
