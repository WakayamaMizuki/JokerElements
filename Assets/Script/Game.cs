//using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Game : MonoBehaviour
{

    public GamePlayer[] Player = new GamePlayer[2];
    //public GamePlayer Player[1] = new GamePlayer();
    public bool IsPhoton;
    //マーク縛り
    public bool[] markRegulation = new bool[4];
    //数字縛り
    public int numberRegulation;
    //革命中かどうか
    private bool IsRevolution;
    private UserCardScript userCardScript;

    // Use this for initialization
    void Start() {
        Player[0] = new GamePlayer();
        Player[1] = new GamePlayer();
        
        Player[0].MyTurn = true;
        Player[1].MyTurn = false;
        Player[0].Enemy = Player[1];
        Player[1].Enemy = Player[0];

        numberRegulation = -1;
        for(int i = 0;i < markRegulation.Length; i++)
        {
            markRegulation[i] = false;
        }
        Player[0].MarkRegulation = markRegulation;
        Player[1].MarkRegulation = markRegulation;
        if (IsPhoton) userCardScript = GameObject.Find("UserCard").GetComponent<UserCardScript>();
        IsRevolution = false;
    }


    void Update() {

        for (int i = 0; i < 2; i++)
        {
            if (Player[i].CardChanged)
            {

                Player[i].IsCardChangeRequest();

                Sort();

                markRegulation = MarkRegulation();
                Player[i].MarkRegulation = markRegulation;
                Player[(i+1)%2].MarkRegulation = markRegulation;
                numberRegulation = NumberRegulationFunction();
                Player[i].NumberRegulation = numberRegulation;
                Player[(i + 1) % 2].NumberRegulation = numberRegulation;

                //IsRevolution = (Player[0].placeCard.cards.Count > 3 || Player[1].placeCard.cards.Count > 3);

                if (IsPhoton && userCardScript != null)
                {
                    userCardScript.SetPlayer(Player);
                }
            }
        }
        if(Player[0].placeCard == null && Player[1].placeCard == null)
        {
            for (int i = 0; i < markRegulation.Length; i++)
            {
                markRegulation[i] = false;
            }
            Player[0].MarkRegulation = markRegulation;
            Player[1].MarkRegulation = markRegulation;
            numberRegulation = -1;
            Player[0].NumberRegulation = -1;
            Player[1].NumberRegulation = -1;
        }
    }

    public void ReceivePlayer(GamePlayer[] player)
    {
        Player = player;
        markRegulation = MarkRegulation();
        Player[0].MarkRegulation = markRegulation;
        Player[1].MarkRegulation = markRegulation;
        numberRegulation = NumberRegulationFunction();
        Player[0].NumberRegulation = numberRegulation;
        Player[1].NumberRegulation = numberRegulation;
    }


    public void CardChangeRequest(int PlayerNumber, PlaceCard p){
        Player[PlayerNumber].CardRequest = p;
        Player[PlayerNumber].CardChanged = true;
    }

    public void DrawCard(int PlayerNumber, int n)
    {
        for(int i = 0;i < n;i++)
        {
            Player[PlayerNumber].DrawCard();
        }
    }

    //数字縛り
    int NumberRegulationFunction()
    {
        if (Player[0].placeCard == null || Player[1].placeCard == null || Player[0].placeCard.cards.Count == 0 || Player[1].placeCard.cards.Count == 0)
        {
            return -1;
        }
        else if (Mathf.Abs(Player[0].placeCard.cards[0].getNum() - Player[1].placeCard.cards[0].getNum()) == 1)
        {
            if (Player[0].placeCard.cards[0].getNum() <= 15 && Player[1].placeCard.cards[0].getNum() <= 15) return Mathf.Max(Player[0].placeCard.cards[0].getNum(), Player[1].placeCard.cards[0].getNum());
            else return -1;
        }
        return -1;
    }

    bool[] MarkRegulation()
    {
        bool[] mark = new bool[4];
        for(int i = 0;i < 4; i++)
        {
            if(Player[0].placeCard == null || Player[1].placeCard == null)
            {
                mark[i] = false;
            }else
            {
                mark[i] = Player[0].placeCard.IsMark[i] && Player[1].placeCard.IsMark[i];
            }
        }
        return mark;
    }



    public void pass(int num){
        Player[num].pass();
        if(IsPhoton)userCardScript.SetPlayer(Player);
    }

    

    private void Sort()
    {
        if (Player[0].HandCard != null)
        {
            Player[0].HandCard = SortCard(Player[0].HandCard);
        }
        if (Player[1].HandCard != null)
        {
            Player[1].HandCard = SortCard(Player[1].HandCard);
        }
    }

    List<Card> SortCard(List<Card> hand)
    {
        List<Card> tmp = new List<Card>();
        for (int i = 0; i < hand.Count; i++){
            Card card = hand[i];
            int n = tmp.Count;
            for (int j = 0; j < tmp.Count;j++){
                if((tmp[j].getNum() > card.getNum() || (tmp[j].getNum() == card.getNum() && tmp[j].getMark() >= card.getMark() ))&& n == tmp.Count){
                    n = j;
                }
            }
            tmp.Insert(n, card);
        }
        return tmp;
    }
}





