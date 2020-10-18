using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UserPUN : MonoBehaviour
{
    //PhotonView photonView;
    GameObject GameController;
    GameObject UIObject;
    Game game;
    public int PlayerNumber;
    //private PlaceCard CardRequest;
    //private bool CardChanged = false;
    GamePlayer player;
    //Player Enemy;
    // Use this for initialization
    void Start()
    {
        //photonView = GetComponent<PhotonView>();
        if(GameObject.Find("GameController") != null){
            GameController = GameObject.Find("GameController");
        }else{
            GameController = GameObject.Find("GameController(Clone)");
        }

        game = GameController.GetComponent<Game>();

        player = game.Player[PlayerNumber];

        UIObject = (GameObject)Resources.Load("UIController");
        Instantiate(UIObject, Vector3.zero, Quaternion.identity);
        //Enemy = game.Player[1];

    }

    // Update is called once per frame
    void Update()
    {

        player = game.Player[PlayerNumber];


        while (player.IsFirst || (player.HandCard.Count < 5 && player.Deck.Count > 0))
        {
            player.DrawCard();
            player.IsFirst = false;
        }
        //if (photonView.IsMine && player.MyTurn && Input.GetMouseButtonDown(0)) Clicked();
        if (player.MyTurn && Input.GetMouseButtonDown(0)) Clicked();

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
        if (ClickCard.name.IndexOf("HandCard") >= 0)
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
        List<int> index = new List<int>();
        for (int i = 0; i < player.HandCard.Count; i++)
        {
            if (player.HandCard[i].IsPrepare)
            {
                p.Add(player.HandCard[i]);
                index.Add(i);
            }
        }
        /*
        for (int i = 0; i < index.Count; i++)
        {
            Debug.Log("PNumber: " + PlayerNumber + "CNum: " + index[i]);
            if (i != index.Count - 1)
            {
                game.GetComponent<PhotonView>().RPC("CardChangeRequest", RpcTarget.All, PlayerNumber, index[i], false);

            }
            else
            {
                game.GetComponent<PhotonView>().RPC("CardChangeRequest", RpcTarget.All, PlayerNumber, index[i], true);
            }
        }*/
        if (p.Count > 0)
        {
            game.Player[PlayerNumber].CardRequest = new PlaceCard(p);
            game.Player[PlayerNumber].CardChanged = true;
            //game.GetComponent<PhotonView>().RequestOwnership();
            //game.CardChangeRequest(PlayerNumber,new PlaceCard(p));
            //game.GetComponent<PhotonView>().RPC("CardChangeRequest", RpcTarget.All, PlayerNumber, new PlaceCard(p));
        }
    }

    /*
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //データの送信
            stream.SendNext(player);
            stream.SendNext(game.Player[1]);
        }
        else
        {
            //データの受信
            game.Player[0] = (GamePlayer)stream.ReceiveNext();
            game.Player[1] = (GamePlayer)stream.ReceiveNext();
        }
    }*/
    /*
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 自身側が生成したオブジェクトの場合は
            // 色相値と移動中フラグのデータを送信する
            //stream.SendNext(hue);
            //stream.SendNext(isMoving);
            stream.SendNext(CardChanged);
            stream.SendNext(CardRequest);
        }
        else
        {
            // 他プレイヤー側が生成したオブジェクトの場合は
            // 受信したデータから色相値と移動中フラグを更新する
            //hue = (float)stream.ReceiveNext();
            //isMoving = (bool)stream.ReceiveNext();

            //ChangeBodyColor();
        }
    }*/
}
