using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;


public class Game_Handler : MonoBehaviour
{
    
    private void Start()
    {
        Debug.Log("Game_Handler.Start");
  

        GameObject gameObject = new GameObject("Pipe", typeof(SpriteRenderer));
        gameObject.GetComponent<SpriteRenderer>().sprite = Game_Assets.GetInstance().pipeHeadSprite;

    }

   
}
