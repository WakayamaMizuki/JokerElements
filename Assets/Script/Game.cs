using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;



public class Game : MonoBehaviour
{

    public GamePlayer[] Player = new GamePlayer[2];
    //public GamePlayer Player[1] = new GamePlayer();
    public bool IsPhoton;
    //マーク縛り
    public bool[] markRegulation = new bool[4];
    //数字縛り
    public int numberRegulation;
    private UserCardScript userCardScript;
    List<Card> p = new List<Card>();


    // Use this for initialization
    void Start() {

        Player[0] = new GamePlayer();
        Player[1] = new GamePlayer();
        
        Player[0].MyTurn = true;
        Player[1].MyTurn = false;
        Player[0].Enemy = Player[1];
        Player[1].Enemy = Player[0];
        /*
        Player[0].HandCard.Add(new Card(4, 0));
        Player[0].HandCard.Add(new Card(4, 0));
        Player[1].HandCard.Add(new Card(4, 0));
        Player[1].HandCard.Add(new Card(4, 0));*/
        numberRegulation = -1;
        for(int i = 0;i < markRegulation.Length; i++)
        {
            markRegulation[i] = false;
        }
        Player[0].MarkRegulation = markRegulation;
        Player[1].MarkRegulation = markRegulation;
    }

    // Update is called once per frame
    void Update() {
        if (userCardScript == null)
        {
            
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetComponent<PhotonView>().IsMine) userCardScript = players[i].GetComponent<UserCardScript>();
            }
        }
        if (IsPhoton && userCardScript != null)
        {
            Player = userCardScript.Players;
        }
         //if (!IsPhoton) {
        if (true){
            if (Player[0].CardChanged)
            {

                //if (IsPhoton)photonView.RequestOwnership();
                Player[0].IsCardChangeRequest();

                Sort();

                markRegulation = MarkRegulation();
                Player[0].MarkRegulation = markRegulation;
                Player[1].MarkRegulation = markRegulation;
                numberRegulation = NumberRegulationFunction();
                Player[0].NumberRegulation = numberRegulation;
                Player[1].NumberRegulation = numberRegulation;
                if (IsPhoton && userCardScript != null)
                {
                    userCardScript.SetPlayer(Player);
                }
            }
            if (Player[1].CardChanged)
            {
                //if (IsPhoton) photonView.RequestOwnership();
                Player[1].IsCardChangeRequest();

                Sort();

                markRegulation = MarkRegulation();
                Player[0].MarkRegulation = markRegulation;
                Player[1].MarkRegulation = markRegulation;
                numberRegulation = NumberRegulationFunction();
                Player[0].NumberRegulation = numberRegulation;
                Player[1].NumberRegulation = numberRegulation;
                if (IsPhoton && userCardScript != null)
                {
                    userCardScript.SetPlayer(Player);
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

    }


    public void CardChangeRequest(int PlayerNumber, int CardIndex, bool last){
        Debug.Log("here");
        p.Add(Player[PlayerNumber].HandCard[CardIndex]);
        if (last)
        {
            Debug.Log("");
            Player[PlayerNumber].CardRequest = new PlaceCard(p);
            Player[PlayerNumber].CardChanged = true;
            p = new List<Card>();
        }
    }

    //数字縛り
    int NumberRegulationFunction()
    {
        if (Player[0].placeCard == null || Player[1].placeCard == null)
        {
            return -1;
        }
        else if (Mathf.Abs(Player[0].placeCard.cards[0].num - Player[1].placeCard.cards[0].num) == 1)
        {
            if (Player[0].placeCard.cards[0].num <= 15 && Player[1].placeCard.cards[0].num <= 15) return Mathf.Max(Player[0].placeCard.cards[0].num, Player[1].placeCard.cards[0].num);
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
                if((tmp[j].num > card.num  || (tmp[j].num == card.num && tmp[j].mark >= card.mark ))&& n == tmp.Count){
                    n = j;
                }
            }
            tmp.Insert(n, card);
        }
        return tmp;
    }
}


public class Card
{
    public int mark;
    public string MarkString;//デバッグ用
    public int num;  //強さ
    public int imageNum; //カードの数字

    public int imageIndex;

    public bool IsPrepare;

    public Card(int mark, int num){
        this.mark = mark;
        this.imageNum = num;
        IsPrepare = false;
        if (mark != 4) imageIndex = mark * 15 + imageNum - 1;
        else imageIndex = 13;
        if (num >=3){
            this.num = num;
        }else{
            if(mark != 4){
                this.num = num + 13;
            }else{
                this.num = 16;
            }
        }
        ChangeMarkString();
    }

    private void ChangeMarkString(){
        switch(this.mark){
            case 0:
                this.MarkString = "スペード";
                break;
            case 1:
                this.MarkString = "クラブ";
                break;
            case 2:
                this.MarkString = "ハート";
                break;
            case 3:
                this.MarkString = "ダイヤ";
                break;
            case 4:
                this.MarkString = "ジョーカー";
                break;
        }
    }
}

public class PlaceCard
{
    public int cardNum;
    public List<Card> cards;
    public bool stair; //階段か
    public bool multi; //数字が同じカードが複数か
    public int CountJoker;
    public int count7;
    public int count8;
    public int count10;
    public bool[] IsMark = new bool[4];

    

