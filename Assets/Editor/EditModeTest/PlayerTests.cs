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
        scoreController = UI.GetComponent<src_ScoreScript>();

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
    public void Pick_Up_Item_When_Slot_Is_Empty()
    {
        // needs fixing
        characterController.slotFull = false;
        characterController.currentSlot = laserGunPickupController;
        characterController.pickUpRange = 1f;

        laserGunPickupController.player = character.transform;

        var expectedResult = true;

        Assert.AreEqual(!expectedResult, characterController.slotFull);

        characterController.PickUpPressed();

        Assert.AreEqual(expectedResult, characterController.slotFull);
    }

    [Test]
    public void Pick_Up_Item_When_Slot_Is_Full()
    {
        // needs fixing
        characterController.slotFull = true;
        characterController.currentSlot = laserGunPickupController;
        characterController.currentWeapon = laserGunController;
        characterController.pickUpRange = 1f;

        laserGunPickupController.player = character.transform;

        var expectedResult = false;

        Assert.AreEqual(expectedResult, characterController.slotFull);

        characterController.PickUpPressed();

        Assert.AreEqual(expectedResult, characterController.slotFull);
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
    public void Increase_Score_By_50()
    {
        // IDK kas cia negerai
        //scoreController.score = UI.GetComponent<Text>();
        var expectedResult = 50;

        scoreController.IncreaseScoreValue(50);

        Assert.AreEqual(expectedResult, scoreController.GetScoreValue());
    }

    [Test]
    public void Increase_Score_By_100()
    {
        // IDK kas cia negerai
        //scoreController.score = UI.GetComponent<Text>();
        var expectedResult = 100;

        scoreController.IncreaseScoreValue(100);

        Assert.AreEqual(expectedResult, scoreController.GetScoreValue());
    }

    [Test]
    public void Shoot_With_Laser_Gun()
    {
        // needs fixing
        characterController.currentSlot = laserGunPickupController;
        characterController.currentWeapon = laserGunController;
        laserGunPickupController.equipped = true;
        laserGunPickupController.player = character.transform;

        characterController.slotFull = true;

        laserGunController.MagazineSize = 100;
        laserGunController.fpsCam = laserGun.GetComponent<Camera>();
        var expectedResult = 99;
        laserGunController.Shoot();

        Assert.AreEqual(expectedResult, laserGunController.bulletsLeft);

    }

    [Test]
    public void Shoot_With_Grenade_Launcher()
    {
        // needs fixing
        characterController.currentSlot = grenadeLauncherPickUpController;
        characterController.currentWeapon = grenadeLauncherController;
        grenadeLauncherPickUpController.equipped = true;
        grenadeLauncherPickUpController.player = character.transform;

        characterController.slotFull = true;

        grenadeLauncherController.MagazineSize = 100;
        grenadeLauncherController.fpsCam = laserGun.GetComponent<Camera>();
        var expectedResult = 99;
        grenadeLauncherController.Shoot();

        Assert.AreEqual(expectedResult, grenadeLauncherController.bulletsLeft);

    }

    [Test]
    public void Reload_10_Bullets_With_Laser_Gun()
    {
        characterController.currentSlot = laserGunPickupController;
        characterController.currentWeapon = laserGunController;
        laserGunPickupController.equipped = true;
        laserGunPickupController.player = character.transform;

        characterController.slotFull = true;

        laserGunController.MagazineSize = 100;
        laserGunController.bulletsLeft = 90;

        var expectedResult = 100;

        laserGunController.Reload();

        Assert.AreEqual(expectedResult, laserGunController.bulletsLeft);
    }

    [Test]
    public void Reload_50_Bullets_With_Laser_Gun()
    {
        characterController.currentSlot = laserGunPickupController;
        characterController.currentWeapon = laserGunController;
        laserGunPickupController.equipped = true;
        laserGunPickupController.player = character.transform;

        characterController.slotFull = true;

        laserGunController.MagazineSize = 50;
        laserGunController.bulletsLeft = 0;

        var expectedResult = 50;

        laserGunController.Reload();

        Assert.AreEqual(expectedResult, laserGunController.bulletsLeft);
    }

    [Test]
    public void Reload_10_Bullets_With_Grenade_Launcher()
    {
        characterController.currentSlot = grenadeLauncherPickUpController;
        characterController.currentWeapon = grenadeLauncherController;
        grenadeLauncherPickUpController.equipped = true;
        grenadeLauncherPickUpController.player = character.transform;

        characterController.slotFull = true;

        grenadeLauncherController.MagazineSize = 10;
        grenadeLauncherController.bulletsLeft = 0;

        var expectedResult = 10;

        grenadeLauncherController.Reload();

        Assert.AreEqual(expectedResult, grenadeLauncherController.bulletsLeft);
    }

    [Test]
    public void Reload_5_Bullets_With_Grenade_Launcher()
    {
        characterController.currentSlot = grenadeLauncherPickUpController;
        characterController.currentWeapon = grenadeLauncherController;
        grenadeLauncherPickUpController.equipped = true;
        grenadeLauncherPickUpController.player = character.transform;

        characterController.slotFull = true;

        grenadeLauncherController.MagazineSize = 10;
        grenadeLauncherController.bulletsLeft = 5;

        var expectedResult = 10;

        grenadeLauncherController.Reload();

        Assert.AreEqual(expectedResult, grenadeLauncherController.bulletsLeft);
    }

    [Test]
    public void Sprint()
    {

    }

    [Test]
    public void Jump()
    {

    }

    [Test]
    public void Crouch()
    {

    }

}
