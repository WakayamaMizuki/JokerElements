using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserCardScript : MonoBehaviour {

    public GamePlayer player;
    public int PlayerNumber;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (PlayerNumber == 1)
		{
			player = GameObject.Find("GameController").GetComponent<Game>().Player[0];
        }
        else
        {
			player = GameObject.Find("GameController").GetComponent<Game>().Player[1];
		}
	}
}
