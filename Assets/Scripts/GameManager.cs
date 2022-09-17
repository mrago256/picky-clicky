using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    Animator playerAnimator;
    AudioSource aud, aud2;
    ClickScript clickScript;
    SlimeScript slimeScript;
    PunkScript punkScript;
    BikerScript bikerScript;
    GeraldScript geraldScript;
    PlayerMovement playerScript;
    ShopScript shopScript;
    Image life1Image, life2Image, life3Image;

    public GameObject life1, life2, life3, moneyFull, moneyEmpty, paper, envelope, background;
    public GameObject HUDCanvas, clickCanvas, slimeCanvas, punkCanvas, bikerCanvas, upgradeCanvas, geraldCanvas, winCanvas;
    public GameObject envelope1, envelope2, cityScape, gameOverImage, gameOverText, gameOverButton;
    public GameObject player, punk, biker, gerald;
    public Text moneyText, paperText;
    public AudioClip pageFlip, envelopeSound, gameOverSound, gameWinSound;

    public static int clickRate;
    public static bool canExit;
    public static bool gameWon;

    //State 0: No enemies beat
    //State 1: First letter read
    //State 2: Both slimes beat
    //State 3: Punk beat
    //state 4: Biker beat
    //state 5: Final boss beat
    public static int gameState;

    bool gameOverKnown;
    bool letterRead;
    Color dimmedHealth, normalHealth;

    void Start()
    {
        //set references to components
        playerScript = player.GetComponent<PlayerMovement>();
        playerAnimator = player.GetComponent<Animator>();

        clickScript = GameObject.Find("ClickScript").GetComponent<ClickScript>();
        slimeScript = GameObject.Find("SlimeScript").GetComponent<SlimeScript>();
        punkScript = punk.GetComponent<PunkScript>();
        bikerScript = biker.GetComponent<BikerScript>();
        geraldScript = gerald.GetComponent<GeraldScript>();
        shopScript = upgradeCanvas.GetComponent<ShopScript>();

        aud = Camera.main.GetComponent<AudioSource>();
        aud2 = Camera.main.GetComponents<AudioSource>()[1];

        gameState = 0;
        clickRate = 1;
        gameOverKnown = false;
        letterRead = false;
        gameWon = false;

        //get components to control image properties
        life1Image = life1.GetComponent<Image>();
        life2Image = life2.GetComponent<Image>();
        life3Image = life3.GetComponent<Image>();

        //get original health color
        normalHealth = life1Image.color;

        //makes the loss of health dimmed
        dimmedHealth = life1Image.color;
        dimmedHealth.a = 0.25f;

        //enable exit
        canExit = true;

        //makes future enemies and story elements not exist yet
        punk.SetActive(false);
        biker.SetActive(false);
        gerald.SetActive(false);
        envelope2.SetActive(false);
        gameOverImage.SetActive(false);
        gameOverText.SetActive(false);
        gameOverButton.SetActive(false);

        //sets all the UI elements to disabled except for HUD
        clickCanvas.SetActive(false);
        slimeCanvas.SetActive(false);
        punkCanvas.SetActive(false);
        bikerCanvas.SetActive(false);
        geraldCanvas.SetActive(false);
        upgradeCanvas.SetActive(false);
        winCanvas.SetActive(false);
        HUDCanvas.SetActive(true);

        //initializes the story elements
        playerScript.enabled = false;
        envelope.SetActive(true);
        paper.SetActive(false);
        background.SetActive(true);
    }

    void Update()
    {
        switch (PlayerMovement.lives)
        {
            case 3:
                life1Image.color = normalHealth;
                life2Image.color = normalHealth;
                life3Image.color = normalHealth;
                break;
            case 2:
                life1Image.color = normalHealth;
                life2Image.color = normalHealth;
                life3Image.color = dimmedHealth;
                break;
            case 1:
                life1Image.color = normalHealth;
                life2Image.color = dimmedHealth;
                life3Image.color = dimmedHealth;
                break;
            case 0:
                life1Image.color = dimmedHealth;
                life2Image.color = dimmedHealth;
                life3Image.color = dimmedHealth;
                break;
        }

        //display money elements on screen
        moneyText.text = ("$" + PlayerMovement.money.ToString());
        switch (PlayerMovement.money)
        {
            case 0:
                moneyFull.SetActive(false);
                moneyEmpty.SetActive(true);
                break;
            default:
                moneyFull.SetActive(true);
                moneyEmpty.SetActive(false);
                break;
        }

        //check for gameover
        if(PlayerMovement.lives <= 0 && !gameOverKnown)
        {
            GameOver();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(gameWon)
            {
                SceneManager.LoadScene("MenuScene");
            }
        }
    }

    public void TriggerEntered(GameObject obj)
    {
        //remove the hud
        HUDCanvas.SetActive(false);

        //disable movement
        playerScript.enabled = false;
        playerAnimator.enabled = false;

        //compare for which trigger player is in
        if (obj.CompareTag("JobTrigger"))
        {
            PlayerEarnMoney();
        }

        if(obj.CompareTag("Slime"))
        {
            SlimeFight();
        }

        if(obj.CompareTag("Punk"))
        {
            PunkFight();
        }

        if(obj.CompareTag("Biker"))
        {
            BikerFight();
        }

        if(obj.CompareTag("Gerald"))
        {
            GeraldFight();
        }

        if(obj.CompareTag("Upgrade"))
        {
            Upgrade();
        }
    }

    public void MenuExit(string buttonID)
    {
        //don't do anything if disabled
        if (!canExit) return;

        //hide current HUD
        if(buttonID == "Clicker")
        {
            clickScript.KillScript();
            clickCanvas.SetActive(false);
        }

        if (buttonID == "Slime")
        {
            slimeScript.KillScript();
            slimeCanvas.SetActive(false);
        }

        if(buttonID == "Punk")
        {
            punkScript.KillScript();
            punkCanvas.SetActive(false);
        }

        if(buttonID == "Biker")
        {
            bikerScript.KillScript();
            bikerCanvas.SetActive(false);
            if(gameState == 4)
            {
                Destroy(biker);
            }
        }

        if(buttonID == "Gerald")
        {
            geraldScript.KillScript();
            geraldCanvas.SetActive(false);

            if(gameWon)
            {
                GameWin();
                return;
            }
        }

        if(buttonID == "Upgrade")
        {
            shopScript.enabled = false;
            upgradeCanvas.SetActive(false);
        }

        //re-enable the player movement
        playerScript.enabled = true;
        playerAnimator.enabled = true;

        //enable the hud
        HUDCanvas.SetActive(true);

    }

    public void LetterEnabled()
    {
        //set the letter to active and make the background dark
        paper.SetActive(true);
        background.SetActive(true);
        playerScript.enabled = false;

        //change letter message depending on what stage of game it is on
        if(gameState == 0)
        {
            envelope.SetActive(false);
            gameState++;
        }

        else if(gameState == 1)
        {
            aud2.Play();
            cityScape.SetActive(false);
            paperText.text = "So, you think that you can just waltz on into MY city and do as you please? I'll have you know that I " +
                "have been looking to purchase the building that you currently reside in. The time got the best of me, I'll admit, and " +
                "I obviously haven't have the change to purchase it. If you know what's best for you, you will come see me at the southern " +
                "pier to work this out. Oh, and by the way, I've put up a forcefield around the city so you can't leave!\n\nSigned - Gerald";
            letterRead = true;
            return;
        }

        else if(gameState == 2)
        {
            paperText.text = "Haha! You fool! You absolute buffoon! You complete imbecile! I can't believe you actually fell for that!" +
                " Did you really think that I would just show up at the docks? Meet me by the tunnel at the northern side of town and we'll" +
                " have a real conversation.\n\nSigned - Gerald";
        }

        else if(gameState == 3)
        {
            paperText.text = "Wow, are you familiar with the whole \"Fool me once\" thing? Well anyway, if you're still on your " +
                "little \"quest\" looking for me, I suggest you take a swing back to your place, I have a feeling that there will be" +
                " someone waiting there for you! Mwahaha!\n\nSigned - Gerald";
        }

        aud.PlayOneShot(envelopeSound);
    }

    public void LetterClicked()
    {
        aud.PlayOneShot(pageFlip);

        if(gameState == 1 && !letterRead)
        {
            LetterEnabled();
            return;
        }

        paper.SetActive(false);
        background.SetActive(false);
        playerScript.enabled = true;
    }

    public void AdvanceStory()
    {
        if(gameState == 2)
        {
            LetterEnabled();
            punk.SetActive(true);
            envelope2.SetActive(true);
        }

        if(gameState == 3)
        {
            LetterEnabled();
            biker.SetActive(true);
            envelope1.SetActive(false);
        }

        if(gameState == 4)
        {
            gerald.SetActive(true);
        }
    }

    public void RestartGame()
    {
        //reset scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void GameWin()
    {
        aud2.Stop();
        aud.PlayOneShot(gameWinSound);

        //disable movement
        playerScript.enabled = false;

        //display gamewon stuff
        winCanvas.SetActive(true);
    }

    void GameOver()
    {
        aud2.Stop();
        aud.PlayOneShot(gameOverSound);

        gameOverKnown = true;

        //disable any canvases
        slimeCanvas.SetActive(false);
        punkCanvas.SetActive(false);

        //disable movement
        playerScript.enabled = false;

        //display gameover stuff
        HUDCanvas.SetActive(true);
        background.SetActive(true);
        gameOverImage.SetActive(true);
        gameOverText.SetActive(true);
        gameOverButton.SetActive(true);
    }

    void PlayerEarnMoney()
    {
        //set the canvas to the earning canvas
        clickCanvas.SetActive(true);

        //start ClickScript with time
        ClickScript.timeRemaining = 10f;
        ClickScript.totalTime = 10f;
        clickScript.enabled = true;
    }

    void SlimeFight()
    {
        //set the canvas to the slime fight one
        slimeCanvas.SetActive(true);

        //start slimeScript with time
        SlimeScript.timeRemaining = 12f;
        SlimeScript.totalTime = 12f;
        slimeScript.enabled = true;
    }

    void PunkFight()
    {
        //set the canvas to punk fight
        punkCanvas.SetActive(true);

        //start punkScript with time
        PunkScript.timeRemaining = 14f;
        PunkScript.totalTime = 14f;
        punkScript.enabled = true;
    }

    void BikerFight()
    {
        //set canvas to biker fight
        bikerCanvas.SetActive(true);

        //start bikerScript with time
        BikerScript.timeRemaining = 16f;
        BikerScript.totalTime = 16f;
        bikerScript.enabled = true;
    }

    void GeraldFight()
    {
        //set canvas to gerald fight
        geraldCanvas.SetActive(true);

        //start geraldScript with time
        GeraldScript.timeRemaining = 12f;
        GeraldScript.totalTime = 12f;
        geraldScript.enabled = true;
    }

    void Upgrade()
    {
        upgradeCanvas.SetActive(true);
        shopScript.enabled = true;
    }
}

