using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour {

    GameObject GameController;
    Game game;
    public int PlayerNumber;
    GamePlayer player;
    //Player Enemy;
	// Use this for initialization
	void Start () {
        PlayerNumber = Random.Range(0, 2);
        GameController = GameObject.Find("GameController");
        game = GameController.GetComponent<Game>();
        player = game.Player[PlayerNumber];
    }
	
	// Update is called once per frame
	void Update () {
        player = game.Player[PlayerNumber];
        while (player != null && player.HandCard.Count < 5 && player.Deck.Count > 0)
        {
            player.DrawCard();
        }
        if (player != null && player.MyTurn && Input.GetMouseButtonDown(0)) Clicked();
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
            {//カードをクリック
                CardChoice(hit.collider.gameObject);
            }
        }
        else
        {
            CardPlace();
        }
    }

    void CardChoice(GameObject ClickCard)
    {

        Vector3 pos = ClickCard.GetComponent<Transform>().position;
        pos = pos + new Vector3(0, 0, 0);
        //ClickCard.GetComponent<Transform>().position = pos;
        //Debug.Log(ClickCard.name.Remove(ClickCard.name.IndexOf("HandCard"),8));
        if(ClickCard.name.IndexOf("HandCard") >= 0)
        {
            int n = int.Parse(ClickCard.name.Remove(ClickCard.name.IndexOf("HandCard"), 8));
            player.HandCard[n].IsPrepare = !player.HandCard[n].IsPrepare;

            for (int i = 0; i < player.HandCard.Count; i++)
            {
                GameObject card = GameObject.Find("HandCard" + i);
                Transform cardTransform = card.GetComponent<Transform>();
                if (player.HandCard[i].IsPrepare)
                {
                    cardTransform.position = new Vector3(i - 3, -1.5f, 0);
                }
                else
                {
                    cardTransform.position = new Vector3(i - 3, -2f, 0);
                }
            }
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
}
