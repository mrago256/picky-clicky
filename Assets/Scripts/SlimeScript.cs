using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlimeScript : MonoBehaviour
{
    GameManager gameManager;
    AudioSource aud;

    public Slider timeRemainingSlider;
    public Button slimeButton;
    public Text timeRemainingText, clickRateText, healthText;
    public AudioClip slimeClickSound, hurt;

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

        slimeButton.enabled = true;
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

    public void SlimeClicked()
    {
        aud.PlayOneShot(slimeClickSound);

        if (timerRunning)
        {
            totalClicks += GameManager.clickRate;
            GameManager.canExit = false;

            if (totalClicks >= health)
            {
                GameManager.canExit = true;
                timerRunning = false;
                
                //leave screen
                gameManager.MenuExit("Slime");

                //destroy enemy
                Destroy(GameObject.FindGameObjectWithTag("Finish"));
            }
        }

        else if (!timerRunning)
        {
            timeRemainingSlider.maxValue = totalTime;

            //if timer not running when button clicked, reset and start again
            timeRemaining = 12f;
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
        slimeButton.enabled = false;
        yield return new WaitForSeconds(sec);
        slimeButton.enabled = true;
    }
}
