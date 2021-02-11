//using UnityEngine;
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

        int pCount = 0;
        if (player.placeCard != null) pCount = player.placeCard.cards.Count;
        var bytes = new byte[((pCount + player.HandCard.Count + player.Deck.Count + player.DisCard.Count) * 3 + 7) * sizeof(int)];
        int index = 0;
        int myturn = BoolToInt(player.MyTurn);
        int Is7 = BoolToInt(player.Is7);
        int Is10 = BoolToInt(player.Is10);

        ExitGames.Client.Photon.Protocol.Serialize(player.HandCard.Count, bytes, ref index);
        //Debug.Log("Send: HandCardCount "+player.HandCard.Count);
        ExitGames.Client.Photon.Protocol.Serialize(myturn, bytes, ref index);
        //Debug.Log("Send: myturn " + myturn);
        ExitGames.Client.Photon.Protocol.Serialize(player.Deck.Count, bytes, ref index);
        //Debug.Log("Send: DeckCount " + player.Deck.Count);
        ExitGames.Client.Photon.Protocol.Serialize(pCount, bytes, ref index);
        //Debug.Log("Send: pCount " + pCount);
        ExitGames.Client.Photon.Protocol.Serialize(player.DisCard.Count, bytes, ref index);

        ExitGames.Client.Photon.Protocol.Serialize(Is7, bytes, ref index);
        ExitGames.Client.Photon.Protocol.Serialize(Is10, bytes, ref index);
        for (int i = 0;i < player.HandCard.Count; i++)
        {
            ExitGames.Client.Photon.Protocol.Serialize(player.HandCard[i].getImageNum(), bytes, ref index);
            ExitGames.Client.Photon.Protocol.Serialize(player.HandCard[i].getNum(), bytes, ref index);
            ExitGames.Client.Photon.Protocol.Serialize(player.HandCard[i].getMark(), bytes, ref index);
            //Debug.Log("Send: HandCard " + player.HandCard[i].getNum() + " " + player.HandCard[i].getMark());
        }
        for (int i = 0; i < pCount; i++)
        {
            ExitGames.Client.Photon.Protocol.Serialize(player.placeCard.cards[i].getImageNum(), bytes, ref index);
            ExitGames.Client.Photon.Protocol.Serialize(player.placeCard.cards[i].getNum(), bytes, ref index);
            ExitGames.Client.Photon.Protocol.Serialize(player.placeCard.cards[i].getMark(), bytes, ref index);
            //Debug.Log("Send: placeCard " + player.placeCard.cards[i].getNum() + " " + player.placeCard.cards[i].getMark());
        }
        for(int i = 0;i < player.Deck.Count; i++)
        {
            ExitGames.Client.Photon.Protocol.Serialize(player.Deck[i].getImageNum(), bytes, ref index);
            ExitGames.Client.Photon.Protocol.Serialize(player.Deck[i].getNum(), bytes, ref index);
            ExitGames.Client.Photon.Protocol.Serialize(player.Deck[i].getMark(), bytes, ref index);
        }
        for (int i = 0; i < player.DisCard.Count; i++)
        {
            ExitGames.Client.Photon.Protocol.Serialize(player.DisCard[i].getImageNum(), bytes, ref index);
            ExitGames.Client.Photon.Protocol.Serialize(player.DisCard[i].getNum(), bytes, ref index);
            ExitGames.Client.Photon.Protocol.Serialize(player.DisCard[i].getMark(), bytes, ref index);
        }

        return bytes;
    }

    private static object DeserializeGamePlayer(byte[] i_bytes)
    {
        GamePlayer player = new GamePlayer();
        int index = 0;
        List<Card> HandCard = new List<Card>();
        List<Card> Deck = new List<Card>();
        List<Card> placeCard = new List<Card>();
        List<Card> DisCard = new List<Card>();
        ExitGames.Client.Photon.Protocol.Deserialize(out int HandCardNum, i_bytes, ref index);
        //Debug.Log("Receive: HandCardCount" + HandCardNum);
        ExitGames.Client.Photon.Protocol.Deserialize(out int myturn, i_bytes, ref index);
        //Debug.Log("Receive: myturn" + myturn);
        player.MyTurn = (myturn == 1);
        ExitGames.Client.Photon.Protocol.Deserialize(out player.DeckCount, i_bytes, ref index);
        //Debug.Log("Receive: DeckCount" + player.DeckCount);
        ExitGames.Client.Photon.Protocol.Deserialize(out int PlaceCardNum, i_bytes, ref index);
        //Debug.Log("Receive: pCount" + PlaceCardNum);
        ExitGames.Client.Photon.Protocol.Deserialize(out int DisCardNum, i_bytes, ref index);

        ExitGames.Client.Photon.Protocol.Deserialize(out int Is7, i_bytes, ref index);
        ExitGames.Client.Photon.Protocol.Deserialize(out int Is10, i_bytes, ref index);
        player.Is7 = (Is7 == 1);
        player.Is10 = (Is10 == 1);
        for (int i = 0;i < HandCardNum; i++)
        {
            ExitGames.Client.Photon.Protocol.Deserialize(out int imageNum, i_bytes, ref index);
            ExitGames.Client.Photon.Protocol.Deserialize(out int num, i_bytes, ref index);
            ExitGames.Client.Photon.Protocol.Deserialize(out int mark, i_bytes, ref index);
            Card card = new Card(mark, imageNum);
            card.setNum(num);
            HandCard.Add(card);
            //Debug.Log("Receive: HandCard " + num + " " + mark);
        }
        for (int i = 0; i < PlaceCardNum; i++)
        {
            ExitGames.Client.Photon.Protocol.Deserialize(out int imageNum, i_bytes, ref index);
            ExitGames.Client.Photon.Protocol.Deserialize(out int num, i_bytes, ref index);
            ExitGames.Client.Photon.Protocol.Deserialize(out int mark, i_bytes, ref index);
            Card card = new Card(mark, imageNum);
            card.setNum(num);
            placeCard.Add(card);
        }
        for (int i = 0; i < player.DeckCount; i++)
        {
            ExitGames.Client.Photon.Protocol.Deserialize(out int imageNum, i_bytes, ref index);
            ExitGames.Client.Photon.Protocol.Deserialize(out int num, i_bytes, ref index);
            ExitGames.Client.Photon.Protocol.Deserialize(out int mark, i_bytes, ref index);
            Card card = new Card(mark, imageNum);
            card.setNum(num);
            Deck.Add(card);
        }
        for (int i = 0; i < DisCardNum; i++)
        {
            ExitGames.Client.Photon.Protocol.Deserialize(out int imageNum, i_bytes, ref index);
            ExitGames.Client.Photon.Protocol.Deserialize(out int num, i_bytes, ref index);
            ExitGames.Client.Photon.Protocol.Deserialize(out int mark, i_bytes, ref index);
            Card card = new Card(mark, imageNum);
            card.setNum(num);
            DisCard.Add(card);
        }

        if(placeCard.Count > 0)
        {
            player.placeCard = new PlaceCard(placeCard);
        }
        else
        {
            player.placeCard = null;
        }
        
        player.HandCard = HandCard;
        player.Deck = Deck;
        player.DisCard = DisCard;
        
        return player;
    }

    private static int BoolToInt(bool b)
    {
        if (b) return 1;
        return 0;
    }
}