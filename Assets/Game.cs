using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;


public class Game : MonoBehaviourPunCallbacks
{

    public GamePlayer[] Player = new GamePlayer[2];
    //public GamePlayer Player[1] = new GamePlayer();
    public bool IsPhoton;
    new PhotonView photonView;
    //マーク縛り
    public int markRegulation;
    //数字縛り
    public int numberRegulation;
    List<Card> p = new List<Card>();


    // Use this for initialization
    void Start() {
        photonView = GetComponent<PhotonView>();
        /*
        while (Player[0].HandCard.Count < 5){
            Player[0].DrawCard();
        }

        while (Player[1].HandCard.Count < 5)
        {
            Player[1].DrawCard();
        }*/

        //Player[0].HandCard = SortCard(Player[0].HandCard);
        //Player[1].HandCard = SortCard(Player[1].HandCard);
        Player[0] = new GamePlayer();
        Player[1] = new GamePlayer();
        Player[0].MyTurn = true;
        Player[1].MyTurn = false;
        Player[0].Enemy = Player[1];
        Player[1].Enemy = Player[0];
        numberRegulation = -1;
        Player[0].NumberRegulation = numberRegulation;
        Player[1].NumberRegulation = numberRegulation;
        Player[0].HandCardName = "HandCard";
        Player[1].HandCardName = "EnemyHandCard";
        /*
        Player[0].HandCardPos = new Vector3(-3, -2, 0);
        Player[1].HandCardPos = new Vector3(0, 2.5f, 0);*/
    }

    // Update is called once per frame
    void Update() {
        //Player[0].DeckCount = Player[0].Deck.Count;
        //Player[1].DeckCount = Player[1].Deck.Count;

        /*
        while (Player[0].HandCard.Count < 5 && Player[0].Deck.Count > 0)
        {
                Player[0].DrawCard();
        }
        while (Player[1].HandCard.Count < 5 && Player[1].Deck.Count > 0)
        {
            
                Player[1].DrawCard();
        }*/

        if(Player[0].HandCard != null)
        {
            Player[0].HandCard = SortCard(Player[0].HandCard);
        }
        if (Player[1].HandCard != null)
        {
            Player[1].HandCard = SortCard(Player[1].HandCard);
        }



        if (!IsPhoton/*|| (photonView != null && photonView.IsMine)*/) {
            if (Player[0].CardChanged)
            {
                //if (IsPhoton)photonView.RequestOwnership();
                //numberRegulation = NumberRegulationFunction(Player[1], Player[0]);
                Player[0].IsCardChangeRequest();
                numberRegulation = Player[0].NumberRegulationFunction();
                Player[0].NumberRegulation = numberRegulation;
                Player[1].NumberRegulation = numberRegulation;

            }
            if (Player[1].CardChanged)
            {
                //if (IsPhoton) photonView.RequestOwnership();
                //numberRegulation = NumberRegulationFunction(Player[0], Player[1]);
                Player[1].IsCardChangeRequest();
                numberRegulation = Player[1].NumberRegulationFunction();
                Player[0].NumberRegulation = numberRegulation;
                Player[1].NumberRegulation = numberRegulation;
            }
        }


        //Player[0].DeckCount = Player[0].Deck.Count;
        //Player[1].DeckCount = Player[1].Deck.Count;
        CheckWin();
    }

