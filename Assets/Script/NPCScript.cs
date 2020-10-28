using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : MonoBehaviour {

    GameObject GameController;
    public int PlayerNumber;
    private int CountJoker;
    Game game;
    GamePlayer player;
    GamePlayer Enemy;
    //PlaceCard Player1.Card;
    //List<Card> player.HandCard = new List<Card>();
    
    // Use this for initialization
    void Start () {
        GameController = GameObject.Find("GameController");
        game = GameController.GetComponent<Game>();
        player = game.Player[(PlayerNumber) % 2];
        Enemy = game.Player[(PlayerNumber + 1) % 2];
        
        CountJoker = 0;
        InvokeRepeating("NPCMove",3,1);
	}
	
	// Update is called once per frame
	void Update () {
        while (player.HandCard.Count < 5 && player.Deck.Count > 0)
        {
            player.DrawCard();
        }
    }

    void NPCMove()
    {
        CountJoker = 0;
        for (int i = 0; i < player.HandCard.Count; i++)
        {
            if (player.HandCard[i].mark == 4)
            {
                CountJoker++;
            }
        }

        if (player.MyTurn){

            if (!player.Is7 && !player.Is10)
            {
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
                }else{
                    player.pass();
                }
            }
            else if (player.Is7)
            {
                WeakCardRequest(player.placeCard.count7);
            }
            else if (player.Is10)
            {
                WeakCardRequest(player.placeCard.count10);
            }
        }
    }

    List<int> NPC()
    {
        List<int> s = new List<int>();
        if (Enemy.placeCard != null){//相手がカードを出している
            if(Enemy.placeCard.cards.Count == 1){
                //単体出し
                int markRegulation = -1;
                for(int i = 0;i < player.MarkRegulation.Length; i++)
                {
                    if (player.MarkRegulation[i])
                    {
                        markRegulation = i;
                    }
                }
                for (int i = 0; i < player.HandCard.Count; i++)
                {
                    Card card = player.HandCard[i];
                    if (card.num > Enemy.placeCard.cards[0].num)
                    {
                        if((player.NumberRegulation == -1 || player.NumberRegulation + 1 == card.num) && (markRegulation == -1 || card.mark == markRegulation || card.mark == 4))
                        {
                            s.Add(i);
                            return s;
                        }
                    }
                }
            }else{
                //複数出し
                for (int i = 0; i < player.HandCard.Count; i++){
                    Card card = player.HandCard[i];
                    int MarkCount = 0;
                    int enemyCardCount = Enemy.placeCard.cards.Count;
                    for (int j = 0; j < 4; j++)
                    {
                        if (player.MarkRegulation[j]) MarkCount++;
                    }
                    if (card.num > Enemy.placeCard.cards[0].num && (player.NumberRegulation == -1 || player.NumberRegulation + 1 == card.num) && (MarkCount == 0 || card.mark == 4 || player.MarkRegulation[card.mark] || enemyCardCount - MarkCount > 0))
                    {
                        s.Add(i);
                        if (card.mark == 4) enemyCardCount--;
                        for (int j = i + 1; j < player.HandCard.Count; j++)
                        {
                            Card nextCard = player.HandCard[j];
                            if ((nextCard.num == card.num || nextCard.mark == 4) && (MarkCount == 0|| card.mark == 4 || player.MarkRegulation[card.mark] || enemyCardCount - MarkCount > 0) && s.Count < Enemy.placeCard.cards.Count)
                            {
                                s.Add(j);
                                if (nextCard.mark != 4 && !player.MarkRegulation[nextCard.mark]) enemyCardCount--;
                            }
                        }
                        if (s.Count < Enemy.placeCard.cardNum) s = new List<int>(); //枚数が足りない
                        if (s.Count >= Enemy.placeCard.cardNum)
                        {
                            return s;
                        }
                    }
                }
            }
        }else{//相手がカードを出していない
            if(player.HandCard.Count > 0){
                s.Add(0);
                for(int i = 1;i < player.HandCard.Count; i++)
                {
                    if (player.HandCard[0].num == player.HandCard[i].num) s.Add(i);
                    else break;
                }
            }else{
                s.Add(-1);
            }
            return s;
        }
        if(s.Count == 0)s.Add(-1);
        return s;
    }

    private void WeakCardRequest(int n)
    {
        //手札のうち弱いカードn枚を提出
        List<Card> p = new List<Card>();
        //nが手札の枚数より大きい時は手札を全て出す
        if (n >= player.HandCard.Count) n = player.HandCard.Count - 1;

        for (int i = 0; i < n; i++)
        {
            p.Add(player.HandCard[i]);
        }
        PlaceCard card = new PlaceCard(p);
        player.CardRequest = card;
        player.CardChanged = true;
    }
}

