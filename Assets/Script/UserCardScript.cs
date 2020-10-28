using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

using Photon.Pun;
//using Photon.Realtime;

public class UserCardScript : MonoBehaviourPun, IPunObservable/*Callbacks*/
{

	public GamePlayer[] Players = new GamePlayer[2];
	private GameObject enemyCard;
    private GamePlayer player;
	private GamePlayer enemy;
    public int PlayerNumber;
	public int rnd;
	public bool IsChanged;
	public bool IsFirst = true;
	public bool MyTurn;
	private UserCardScript enemyCardScript;

	void Awake()
	{

		GamePlayerSerializer.Register();
	}


	// Use this for initialization
	void Start () {
		//var customProperties = photonView.Owner.CustomProperties;
		Players = GameObject.Find("GameController").GetComponent<Game>().Player;
		player = Players[PlayerNumber];
		enemy = Players[(PlayerNumber + 1) % 2];
		MyTurn = player.MyTurn;
		rnd = Random.Range(0, 100);
	}
	
	// Update is called once per frame
	void Update () {

		/*
		if (enemyCardScript == null)
		{
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			for(int i = 0;i < players.Length; i++)
            {
				if (!players[i].GetComponent<PhotonView>().IsMine)
				{
					enemyCardScript = players[i].GetComponent<UserCardScript>();
					enemyCard = players[i];
				}

			}	
		}
		if (enemyCardScript != null && !IsFirst && enemyCardScript.IsChanged)
		{
			Players = NowPlayer();
			enemyCard.GetComponent<PhotonView>().RPC("SetFalse", RpcTarget.All);
		}*/
		if (IsFirst && Players[PlayerNumber].HandCard.Count >= 5)
        {
			ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;
			hash.Add("Player" + PlayerNumber, Players[PlayerNumber]);
			Debug.Log("HERE");
			PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
			Debug.Log("OWATA");
			//Players[(PlayerNumber + 1)%2] = enemyCardScript.FirstPlayer();
			IsFirst = false;
        }
        /*
		if(enemy.HandCard != null)
		{
			for(int i = 0;i < enemy.HandCard.Count; i++)
            {
				Debug.Log(enemy.HandCard[i].MarkString + " " + enemy.HandCard[i].num);
            }
		}*/
        if (IsChanged)
        {
			SetRoomPlayers(Players);
			IsChanged = false;
        }
		MyTurn = player.MyTurn;
	}

	/*
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			//データの送信
			//stream.SendNext(player);
			//stream.SendNext(enemy);
			stream.SendNext(rnd);
			//stream.SendNext(Players);
			stream.SendNext(IsChanged);
			
		}
		else
		{
			//データの受信
			//this.player = (GamePlayer)stream.ReceiveNext();
			//this.enemy = (GamePlayer)stream.ReceiveNext();
			this.rnd = (int)stream.ReceiveNext();
			//this.Players = (GamePlayer[])stream.ReceiveNext();
			this.IsChanged = (bool)stream.ReceiveNext();
			
		}
	}*/

	public void SetRoomPlayers(GamePlayer[] players)
	{
		var hash = new ExitGames.Client.Photon.Hashtable();
		hash.Add("rnd", rnd);
		hash.Add("Player0", players[0]);
		hash.Add("Player1", players[1]);
		//hash.Add("IsChanged", IsChanged);
		//properties.Add("Sender", PhotonNetwork.player.ID);
		PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
	}

	public void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable i_propertiesThatChanged)
	{
		{
			object value = null;
			if(i_propertiesThatChanged.TryGetValue("rnd", out value))
			{
				rnd = (int)value;
			}
			value = null;
			if (i_propertiesThatChanged.TryGetValue("Player0", out value))
			{
				Players[0] = (GamePlayer)value;
			}
			value = null;
			if (i_propertiesThatChanged.TryGetValue("Player1", out value))
			{
				Players[1] = (GamePlayer)value;
			}
		}
	}


	public GamePlayer[] NowPlayer()
    {
		//IsChanged = false;
		return Players;
    }

	[PunRPC]
	public void SetFalse()
    {
		IsChanged = false;
    }

	public GamePlayer FirstPlayer()
    {
		return Players[PlayerNumber];
    }
	

	public void SetPlayer(GamePlayer[] players)
    {
		Players = players;
		player = Players[PlayerNumber];
		enemy = Players[(PlayerNumber + 1) % 2];
		IsChanged = true;
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
		//throw new System.NotImplementedException();
		if (stream.IsWriting)
		{
			//データの送信
			//stream.SendNext(player);
			//stream.SendNext(enemy);
			stream.SendNext(Players);
			stream.SendNext(rnd);
		}
		else
		{
			//データの受信
			//this.player = (GamePlayer)stream.ReceiveNext();
			//this.enemy = (GamePlayer)stream.ReceiveNext();
			this.Players = (GamePlayer[])stream.ReceiveNext();
			this.rnd = (int)stream.ReceiveNext();
		}
	}
}
