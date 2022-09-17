using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickScript : MonoBehaviour
{
    AudioSource aud;

    public Slider timeRemainingSlider;
    public Button grassButton;
    public Text timeRemainingText, totalClickText, NPCText;
    public AudioClip grassClickSound;

    public static float timeRemaining;
    public static float totalTime;
    public static bool timerRunning;

    int totalClicks;

    void Start()
    {
        aud = Camera.main.GetComponent<AudioSource>();

        //initialize click value
        totalClicks = 0;
        totalClickText.text = "0 clicks";

        grassButton.enabled = true;
    }

    void Update()
    {        
        //make the time tick down
        if(timerRunning)
        {
            if(timeRemaining > 0)
            {
                timerRunning = true;
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                timerRunning = false;
                timeRemaining = 0;
                StartCoroutine(DisableButtonTime(1));
            }
        }
        //display the time remaining
        DisplayTime(timeRemaining);
        timeRemainingSlider.value = timeRemaining;
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeRemainingText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void GrassClicked()
    {
        aud.PlayOneShot(grassClickSound);

        if(timerRunning)
        {
            totalClicks += GameManager.clickRate;
            totalClickText.text = (totalClicks + " clicks");

            if(totalClicks >= 45)
            {
                NPCText.text = "Thank you for cutting that! Here's $10, come back any time for more, I have an infinite amount of grass!";
                PlayerMovement.money += 10;
                timerRunning = false;
                totalClickText.text = "45 clicks";

                StartCoroutine(DisableButtonTime(1));
            }
        }

        else if(!timerRunning)
        {
            timeRemainingSlider.maxValue = totalTime;

            //if timer not running when button clicked, reset and start again
            timeRemaining = 10f;
            timerRunning = true;
            totalClicks = 0;
        }
    }

    //kill the script
    public void KillScript()
    {
        //set values for next time menu opened
        timerRunning = false;
        totalClicks = 0;
        totalClickText.text = "0 clicks";
        NPCText.text = "Hey! I need some of my grass cut. If you can cut 45 patches in 10 seconds I'll pay you $10!";

        this.enabled = false;
    }

    IEnumerator DisableButtonTime(int sec)
    {
        grassButton.enabled = false;
        yield return new WaitForSeconds(sec);
        grassButton.enabled = true;
    }
}
