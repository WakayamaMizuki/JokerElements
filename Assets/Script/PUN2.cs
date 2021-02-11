using UnityEngine;
//using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PUN2 : MonoBehaviourPunCallbacks
{
    public byte maxPlayers = 2;
    private bool IsLobby;
    List<RoomInfo> roomDispList = new List<RoomInfo>();
    [SerializeField] Text connectionText;
    [SerializeField] Transform[] spawnPoints;

    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        IsLobby = false;
        PhotonNetwork.ConnectUsingSettings();
    }

    #region Photon Callbacks

    void OnGUI()
    {
        //ログインの状態を画面上に出力
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }

    public override void OnConnectedToMaster()
    {
        JoinLobby();
    }

    private void JoinLobby()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        IsLobby = true;
    }

    public void JoinRoom(int roomNumber)
    {
        if (!IsLobby) return;
       
        string name = GameObject.Find("name").GetComponent<Text>().text;
        UserName.NameSet(name);

        ExitGames.Client.Photon.Hashtable customProp = new ExitGames.Client.Photon.Hashtable();
        customProp.Add("userName", name); //ユーザ名
        PhotonNetwork.SetPlayerCustomProperties(customProp);

        // "room"という名前のルームに参加する（ルームが無ければ作成してから参加する）
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
            
        PhotonNetwork.JoinOrCreateRoom("Room" + roomNumber, roomOptions, TypedLobby.Default);
    }

    //ルームに入室後に呼び出される
    public override void OnJoinedRoom()
    {
        GamePlayerNumber.SetNum(PhotonNetwork.PlayerList.Length - 1);

        PhotonNetwork.IsMessageQueueRunning = false;
        IsLobby = false;
        SceneManager.LoadScene("GamePUN");

    }

    


    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

        if (PhotonNetwork.PlayerList.Length >= 2) GameObject.Find("UIController").GetComponent<CardScript>().IsStart = true;

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


            //LoadArena();
        }
    }


    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        // ルーム一覧更新
        foreach (var info in roomList)
        {
            if (!info.RemovedFromList)
            {
                // 更新データが削除でない場合
                roomDispList.Add(info);
            }
            else
            {
                // 更新データが削除の場合
                roomDispList.Remove(info);
            }
        }
        if (!IsLobby) return;
        for (int i = 1; i <= 5; i++)
        {
            int count = 0;

            for (int j = 0; j < roomDispList.Count; j++)
            {
                if (roomDispList[j].Name.Equals("Room" + i)) count = roomDispList[j].PlayerCount;
            }
            GameObject ButtonText = GameObject.Find("Room" + i + "Text");
            if (ButtonText != null)
            {
                ButtonText.GetComponent<Text>().text = "Room" + i + "\n" + count + "/2";
            }
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        IsLobby = false;
    }


    #endregion
}