    [PunRPC]
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

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //データの送信
            stream.SendNext(Player[0]);
            stream.SendNext(Player[1]);
        }
        else
        {
            //データの受信
            this.Player[0] = (GamePlayer)stream.ReceiveNext();
            this.Player[1] = (GamePlayer)stream.ReceiveNext();
        }
        Debug.Log("here");
    }

    public void OnOwnershipRequest(object[] viewAndPlayer)
    {
        Debug.Log("ちょーだい");
        PhotonView view = viewAndPlayer[0] as PhotonView;
        Player requestingPlayer = viewAndPlayer[1] as Player;

        Debug.Log("OnOwnershipRequest(): Player " + requestingPlayer + " requests ownership of: " + view + ".");

        view.TransferOwnership(requestingPlayer);
    }



    public void pass(int num){
        Player[num].pass();
    }

    void CheckWin(){
        //勝ちかどうか
        if(Player[0].Deck.Count <= 0 && Player[0].HandCard.Count <= 0){
            SceneManager.LoadScene("Win");
        }

        if(Player[1].Deck.Count <= 0 && Player[1].HandCard.Count <= 0){
            SceneManager.LoadScene("Lose");
        }
    }

    void func7(Card card,List<Card> receiveHand, List<Card> passHand)
    {
        //7渡し
        passHand.Remove(card);
        receiveHand.Add(card);

    }

    List<Card> SortCard(List<Card> hand)
    {
        List<Card> tmp = new List<Card>();
        for (int i = 0; i < hand.Count; i++){
            Card card = hand[i];
            int n = tmp.Count;
            for (int j = 0; j < tmp.Count;j++){
                if(tmp[j].num > card.num && n == tmp.Count){
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
        imageIndex = mark * 13 + imageNum - 1;
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
                this.MarkString = "ハート";
                break;
            case 1:
                this.MarkString = "ダイヤ";
                break;
            case 2:
                this.MarkString = "クラブ";
                break;
            case 3:
                this.MarkString = "スペード";
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
    public int count7;

    public PlaceCard(List<Card> cards)
    {
        this.cards = cards;
        cardNum = cards.Count;
        count7 = FuncCount7();
    }

    private int FuncCount7(){
        int count = 0;
        for (int i = 0; i < this.cards.Count;i++){
            if(this.cards[i].num == 7){
                count++;
            }
        }
        return count;
    }
}

public class GamePlayer{
    public PlaceCard placeCard; //場に出ているカード
    public PlaceCard CardRequest;
    public List<Card> Deck = new List<Card>(); //山札
    public List<Card> HandCard = new List<Card>(); //手札
    public bool CardChanged;
    public bool CardSpriteChanged;
    public bool MyTurn;
    //public int DeckCount;
    public GamePlayer Enemy;
    public int NumberRegulation;
    public string HandCardName;
    public bool IsFirst;
    public bool Is7;
    //public Vector3 HandCardPos;

    public GamePlayer(){
        Deck = MakeDeck();
        CardChanged = false;
        CardSpriteChanged = false;
        NumberRegulation = -1;
        MyTurn = false;
        IsFirst = true;
    }

    [PunRPC]
    public void DrawCard()
    {
        if (Deck.Count > 0)
        {
            int index = Random.Range(0, Deck.Count);
            HandCard.Add(Deck[index]);
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

    [PunRPC]
    public void IsCardChangeRequest(){
        Debug.Log("CHangeRequest");
        
        if (MyTurn && !Is7)
        {

            bool check;
            if(CardRequest != null && CardRequest.cards != null){
                check = CardPlaceCheck(CardRequest.cards);
            }else{
                check = false;
            }

            if (check)
            {
                Debug.Log("HEKKIO");
                placeCard = CardRequest;
                int count7 = 0;
                for(int i = 0;i < placeCard.cards.Count;i++)
                {
                    if (placeCard.cards[i].num == 7) count7++;
                }
                placeCard.count7 = count7;
                if (count7 > 0) Is7 = true;
                CardRequest = null;
                CardChanged = true;
                CardSpriteChanged = true;
                for (int i = placeCard.cards.Count - 1; i >= 0; i--)
                {
                    HandCard.RemoveAt(HandCard.IndexOf(placeCard.cards[i]));
                }
                /*
                for (int i = 0; i < HandCard.Count; i++)
                {
                    GameObject card = GameObject.Find(HandCardName + i);
                    Transform cardTransform = card.GetComponent<Transform>();
                    cardTransform.position = new Vector3(i, 0, 0) + HandCardPos;
                }*/
                //スペ3返し
                if (Enemy.placeCard != null && placeCard != null && Enemy.placeCard.cardNum == 1 && Enemy.placeCard.cards != null && Enemy.placeCard.cards[0] != null && Enemy.placeCard.cards[0].mark == 4 && placeCard.cards != null && placeCard.cards != null && placeCard.cards[0].mark == 3 && placeCard.cards[0].num == 3)
                {
                    placeCard.cards[0].num = 17;
                }

                //8切り
                func8();

                if (!Is7)
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
        else if(Is7 && CardRequest != null && CardRequest.cards != null)
        {
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
    }

    [PunRPC]
    public void pass(){
        if (MyTurn)
        {
            for(int i = 0;i < HandCard.Count; i++)
            {
                HandCard[i].IsPrepare = false;
            }
            CardSpriteChanged = true;
            Enemy.CardSpriteChanged = true;
            MyTurn = false;
            Enemy.MyTurn = true;
            Is7 = false;
            placeCard = null;
            Enemy.placeCard = null;
            NumberRegulation = -1;
        }
    }

    bool CardPlaceCheck(List<Card> p)//カードが受理されるか
    {
        //枚数が違う
        if (Enemy.placeCard != null && Enemy.placeCard.cards.Count != p.Count) return false;

        if (NumberRegulation != -1 && NumberRegulation + 1 != p[0].num && p[0].num != 16 && (p[0].mark != 3 || p[0].num != 3))
        {
            //数字縛り
            return false;
        }
        if (p.Count >= 2)//複数
        {
            int n = p[0].num;
            for (int i = 1; i < p.Count; i++)
            {
                //数がそろっていない
                if (p[i].num != n && p[i].num != 16) return false;
            }
            
            //数が敵のカードよりも小さい
            if (Enemy.placeCard != null && p[0].num <= Enemy.placeCard.cards[0].num) return false;
        }else//単体
        {
            //スペ3
            if (Enemy.placeCard != null && Enemy.placeCard.cardNum == 1 && Enemy.placeCard.cards[0].mark == 4 && p[0].mark == 3 && p[0].num == 3)
            {
                return true;
            }
            //数が敵のカードよりも小さい
            if (Enemy.placeCard != null && p[0].num <= Enemy.placeCard.cards[0].num && p[0].num != 16) return false;
            //ジョーカーに対してジョーカーを出そうとする
            if (Enemy.placeCard != null && Enemy.placeCard.cards[0].num  == 16 && p[0].num == 16) return false;
        }
        return true;
    }

    public int NumberRegulationFunction()
    {
        if (Enemy.placeCard == null || Enemy.placeCard.cards == null)
        {
            return -1;
        }
        else if (Enemy.placeCard != null && Enemy.placeCard.cards != null && Enemy.placeCard.cards[0] != null && placeCard != null && placeCard.cards != null && placeCard.cards[0] != null && Enemy.placeCard.cards[0].num + 1 == placeCard.cards[0].num)
        {
            if (placeCard.cards[0].num <= 15) return placeCard.cards[0].num;
            else return -1;
        }
        return -1;
    }

    void func8()
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
}