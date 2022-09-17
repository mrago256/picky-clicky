using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    AudioSource aud;

    public Text amountText, healthText, upgradeText, clickRateText, moneyText;
    public AudioClip moneySound, errorSound;

    float totalUpgrades;
    int healthCost;
    int upgradeCost;

    void Start()
    {
        aud = Camera.main.GetComponent<AudioSource>();

        clickRateText.text = "Click Speed: 1";
        amountText.text = "$5";

        //initialize values
        totalUpgrades = 2;
        healthCost = 45;
        upgradeCost = 5;
    }

    void Update()
    {
        moneyText.text = ("$" + PlayerMovement.money.ToString());
    }

    public void AddHealth()
    {
        //if there are sufficient funds and health isn't full, subtract 75 and add health
        if(PlayerMovement.money >= healthCost && PlayerMovement.lives != 3)
        {
            StartCoroutine(UpdateMessage("HealthSuccessful"));
            aud.PlayOneShot(moneySound);
            PlayerMovement.lives++;
            PlayerMovement.money -= healthCost;
        }
        else if(PlayerMovement.money < healthCost)
        {
            aud.PlayOneShot(errorSound);
            StartCoroutine(UpdateMessage("HealthMoney"));
        }
        else
        {
            aud.PlayOneShot(errorSound);
            StartCoroutine(UpdateMessage("Lives"));
        }
    }

    //if sufficient funds, add upgrade
    public void AddUpgrade()
    {
        //if the player has enough money, add upgrade, increment the cost, and display it
        if(PlayerMovement.money >= upgradeCost)
        {
            if(GameManager.clickRate >= 8)
            {
                StartCoroutine(UpdateMessage("MaxUpgrades"));
                return;
            }

            if(GameManager.clickRate == 1)
            {
                GameManager.clickRate++;
            }
            else
            {
                GameManager.clickRate += 2;
            }
            aud.PlayOneShot(moneySound);

            PlayerMovement.money -= upgradeCost;

            upgradeCost = (int) Mathf.Pow(totalUpgrades, 2.5f);
            totalUpgrades++;

            amountText.text = ("$" + upgradeCost.ToString());

            StartCoroutine(UpdateMessage("UpgradeSuccessful"));
        }
        else
        {
            aud.PlayOneShot(errorSound);
            StartCoroutine(UpdateMessage("UpgradeMoney"));
        }

        clickRateText.text = ("Click Speed: " + GameManager.clickRate.ToString());
    }

    //if there is some kind of error, display text for a few seconds and then return
    IEnumerator UpdateMessage(string reason)
    {
        if(reason == "Lives")
        {
            healthText.text = "You already have full health!";
        }

        if(reason == "HealthMoney")
        {
            healthText.text = "You have insufficient funds!";
        }

        if(reason == "UpgradeMoney")
        {
            upgradeText.text = "You have insufficient funds!";
        }

        if(reason == "HealthSuccessful")
        {
            healthText.text = "Purchased";
        }

        if(reason == "UpgradeSuccessful")
        {
            upgradeText.text = "Purchased";
        }

        if(reason == "MaxUpgrades")
        {
            upgradeText.text = "Max upgrades reached!";
        }

        yield return new WaitForSeconds(1.5f);

        healthText.text = "Health: Will replenish one heart of health";
        upgradeText.text = "Click Speed: Will increase the click speed by two";
    }

    IEnumerator UpgradeUpdateMessage()
    {
        return null;
    }
}