public class GamePlayer{
    public PlaceCard placeCard; //場に出ているカード
    public PlaceCard CardRequest;
    public List<Card> Deck = new List<Card>(); //山札
    public List<Card> HandCard = new List<Card>(); //手札
    public List<Card> DisCard = new List<Card>(); //捨て札
    public bool CardChanged;
    public bool CardSpriteChanged;
    public bool MyTurn;
    //public int DeckCount;
    public GamePlayer Enemy;
    public int NumberRegulation;
    public int DeckCount;
    public bool[] MarkRegulation;
    public bool Is7 = false;
    public bool Is10 = false;

    public GamePlayer(){
        Enemy = null;
        MyTurn = false;
        Deck = MakeDeck();
        CardChanged = false;
        CardSpriteChanged = true;
        NumberRegulation = -1;
    }

    public void DrawCard()
    {
        if (Deck.Count > 0)
        {
            int index = Random.Range(0, Deck.Count);
            int i;
            for (i = 0; i < HandCard.Count; i++)
            {
                if (HandCard[i].getNum() > Deck[index].getNum()) break;
            }
            HandCard.Insert(i, Deck[index]);
            Deck.RemoveAt(index);
            DeckCount = Deck.Count;
        }
    }


    private List<Card> MakeDeck()
    {
        List<Card> deck = new List<Card>();
        for (int mark = 0; mark < 4; mark++)
        {
            for (int num = 1; num < 14; num++)
            {
                deck.Add(new Card(mark, num));
            }
        }

        //ジョーカー
        deck.Add(new Card(4, 1));
        deck.Add(new Card(4, 1));
        return deck;
    }


    public void IsCardChangeRequest(){

        if (!MyTurn || CardRequest == null || CardRequest.cards == null)
        {
            CardRequest = null;
            CardChanged = false;
            return;
        }
        if (Is7)
        {
            //7渡し
            Func7();
            return;
        }
        if (Is10)
        {
            //10捨て
            Func10();
            return;
        }

        //7渡し,10捨て以外の場合
        bool check = CardPlaceCheck(CardRequest);
        if (!check)
        {
            CardRequest = null;
            CardChanged = false;
            CardSpriteChanged = false;
            for (int i = 0; i < HandCard.Count; i++)
            {
                HandCard[i].setIsPrepare(false);
            }
            return;
        }

        //チェックが通った
        if (placeCard != null)
        {
            for (int i = 0; i < placeCard.cards.Count; i++)
            {
                DisCard.Add(placeCard.cards[i]);
            }
        }
                
        placeCard = CardRequest;

        Is7 = (placeCard.count7 > 0);
        Is10 = (placeCard.count10 > 0);

        CardRequest = null;
        CardChanged = true;
        CardSpriteChanged = true;
        for (int i = placeCard.cards.Count - 1; i >= 0; i--)
        {
            HandCard.RemoveAt(HandCard.IndexOf(placeCard.cards[i]));
        }

        //スペ3返し
        if (Enemy.placeCard != null && placeCard != null && Enemy.placeCard.cardNum == 1 && Enemy.placeCard.cards != null && Enemy.placeCard.cards[0] != null && Enemy.placeCard.cards[0].getMark() == 4 && placeCard.cards != null && placeCard.cards != null && placeCard.cards[0].getMark() == 0 && placeCard.cards[0].getImageNum() == 3)
        {
            placeCard.cards[0].setNum(17);
        }

        //8切り
        Func8();

        if (!Is7 && !Is10)
        {
            //7,10が含まれるならターンを継続
            MyTurn = false;
            Enemy.MyTurn = true;
        }
                
        if (placeCard == null)
        {
            pass();
        } 
    }

