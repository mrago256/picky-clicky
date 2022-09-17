using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BikerScript : MonoBehaviour
{
    GameManager gameManager;
    AudioSource aud;

    public Slider timeRemainingSlider;
    public Button bikerButton;
    public Text timeRemainingText, clickRateText, healthText, bikerText;
    public AudioClip bikerClickSound, hurt;

    public static float timeRemaining;
    public static float totalTime;
    public static bool timerRunning;
    public int health;

    int totalClicks;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        aud = Camera.main.GetComponent<AudioSource>();

        //initialize click value
        totalClicks = 0;
        clickRateText.text = ("Damage per Click: " + GameManager.clickRate.ToString());

        bikerButton.enabled = true;
    }

    void Update()
    {
        clickRateText.text = ("Damage per Click: " + GameManager.clickRate.ToString());

        //make the time tick down
        if (timerRunning)
        {
            if (timeRemaining > 0)
            {
                timerRunning = true;
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                timerRunning = false;
                timeRemaining = 0;
                aud.PlayOneShot(hurt);
                Lost();
                StartCoroutine(DisableButtonTime(1));
            }
        }
        //display the time remaining
        DisplayTime(timeRemaining);
        timeRemainingSlider.value = timeRemaining;

        healthText.text = ("Enemy Health: " + (health - totalClicks).ToString());
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeRemainingText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void Lost()
    {
        PlayerMovement.lives--;
        GameManager.canExit = true;
    }

    public void BikerClicked()
    {
        aud.PlayOneShot(bikerClickSound);

        if (timerRunning)
        {
            totalClicks += GameManager.clickRate;
            GameManager.canExit = false;

            if (totalClicks >= health)
            {
                PlayerMovement.money += 15;
                GameManager.canExit = true;
                timerRunning = false;
                totalClicks = health;

                bikerText.text = "Okay, you've proven your worthiness to meet Gerald face to face. He's somewhere west of here.";

                GameManager.gameState++;
                gameManager.AdvanceStory();
                bikerButton.enabled = false;
            }
        }

        else if (!timerRunning)
        {
            timeRemainingSlider.maxValue = totalTime;

            //if timer not running when button clicked, reset and start again
            timeRemaining = 16f;
            totalClicks = 0;
            timerRunning = true;
        }
    }

    //kill the script
    public void KillScript()
    {
        //set values for next time menu opened
        timerRunning = false;
        totalClicks = 0;

        this.enabled = false;
    }

    IEnumerator DisableButtonTime(int sec)
    {
        bikerButton.enabled = false;
        yield return new WaitForSeconds(sec);
        bikerButton.enabled = true;
    }
}
