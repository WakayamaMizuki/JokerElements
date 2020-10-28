using UnityEngine;
using System.Collections.Generic;
public static class GamePlayerSerializer
{
    public static void Register()
    {
        ExitGames.Client.Photon.PhotonPeer.RegisterType(typeof(GamePlayer), (byte)'G', SerializeGamePlayer, DeserializeGamePlayer);
    }

    private static byte[] SerializeGamePlayer(object i_customobject)
    {
        GamePlayer player = (GamePlayer)i_customobject;

        var bytes = new byte[28 * sizeof(int)];
        int index = 0;
        int myturn;
        if (player.MyTurn)
        {
            myturn = 1;
        }
        else
        {
            myturn = 0;
        }
        ExitGames.Client.Photon.Protocol.Serialize(player.HandCard.Count, bytes, ref index);
        ExitGames.Client.Photon.Protocol.Serialize(myturn, bytes, ref index);
        ExitGames.Client.Photon.Protocol.Serialize(player.Deck.Count, bytes, ref index);
        if (player.placeCard != null)
        {
            ExitGames.Client.Photon.Protocol.Serialize(player.placeCard.cards.Count, bytes, ref index);
        }
        else
        {
            ExitGames.Client.Photon.Protocol.Serialize(0, bytes, ref index);
        }
        for(int i = 0;i < 12; i++)
        {
            if(i < player.HandCard.Count)
            {
                ExitGames.Client.Photon.Protocol.Serialize(player.HandCard[i].num, bytes, ref index);
                ExitGames.Client.Photon.Protocol.Serialize(player.HandCard[i].mark, bytes, ref index);
            }
            else
            {
                ExitGames.Client.Photon.Protocol.Serialize(0, bytes, ref index);
                ExitGames.Client.Photon.Protocol.Serialize(0, bytes, ref index);
            }  
        }
        for (int i = 0; i < 12; i++)
        {
            if(player.placeCard != null && player.placeCard.cards != null && i < player.placeCard.cards.Count && player.placeCard.cards[i] != null)
            {
                Debug.Log(i);
                ExitGames.Client.Photon.Protocol.Serialize(player.placeCard.cards[i].num, bytes, ref index);
                ExitGames.Client.Photon.Protocol.Serialize(player.placeCard.cards[i].mark, bytes, ref index);
            }
            else
            {
                ExitGames.Client.Photon.Protocol.Serialize(0, bytes, ref index);
                ExitGames.Client.Photon.Protocol.Serialize(0, bytes, ref index);
            }
        }

        return bytes;
    }

    private static object DeserializeGamePlayer(byte[] i_bytes)
    {
        GamePlayer player = new GamePlayer();
        int index = 0;
        int HandCardNum;
        int PlaceCardNum;
        int myturn;
        List<Card> HandCard = new List<Card>();
        List<Card> placeCard = new List<Card>();
        ExitGames.Client.Photon.Protocol.Deserialize(out HandCardNum, i_bytes, ref index);
        ExitGames.Client.Photon.Protocol.Deserialize(out myturn, i_bytes, ref index);
        player.MyTurn = (myturn == 1);
        ExitGames.Client.Photon.Protocol.Deserialize(out player.DeckCount, i_bytes, ref index);
        ExitGames.Client.Photon.Protocol.Deserialize(out PlaceCardNum, i_bytes, ref index);
        for(int i = 0;i < 12; i++)
        {
            if(i < HandCardNum)
            {
                int num;
                int mark;
                ExitGames.Client.Photon.Protocol.Deserialize(out num, i_bytes, ref index);
                ExitGames.Client.Photon.Protocol.Deserialize(out mark, i_bytes, ref index);
                HandCard.Add(new Card(mark, num));
            }
            else
            {
                int num;
                int mark;
                ExitGames.Client.Photon.Protocol.Deserialize(out num, i_bytes, ref index);
                ExitGames.Client.Photon.Protocol.Deserialize(out mark, i_bytes, ref index);
            }
        }
        for (int i = 0; i < 12; i++)
        {
            if (i < PlaceCardNum)
            {
                int num;
                int mark;
                ExitGames.Client.Photon.Protocol.Deserialize(out num, i_bytes, ref index);
                ExitGames.Client.Photon.Protocol.Deserialize(out mark, i_bytes, ref index);
                placeCard.Add(new Card(mark, num));
            }
            else
            {
                int num;
                int mark;
                ExitGames.Client.Photon.Protocol.Deserialize(out num, i_bytes, ref index);
                ExitGames.Client.Photon.Protocol.Deserialize(out mark, i_bytes, ref index);
            }
        }

        player.placeCard = new PlaceCard(placeCard);
        player.HandCard = HandCard;
        
        return player;
    }

}