    public PlaceCard(List<Card> cards)
    {
        this.cards = cards;
        cardNum = cards.Count;
        count7 = FuncCountImageNum(7);
        count8 = FuncCountImageNum(8);
        count10 = FuncCountImageNum(10);
        CountJoker = FuncCountJoker();
        multi = CheckMulti();
        stair = CheckStairs();
        CheckMark();
    }

    private int FuncCountImageNum(int imageNum)
    {
        int count = 0;
        for (int i = 0; i < this.cards.Count; i++)
        {
            if (this.cards[i].imageNum == imageNum)
            {
                count++;
            }
        }
        return count;
    }

    private bool CheckMulti()
    {
        if (cards.Count <= 1) return false;
        int num = cards[0].num;
        for(int i = 1;i < cards.Count; i++)
        {
            if (cards[i].num != num && cards[i].mark != 4) return false;
        }
        return true;
    }

    private int FuncCountJoker()
    {
        int sum = 0;
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].mark == 4) sum++;
        }
        return sum;
    }

    private bool CheckStairs()
    {
        for (int i = 0; i < cards.Count - 1; i++)
        {
            if (cards[i].num == cards[i+1].num && cards[i].mark == cards[i].mark)
            {

            }
            else
            {

            }
        }
        return false;
    }

    private void CheckMark()
    {
        for (int i = 0; i < IsMark.Length; i++)
        {
            IsMark[i] = false;
        }
        for (int i = 0; i < cards.Count; i++)
        {
            if(cards[i].mark != 4)IsMark[cards[i].mark] = true;
        }
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
    public string HandCardName;
    public bool Is7 = false;
    public bool Is10 = false;
    //public Vector3 HandCardPos;

    public GamePlayer(){
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
                if (HandCard[i].num > Deck[index].num) break;
            }
            HandCard.Insert(i, Deck[index]);
            Deck.RemoveAt(index);
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
        
        if (MyTurn && !Is7 && !Is10)
        {
            bool check;
            if(CardRequest != null && CardRequest.cards != null){
                check = CardPlaceCheck(CardRequest);
            }else{
                check = false;
            }

            if (check)
            {
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
                if (Enemy.placeCard != null && placeCard != null && Enemy.placeCard.cardNum == 1 && Enemy.placeCard.cards != null && Enemy.placeCard.cards[0] != null && Enemy.placeCard.cards[0].mark == 4 && placeCard.cards != null && placeCard.cards != null && placeCard.cards[0].mark == 0 && placeCard.cards[0].imageNum == 3)
                {
                    placeCard.cards[0].num = 17;
                }

                //8切り
                Func8();

                if (!Is7 && !Is10)
                {
                    MyTurn = false;
                    Enemy.MyTurn = true;
                }
                
                if (placeCard == null)
                {
                    pass();
                }
            }
            else
            {
                CardRequest = null;
                CardChanged = false;
                CardSpriteChanged = false;
                for (int i = 0; i < HandCard.Count; i++)
                {
                    HandCard[i].IsPrepare = false;
                }
            }
        }
        else if(MyTurn && Is7 && CardRequest != null && CardRequest.cards != null)
        {
            //7渡し
            Func7();
        }
        else if (MyTurn && Is10 && CardRequest != null && CardRequest.cards != null)
        {
            //10捨て
            Func10();
        }
        CardRequest = null;
        CardChanged = false;
    }

    public void pass(){
        if (MyTurn)
        {
            if (!Is7 && !Is10) {
                for (int i = 0; i < HandCard.Count; i++)
                {
                    HandCard[i].IsPrepare = false;
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
                    HandCard[i].IsPrepare = false;
                }
                Is7 = false;
                MyTurn = false;
                Enemy.MyTurn = true;
            }
            else
            {
                for (int i = 0; i < HandCard.Count; i++)
                {
                    HandCard[i].IsPrepare = false;
                }
                Is10 = false;
                MyTurn = false;
                Enemy.MyTurn = true;
            }
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
            

        if (NumberRegulation != -1 && NumberRegulation + 1 != p.cards[0].num && p.cards[0].num != 16 && (p.cards[0].mark != 3 || p.cards[0].num != 3))
        {
            //数字縛り
            return false;
        }
        if (p.cards.Count >= 2)//複数
        {
            int n = p.cards[0].num;
            for (int i = 1; i < p.cards.Count; i++)
            {
                //数がそろっていない
                if (p.cards[i].num != n && p.cards[i].num != 16) return false;
            }
            
            //数が敵のカードよりも小さい
            if (Enemy.placeCard != null && p.cards[0].num <= Enemy.placeCard.cards[0].num) return false;
        }else//単体
        {
            //スペ3
            if (Enemy.placeCard != null && Enemy.placeCard.cardNum == 1 && Enemy.placeCard.cards[0].mark == 4 && p.cards[0].mark == 0 && p.cards[0].imageNum == 3)
            {
                return true;
            }
            //数が敵のカードよりも小さい
            if (Enemy.placeCard != null && p.cards[0].num <= Enemy.placeCard.cards[0].num ) return false;
            //ジョーカーに対してジョーカーを出そうとする
            if (Enemy.placeCard != null && Enemy.placeCard.cards[0].num  == 16 && p.cards[0].num == 16) return false;
        }
        return true;
    }

    


    void Func8()
    {
        //8切り
        if (placeCard == null) return;
        for (int i = 0; i < placeCard.cards.Count; i++)
        {
            if (placeCard.cards[i].imageNum == 8)
            {
                placeCard.cards[i].num = 17;
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
                HandCard[i].IsPrepare = false;
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
                HandCard[i].IsPrepare = false;
            }
        }
    }
}