//using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : MonoBehaviour {

    GameObject GameController;
    public int PlayerNumber;
    private int CountJoker;
    Game game;
    GamePlayer player;
    GamePlayer Enemy;
    
    // Use this for initialization
    void Start () {
        PlayerNumber = (GamePlayerNumber.num + 1)%2;
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
        if (!player.MyTurn) return;
        CountJoker = 0;
        for (int i = 0; i < player.HandCard.Count; i++)
        {
            if (player.HandCard[i].getMark() == 4)
            {
                CountJoker++;
            }
        }

        if (player.Is7)
        {
            WeakCardRequest(player.placeCard.count7);
            return;
        }
        if (player.Is10)
        {
            WeakCardRequest(player.placeCard.count10);
            return;
        }

        List<int> n = NPC();
        List<Card> p = new List<Card>();
        if (n[0] != -1)
        {
            for (int i = 0; i < n.Count; i++)
            {
                p.Add(player.HandCard[n[i]]);
            }
            //カードを出す
            game.CardChangeRequest(PlayerNumber, new PlaceCard(p));
        }
        else{
            player.pass();
        }
    }

    List<int> NPC()
    {
        //出すカードを選ぶ
        List<int> s = new List<int>();

        if(Enemy.placeCard == null)
        {
            //相手がカードを出していない
            if (player.HandCard.Count > 0)
            {
                s.Add(0);
                for (int i = 1; i < player.HandCard.Count; i++)
                {
                    if (player.HandCard[0].getNum() == player.HandCard[i].getNum()) s.Add(i);
                    else break;
                }
            }
            else
            {
                s.Add(-1);
            }
            return s;
        }

        //相手がカードを出している
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
                if (card.getNum() > Enemy.placeCard.cards[0].getNum())
                {
                    if((player.NumberRegulation == -1 || player.NumberRegulation + 1 == card.getNum()) && (markRegulation == -1 || card.getMark() == markRegulation || card.getMark() == 4))
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
                if (card.getNum() > Enemy.placeCard.cards[0].getNum() && (player.NumberRegulation == -1 || player.NumberRegulation + 1 == card.getNum()) && (MarkCount == 0 || card.getMark() == 4 || player.MarkRegulation[card.getMark()] || enemyCardCount - MarkCount > 0))
                {
                    s.Add(i);
                    if (card.getMark() == 4) enemyCardCount--;
                    for (int j = i + 1; j < player.HandCard.Count; j++)
                    {
                        Card nextCard = player.HandCard[j];
                        if ((nextCard.getNum() == card.getNum() || nextCard.getMark() == 4) && (MarkCount == 0|| card.getMark() == 4 || player.MarkRegulation[card.getMark()] || enemyCardCount - MarkCount > 0) && s.Count < Enemy.placeCard.cards.Count)
                        {
                            s.Add(j);
                            if (nextCard.getMark() != 4 && !player.MarkRegulation[nextCard.getMark()]) enemyCardCount--;
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
        game.CardChangeRequest(PlayerNumber, new PlaceCard(p));
    }
}

