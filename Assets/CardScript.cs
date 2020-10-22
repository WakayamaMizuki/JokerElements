using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CardScript : MonoBehaviour
{

    private GameObject GameController;
    private GameObject CardSprite;
    Game game;
    public int PlayerNumber;
    private GamePlayer player;
    private GamePlayer enemy;
    private bool first = true;
    // Use this for initialization
    void Start()
    {
        CardSprite = (GameObject)Resources.Load("Card");
        
        if(GameObject.Find("GameController") != null)
        {
            game = GameObject.Find("GameController").GetComponent<Game>();
        }
        else if (GameObject.Find("GameController(Clone)") != null)
        {
            game = GameObject.Find("GameController(Clone)").GetComponent<Game>();
        }       
    }

    // Update is called once per frameusing Photon.Pun;
    void Update()
    {
        if (game == null)
        {
            if (GameObject.Find("GameController") != null)
            {
                game = GameObject.Find("GameController").GetComponent<Game>();
            }
            else if (GameObject.Find("GameController(Clone)") != null)
            {
                game = GameObject.Find("GameController(Clone)").GetComponent<Game>();
            }
        }
        

        if (game != null && PlayerNumber != -1)
        {
            player = game.Player[PlayerNumber];
            enemy = game.Player[(PlayerNumber + 1) % 2];
            PlayerNumber = -1;
        }

        if (enemy != null)
        {
            if (enemy.CardSpriteChanged || player.CardSpriteChanged)
            {
                SetPlaceCard();
                
            }
            if(enemy.HandCard != null && player.HandCard != null)
            {
                CardTextureSet();
                CardPlaceSet();
            }
            
        }
    }

    void SetPlaceCard()
    {
        //場のカード
        int n = 0;
        if (player.placeCard != null)
        {
            for (int i = 0; i < player.placeCard.cardNum; i++)
            {
                if (GameObject.Find("PlayerPlaceCard" + i) == null)
                {
                    GameObject instance = (GameObject)Instantiate(CardSprite, new Vector3(i - 3, -1, 0), Quaternion.identity);
                    instance.name = "PlayerPlaceCard" + i;
                    Texture(instance.GetComponent<SpriteRenderer>(), player.placeCard.cards[i].imageIndex);
                }
                else
                {
                    Texture(GameObject.Find("PlayerPlaceCard" + i).GetComponent<SpriteRenderer>(), player.placeCard.cards[i].imageIndex);
                }
            }
            n = player.placeCard.cards.Count;
        }
        while (true)
        {
            GameObject card = GameObject.Find("PlayerPlaceCard" + n);
            if (card == null)
            {
                break;
            }
            else
            {
                Destroy(card);
            }
            n++;
        }

        n = 0;
        if (enemy.placeCard != null)
        {
            for (int i = 0; i < enemy.placeCard.cardNum; i++)
            {
                if (GameObject.Find("EnemyPlaceCard" + i) == null)
                {
                    GameObject instance = (GameObject)Instantiate(CardSprite, new Vector3(i, 1, 0), Quaternion.identity);
                    if (enemy.placeCard.cards[i] != null)
                    {
                        instance.name = "EnemyPlaceCard" + i;
                    }
                    
                    Texture(instance.GetComponent<SpriteRenderer>(), enemy.placeCard.cards[i].imageIndex);
                }
                else
                {
                    Texture(GameObject.Find("EnemyPlaceCard" + i).GetComponent<SpriteRenderer>(), enemy.placeCard.cards[i].imageIndex);
                }
            }
            n = enemy.placeCard.cards.Count;
        }
        while (true)
        {
            GameObject card = GameObject.Find("EnemyPlaceCard" + n);
            if (card == null)
            {
                break;
            }
            else
            {
                if (enemy.placeCard == null || enemy.placeCard.cards == null || enemy.placeCard.cards[n] == null) Destroy(card);
            }
            n++;
        }
        player.CardSpriteChanged = false;
        enemy.CardSpriteChanged = false;
    }



    void CardTextureSet()
    {
        
        for (int i = 0; i < Mathf.Max(5, player.HandCard.Count); i++)
        {
            GameObject card = GameObject.Find("HandCard" + i);
            if (i < player.HandCard.Count)
            {
                if (card == null)
                {
                    GameObject instance = (GameObject)Instantiate(CardSprite, new Vector3(i - 3, -3, 0), Quaternion.identity);
                    if (player.HandCard[i] != null)
                    {
                        instance.name = "HandCard" + i;
                        instance.tag = "HandCard";
                    }
                    
                    Texture(instance.GetComponent<SpriteRenderer>(), player.HandCard[i].imageIndex);
                }
                else
                {
                    Texture(card.GetComponent<SpriteRenderer>(), player.HandCard[i].imageIndex);
                }
            }
            else if (card != null)
            {
                Destroy(card);
            }
        }

        for (int i = 0; i < Mathf.Max(5, enemy.HandCard.Count); i++)
        {
            GameObject card = GameObject.Find("EnemyHandCard" + i);
            if (i < enemy.HandCard.Count)
            {
                if (card == null)
                {
                    GameObject instance = (GameObject)Instantiate(CardSprite, new Vector3(i-1, 3f, 0), Quaternion.identity);
                    instance.name = "EnemyHandCard" + i;
                    instance.tag = "EnemyHandCard";
                    Texture(instance.GetComponent<SpriteRenderer>(), enemy.HandCard[i].imageIndex);
                }
                else
                {
                    Texture(card.GetComponent<SpriteRenderer>(), enemy.HandCard[i].imageIndex);
                }
            }
            else if (card != null)
            {
                Destroy(card);
            }
        }

        
        GameObject[] cards = GameObject.FindGameObjectsWithTag("EnemyHandCard");
        for (int i = cards.Length - 1; i >= 0; i--)
        {
            if(i > enemy.HandCard.Count - 1)
            {
                Destroy(GameObject.Find("EnemyHandCard"+i));
            }
        }

        cards = GameObject.FindGameObjectsWithTag("HandCard");
        for (int i = cards.Length - 1; i >= 0; i--)
        {
            if (i > player.HandCard.Count - 1)
            {
                Destroy(GameObject.Find("HandCard" + i));
            }
        }
    }

    void CardPlaceSet()
    {
        for (int i = 0; i < player.HandCard.Count; i++)
        {
            GameObject card = GameObject.Find("HandCard" + i);
            if (player.HandCard[i].IsPrepare)
            {
                card.GetComponent<Transform>().position = new Vector3(i - 3, -2.5f, 0);
            }
            else
            {
                card.GetComponent<Transform>().position = new Vector3(i - 3, -3f, 0);
            }
        }
    }

    
    void Texture(SpriteRenderer s, int index)
    {
        s.sprite = Resources.LoadAll<Sprite>("cards3")[index];
    }
}
