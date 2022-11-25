using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class PlayerTests
{
    GameObject character;
    scr_CharacterController characterController;

    GameObject laserGun;
    scr_WeaponController laserGunController;
    scr_PickupController laserGunPickupController;

    GameObject grenadeLauncher;
    scr_WeaponController grenadeLauncherController;
    scr_PickupController grenadeLauncherPickUpController;

    GameObject UI;
    src_ScoreScript scoreController;
    static int[] scoreValues = new int[] { 50, 100, 120 };



    [SetUp]
    public void Setup()
    {
        // Instantiate a Player GameObject.
        // (This MUST be a prefab to create it in the test scene.)
        character = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Player"));
        characterController = character.GetComponent<scr_CharacterController>();

        laserGun = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Weapon_LaserGun"));
        laserGunController = laserGun.GetComponent<scr_WeaponController>();
        laserGunPickupController = laserGun.GetComponent<scr_PickupController>();

        grenadeLauncher = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Weapon_GrenadeLauncher"));
        grenadeLauncherController = laserGun.GetComponent<scr_WeaponController>();
        grenadeLauncherPickUpController = laserGun.GetComponent<scr_PickupController>();

        UI = MonoBehaviour.Instantiate(Resources.Load<GameObject>("UI"));
        scoreController = UI.GetComponentInChildren<src_ScoreScript>();

    }

    [Test]
    public void Heal_Player_By_10()
    {
        characterController.hitpoints = 100;
        float healValue = 10;
        int expectedValue = (int)(characterController.hitpoints + healValue);

        characterController.Heal(healValue);

        Assert.AreEqual(expectedValue, characterController.hitpoints);
    }

    [Test]
    public void Heal_Player_By_50()
    {
        characterController.hitpoints = 50;
        float healValue = 50;
        int expectedValue = (int)(characterController.hitpoints + healValue);

        characterController.Heal(healValue);

        Assert.AreEqual(expectedValue, characterController.hitpoints);
    }

    [Test]
    public void Damage_Player_By_10()
    {

        characterController.hitpoints = 100;
        int damageAmount = 10;
        var expectedResult = characterController.hitpoints - damageAmount;

        characterController.TakeDamage(damageAmount, scr_Models.DamageType.Electric);

        Assert.AreEqual(expectedResult, characterController.hitpoints);
    }

    [Test]
    public void Damage_Player_By_50()
    {
        characterController.hitpoints = 100;
        int damageAmount = 50;
        var expectedResult = characterController.hitpoints - damageAmount;

        characterController.TakeDamage(damageAmount, scr_Models.DamageType.Electric);

        Assert.AreEqual(expectedResult, characterController.hitpoints);
    }

    [Test]
    public void Take_Leathal_Damage()
    {
        characterController.hitpoints = 100;
        int damageAmount = 150;
        bool gameEnded = true;

        Assert.AreEqual(!gameEnded, characterController.gameOver);

        characterController.TakeDamage(damageAmount, scr_Models.DamageType.Blunt);

        Assert.AreEqual(gameEnded, characterController.gameOver);
    }

    [Test]
    public void Drop_Item_When_Slot_Is_Full()
    {

        characterController.currentSlot = laserGunPickupController;
        characterController.currentWeapon = laserGunController;
        laserGunPickupController.equipped = true;
        laserGunPickupController.player = character.transform;

        characterController.slotFull = true;
        var expectedResult = false;

        characterController.DropPressed();
        Assert.AreEqual(expectedResult, characterController.slotFull);
    }

    [Test]
    public void Drop_Item_When_Slot_Is_Empty()
    {
        characterController.currentSlot = laserGunPickupController;
        laserGunPickupController.equipped = false;
        laserGunPickupController.player = character.transform;

        characterController.slotFull = false;
        var expectedResult = false;

        characterController.DropPressed();
        Assert.AreEqual(expectedResult, characterController.slotFull);
    }

    [Test]
    public void Increase_Score_By_Value([ValueSource("scoreValues")] int value)
    {
        var startingResult = scoreController.GetScoreValue();
        var expectedResult = startingResult + value;

        scoreController.IncreaseScoreValue(value);

        Assert.AreEqual(expectedResult, scoreController.GetScoreValue());
    }

    [Test]
    public void Enemy_Damages_Player()
    {

    }

    [Test]
    public void Player_Damages_Enemy()
    {

    }
}
