

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class GameOverWindow : MonoBehaviour {

    private Text scoreText;

    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
        transform.Find("retryBtn").GetComponent<Button_UI>().ClickFunc = () => { UnityEngine.SceneManagement.SceneManager.LoadScene("Flappy 1"); };
       
    }
    private void Start()
    {
        Birds.GetInstance().OnDied += Birds_OnDied;
        Hide();
    }
    private void Birds_OnDied(object sender, System.EventArgs e)
    {
        scoreText.text = Levels.GetInstance().GetPipesPassedCount().ToString();
        Show();
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
}

