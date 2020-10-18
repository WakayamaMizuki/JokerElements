using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PUN2 : MonoBehaviourPunCallbacks
{
    public byte maxPlayers = 2;
    //[SerializeField] Text connectionText;
    // Use this for initialization
    void Start()
    {
        //PhotonNetwork.logLevel = PhotonLogLevel.Full;
        //旧バージョンでは引数必須でしたが、PUN2では不要です。
        PhotonNetwork.ConnectUsingSettings();
    }

    void Update()
    {
        //connectionText.text = PhotonNetwork.connectionStateDetailed.ToString();
    }

    void OnGUI()
    {
        //ログインの状態を画面上に出力
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }


    //ルームに入室前に呼び出される
    public override void OnConnectedToMaster()
    {
        // "room"という名前のルームに参加する（ルームが無ければ作成してから参加する）
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        PhotonNetwork.JoinOrCreateRoom("JokerElementsRoom", new RoomOptions(), TypedLobby.Default);
    }

    //ルームに入室後に呼び出される
    public override void OnJoinedRoom()
    {
        //キャラクターを生成
        //GameObject player = PhotonNetwork.Instantiate("PlayerObject", Vector3.zero, Quaternion.identity, 0);
        //GameObject gameController = (GameObject)PhotonNetwork.Instantiate("GameController", Vector3.zero, Quaternion.identity, 0);
       //gameController.name = "GameController";
        //GameObject player = (GameObject)PhotonNetwork.Instantiate("UserController", Vector3.zero, Quaternion.identity, 0);

        GameObject player = (GameObject)Instantiate((GameObject)Resources.Load("UserController"), Vector3.zero, Quaternion.identity);
        int index = 0;
        if (PhotonNetwork.PlayerList.Length == 1)//一人目の場合
        {
            GameObject gameController = (GameObject)PhotonNetwork.Instantiate("GameController", Vector3.zero, Quaternion.identity, 0);
            //GameObject MyUser = (GameObject)PhotonNetwork.Instantiate("UserCard", Vector3.zero, Quaternion.identity, 0);
            //MyUser.GetComponent<UserCardScript>().PlayerNumber = 1;
            index = 0;
        }
        else if (PhotonNetwork.PlayerList.Length == 2)//プレイヤー2の場合
        {
            //GameObject MyUser = (GameObject)PhotonNetwork.Instantiate("UserCard", Vector3.zero, Quaternion.identity, 0);
           // MyUser.name = "";
            //MyUser.GetComponent<UserCardScript>().PlayerNumber = 2;
            index = 1;
        }

        player.GetComponent<UserPUN>().PlayerNumber = index;
        player.name = "UserController";

        //Instantiate(UIObject, Vector3.zero, Quaternion.identity);
        //自分だけが操作できるようにスクリプトを有効にする
        //MonsterScript monsterScript = monster.GetComponent<MonsterScript>();
        //monsterScript.enabled = true;
    }

    #region Photon Callbacks


    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


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


            //LoadArena();
        }
    }


    #endregion
}
