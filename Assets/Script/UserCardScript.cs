//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
//using Photon.Realtime;

public class UserCardScript : MonoBehaviourPunCallbacks
{
	public string[] Name = new string[2];
	public GamePlayer[] Players = new GamePlayer[2];
    public int PlayerNumber;
	public bool IsFirst;
	bool First1;
	public bool MyTurn;
	private Game game;

	void Awake()
	{
		GamePlayerSerializer.Register();
	}


	// Use this for initialization
	void Start () {
		game = GameObject.Find("GameController").GetComponent<Game>();
        PlayerNumber = GamePlayerNumber.num;
        if(GameObject.Find("GameController") != null)
        {
			Players = GameObject.Find("GameController").GetComponent<Game>().Player;
		}
		
		//MyTurn = player.MyTurn;
		IsFirst = true;
		First1 = true;
		InvokeRepeating("UpdateProperties", 10, 1);
	}
	
	// Update is called once per frame
	void Update () {

		if ((Players[0] != null && Players[1] != null) && (Players[0].Enemy == null || Players[1].Enemy == null))
        {
			if (Players[0] != null) Players[0].Enemy = Players[1];
			if (Players[1] != null) Players[1].Enemy = Players[0];
		}

		if(PlayerNumber == 1 && First1 && Players[0].DeckCount == 0)
        {
			First();
        }
        if (PlayerNumber == 1 && First1 && Players[0].DeckCount != 0)
        {
			First1 = false;
        }
	}

	public void UpdateProperties()
    {
		OnRoomPropertiesUpdate(PhotonNetwork.CurrentRoom.CustomProperties);

	}

	public void First()
    {
		ExitGames.Client.Photon.Hashtable hash;
		hash = PhotonNetwork.CurrentRoom.CustomProperties;
		hash["Player" + PlayerNumber] = Players[PlayerNumber];
		hash["Name" + PlayerNumber] = UserName.Name;
		PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
		IsFirst = false;
	}


	public void SetRoomPlayers(GamePlayer[] players)
	{
		ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
		for(int i = 0;i < 2; i++)
        {
			if (Name[i] != null) hash.Add("Name" + i, Name[i]);
        }
		if(players[0] != null)hash.Add("Player0", players[0]);
		if(players[1] != null && PhotonNetwork.PlayerList.Length >= 2) hash.Add("Player1", players[1]);
		PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
	}

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable i_propertiesThatChanged)
	{
		{
			object value = null;
			if (i_propertiesThatChanged.TryGetValue("Player0", out value))
			{
				Players[0] = (GamePlayer)value;
			}
		}
		{
			object value = null;
			if (i_propertiesThatChanged.TryGetValue("Player1", out value))
			{
				Players[1] = (GamePlayer)value;
			}
		}
		{
			object value = null;
			if (i_propertiesThatChanged.TryGetValue("Name0", out value))
			{
				Name[0] = (string)value;
			}
		}
		{
			object value = null;
			if (i_propertiesThatChanged.TryGetValue("Name1", out value))
			{
				Name[1] = (string)value;
			}
		}

		if (Players[0] != null)Players[0].Enemy = Players[1];
		if(Players[1] != null)Players[1].Enemy = Players[0];
		game.ReceivePlayer(Players);
	}


	public GamePlayer[] NowPlayer()
    {
		//IsChanged = false;
		return Players;
    }


	public GamePlayer FirstPlayer()
    {
		return Players[PlayerNumber];
    }
	

	public void SetPlayer(GamePlayer[] players)
    {
		if (IsFirst && Players[PlayerNumber] != null && Players[PlayerNumber].HandCard.Count >= 5 && PhotonNetwork.CurrentRoom != null && PhotonNetwork.PlayerList.Length >= 2)
		{
			ExitGames.Client.Photon.Hashtable hash;
			hash = PhotonNetwork.CurrentRoom.CustomProperties;
			hash["Player" + PlayerNumber] = Players[PlayerNumber];
			hash["Name"+PlayerNumber] = UserName.Name;
			PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
			//Players[(PlayerNumber + 1)%2] = enemyCardScript.FirstPlayer();
			OnRoomPropertiesUpdate(PhotonNetwork.CurrentRoom.CustomProperties);
			IsFirst = false;
        }
        else if(Players[PlayerNumber] != null && PhotonNetwork.CurrentRoom != null && PhotonNetwork.PlayerList.Length >= 2)
		{
			Players = players;
			SetRoomPlayers(Players);
		}
    }
}