    public void pass(){
        if (!MyTurn) return;
        if (!Is7 && !Is10) {
            for (int i = 0; i < HandCard.Count; i++)
            {
                HandCard[i].setIsPrepare(false);
            }
            CardSpriteChanged = true;
            Enemy.CardSpriteChanged = true;
            Is7 = false;
            Is10 = false;
            placeCard = null;
            Enemy.placeCard = null;
            NumberRegulation = -1;
            MyTurn = false;
            Enemy.MyTurn = true;
            DisCard = new List<Card>();
            Enemy.DisCard = new List<Card>();
            GameObject.Find("UIController").GetComponent<CardScript>().Flash();
        }
        else if(Is7)
        {
            for (int i = 0; i < HandCard.Count; i++)
            {
                HandCard[i].setIsPrepare(false);
            }
            Is7 = false;
            MyTurn = false;
            Enemy.MyTurn = true;
        }
        else
        {
            for (int i = 0; i < HandCard.Count; i++)
            {
                HandCard[i].setIsPrepare(false);
            }
            Is10 = false;
            MyTurn = false;
            Enemy.MyTurn = true;
        }    
    }

    bool CardPlaceCheck(PlaceCard p)//カードが受理されるか
    {
        //枚数が違う
        if (Enemy.placeCard != null && Enemy.placeCard.cards.Count != p.cards.Count) return false;

        //マーク縛り
        if (Enemy.placeCard != null)
        {
            for (int i = 0; i < p.IsMark.Length; i++)
            {
                if (MarkRegulation[i] && !p.IsMark[i]) {
                    if (p.CountJoker > 0)
                    {
                        p.CountJoker--;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
            

        if (NumberRegulation != -1 && NumberRegulation + 1 != p.cards[0].getNum() && p.cards[0].getNum() != 16 && (p.cards[0].getMark() != 0 || p.cards[0].getImageNum() != 3))
        {
            //数字縛り
            return false;
        }
        if (p.cards.Count >= 2)//複数
        {
            int n = p.cards[0].getNum();
            for (int i = 1; i < p.cards.Count; i++)
            {
                //数がそろっていない
                if (p.cards[i].getNum() != n && p.cards[i].getNum() != 16) return false;
            }
            
            //数が敵のカードよりも小さい
            if (Enemy.placeCard != null && p.cards[0].getNum() <= Enemy.placeCard.cards[0].getNum()) return false;
        }else//単体
        {
            //スペ3
            if (Enemy.placeCard != null && Enemy.placeCard.cardNum == 1 && Enemy.placeCard.cards[0].getMark() == 4 && p.cards[0].getMark() == 0 && p.cards[0].getImageNum() == 3)
            {
                return true;
            }
            //数が敵のカードよりも小さい
            if (Enemy.placeCard != null && p.cards[0].getNum() <= Enemy.placeCard.cards[0].getNum()) return false;
            //ジョーカーに対してジョーカーを出そうとする
            if (Enemy.placeCard != null && Enemy.placeCard.cards[0].getNum() == 16 && p.cards[0].getNum() == 16) return false;
        }
        return true;
    }

    


    void Func8()
    {
        //8切り
        if (placeCard == null) return;
        for (int i = 0; i < placeCard.cards.Count; i++)
        {
            if (placeCard.cards[i].getImageNum() == 8)
            {
                placeCard.cards[i].setNum(17);
            }
        }
        return;
    }

    void Func7()
    {
        //7渡し
        if (CardRequest.cards.Count <= placeCard.count7)
        {
            for (int i = 0; i < CardRequest.cards.Count; i++)
            {
                Enemy.HandCard.Add(CardRequest.cards[i]);
            }
            for (int i = CardRequest.cards.Count - 1; i >= 0; i--)
            {
                HandCard.RemoveAt(HandCard.IndexOf(CardRequest.cards[i]));
            }

            MyTurn = false;
            Enemy.MyTurn = true;
            Is7 = false;
            CardSpriteChanged = true;
        }
        else
        {
            CardRequest = null;
            CardChanged = false;
            CardSpriteChanged = false;
            for (int i = 0; i < HandCard.Count; i++)
            {
                HandCard[i].setIsPrepare(false);
            }
        }
    }

    void Func10()
    {
        //10捨て
        if (CardRequest.cards.Count <= placeCard.count10)
        {

            for (int i = CardRequest.cards.Count - 1; i >= 0; i--)
            {
                DisCard.Add(CardRequest.cards[i]);
                HandCard.RemoveAt(HandCard.IndexOf(CardRequest.cards[i]));
            }

            MyTurn = false;
            Enemy.MyTurn = true;
            Is10 = false;
            CardSpriteChanged = true;
        }
        else
        {
            CardRequest = null;
            CardChanged = false;
            CardSpriteChanged = false;
            for (int i = 0; i < HandCard.Count; i++)
            {
                HandCard[i].setIsPrepare(false);
            }
        }
    }
}