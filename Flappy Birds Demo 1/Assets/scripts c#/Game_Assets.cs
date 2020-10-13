using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Assets : MonoBehaviour
{
    private static Game_Assets instance;

    public static Game_Assets GetInstance()
    {
        return instance;

    }
    private void Awake()
    {
        instance = this;

    }

    public Sprite pipeHeadSprite;
    public Transform pfPipeHead;
    public Transform pfPipeBody;
}
   
    
    