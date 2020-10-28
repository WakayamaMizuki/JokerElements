using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserName : MonoBehaviour {

    public static string Name = "";
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void NameSet(string name){
        Name = name;
    }
}
