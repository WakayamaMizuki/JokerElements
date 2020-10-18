using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : MonoBehaviour {

    GameObject GameController;
    public static int PlayerNumber;
    Game game;
    GamePlayer player;
    GamePlayer Enemy;
    //PlaceCard Player1.Card;
    //List<Card> player.HandCard = new List<Card>();
    
    // Use this for initialization
    void Start () {
        GameController = GameObject.Find("GameController");
        game = GameController.GetComponent<Game>();
        PlayerNumber = (GameObject.Find("UserController").GetComponent<User>().PlayerNumber + 1) % 2;
        player = game.Player[PlayerNumber];
        Enemy = game.Player[(PlayerNumber) % 2];
        InvokeRepeating("NPCMove",3,1);
	}
	
	// Update is called once per frame
	void Update () {
        while (player.HandCard.Count < 5 && player.Deck.Count > 0)
        {
            player.DrawCard();
        }
        //Enemy.placeCard = Enemy.placeCard;
        //player.HandCard = game.player.HandCard;
    }

    void NPCMove()
    {
        if(player.MyTurn && !player.Is7){
            List<int> n = NPC();
            List<Card> p = new List<Card>();
            if (n[0] != -1)
            {

                for (int i = 0; i < n.Count; i++)
                {
                    p.Add(player.HandCard[n[i]]);
                }
                PlaceCard card = new PlaceCard(p);
                player.CardRequest = card;
                player.CardChanged = true;
            }
            else
            {
                player.pass();
            }
        }
        else if(player.MyTurn && player.Is7)
        {
            int Count7 = 0;
            for(int i = 0;i < player.HandCard.Count; i++)
            {
                if (player.HandCard[i].num == 7) Count7++;
            }
            List<Card> p = new List<Card>();
            for (int i = 0; i < Count7; i++)
            {
                p.Add(player.HandCard[i]);
            }
            PlaceCard card = new PlaceCard(p);
            player.CardRequest = card;
            player.CardChanged = true;
        }
    }

    List<int> NPC()
    {
        List<int> s = new List<int>();
        if (Enemy.placeCard != null){
            if(Enemy.placeCard.cardNum == 1){
                for (int i = 0; i < player.HandCard.Count; i++)
                {
                    Card card = player.HandCard[i];
                    if (card.num > Enemy.placeCard.cards[0].num)
                    {
                        if(player.NumberRegulation == 0 || player.NumberRegulation == -1){
                            s.Add(i);
                            return s;
                        }else if(player.NumberRegulation + 1 == card.num){
                            s.Add(i);
                            return s;
                        }
                    }
                }
            }else{
                //2まい出し
                for (int i = 0; i < player.HandCard.Count; i++){
                    Card card = player.HandCard[i];
                    if (card.num > Enemy.placeCard.cards[0].num){

                        if (player.NumberRegulation == -1 || player.NumberRegulation + 1 == card.num)
                        {
                            s.Add(i);
                            for (int j = i + 1; j < player.HandCard.Count; j++)
                            {
                                Card nextCard = player.HandCard[j];
                                if (nextCard != null && nextCard.num == card.num)
                                {
                                    s.Add(j);
                                }
                            }
                            if (s.Count < Enemy.placeCard.cardNum) s = new List<int>();
                            if (s.Count >= Enemy.placeCard.cardNum)
                            {
                                return s;
                            }
                        }
                    }
                }
            }
        }else{
            if(player.HandCard.Count > 0){
                s.Add(0);
            }else{
                s.Add(-1);
            }
            return s;
        }
        if(s.Count == 0)s.Add(-1);
        return s;
    }
}
