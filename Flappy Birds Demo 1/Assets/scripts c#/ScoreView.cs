using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreView : MonoBehaviour
{

    private Text scoreText;
   private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
       
    }

    void Update()
    {
        scoreText.text = Levels.GetInstance().GetPipesPassedCount().ToString();
    }
}
