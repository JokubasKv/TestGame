using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class PlayerTests
{

    [Test]
    public void Heal_Player_By_10()
    {
        GameObject gameObject = new GameObject();
        var characterController = gameObject.AddComponent<scr_CharacterController>();
        float healValue = 10;

        characterController.Heal(healValue);

        Assert.AreEqual(10, characterController.hitpoints);
    }

    [Test]
    public void Heal_Player_By_50()
    {
        GameObject gameObject = new GameObject();
        var characterController = gameObject.AddComponent<scr_CharacterController>();
        characterController.hitpoints = 50;
        float healValue = 50;
        int expectedValue = (int)(characterController.hitpoints + healValue);

        characterController.Heal(healValue);

        Assert.AreEqual(expectedValue, characterController.hitpoints);
    }

    [Test]
    public void Damage_Player_By_10()
    {
        GameObject gameObject = new GameObject();
        var characterController = gameObject.AddComponent<scr_CharacterController>();
        characterController.hitpoints = 100;
        int damageAmount = 10;

        var expectedResult = characterController.hitpoints - damageAmount;

        characterController.TakeDamage(damageAmount, scr_Models.DamageType.Electric);

        Assert.AreEqual(expectedResult, characterController.hitpoints);
    }

    [Test]
    public void Damage_Player_By_50()
    {
        GameObject gameObject = new GameObject();
        var characterController = gameObject.AddComponent<scr_CharacterController>();
        characterController.hitpoints = 100;
        int damageAmount = 50;

        var expectedResult = characterController.hitpoints - damageAmount;

        characterController.TakeDamage(damageAmount, scr_Models.DamageType.Electric);

        Assert.AreEqual(expectedResult, characterController.hitpoints);
    }

    [Test]
    public void Take_Leathal_Damage()
    {
        GameObject gameObject = new GameObject();
        var characterController = gameObject.AddComponent<scr_CharacterController>();
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
        GameObject gameObject = new GameObject();
        var characterController = gameObject.AddComponent<scr_CharacterController>();
        var pickUpController = gameObject.AddComponent<scr_PickupController>();
        var weaponController = gameObject.AddComponent<scr_WeaponController>();
        var weaponLayer = new LayerMask();

        characterController.slotFull = false;
        characterController.currentSlot = pickUpController;
        characterController.currentWeapon = weaponController;
        characterController.pickUpRange = 1f;
        characterController.weaponLayer = weaponLayer;

        characterController.PickUpPressed();
    }

    [Test]
    public void Pick_Up_Item_When_Slot_Is_Full()
    {
        GameObject gameObject = new GameObject();
        var characterController = gameObject.AddComponent<scr_CharacterController>();
        var pickUpController = gameObject.AddComponent<scr_PickupController>();
        var weaponController = gameObject.AddComponent<scr_WeaponController>();
        var weaponLayer = new LayerMask();

        characterController.slotFull = true;
        characterController.currentSlot = pickUpController;
        characterController.currentWeapon = weaponController;
        characterController.pickUpRange = 1f;
        characterController.weaponLayer = weaponLayer;

        characterController.PickUpPressed();
    }

    [Test]
    public void Drop_Item_When_Slot_Is_Full()
    {
        GameObject gameObject = new GameObject();
        var characterController = gameObject.AddComponent<scr_CharacterController>();
        var pickUpController = gameObject.AddComponent<scr_PickupController>();

        characterController.currentSlot = pickUpController;

        pickUpController.equipped = true;
        

        characterController.slotFull = true;
        var expectedResult = false;

        characterController.DropPressed();
        Assert.AreEqual(expectedResult, characterController.slotFull);
    }

    [Test]
    public void Drop_Item_When_Slot_Is_Empty()
    {
        GameObject gameObject = new GameObject();
        var characterController = gameObject.AddComponent<scr_CharacterController>();

        characterController.slotFull = false;
        var expectedResult = false;

        characterController.DropPressed();
        Assert.AreEqual(expectedResult, characterController.slotFull);
    }

    [Test]
    public void Walk_Forwards()
    {
        GameObject gameObject = new GameObject();
        var characterController = gameObject.AddComponent<scr_CharacterController>();
    }

    [Test]
    public void Walk_Backwards()
    {
        GameObject gameObject = new GameObject();
        var characterController = gameObject.AddComponent<scr_CharacterController>();
    }

    [Test]
    public void Walk_Left()
    {
        GameObject gameObject = new GameObject();
        var characterController = gameObject.AddComponent<scr_CharacterController>();
    }

    [Test]
    public void Walk_Right()
    {
        GameObject gameObject = new GameObject();
        var characterController = gameObject.AddComponent<scr_CharacterController>();
    }

    [Test]
    public void Sprint()
    {
        GameObject gameObject = new GameObject();
        var characterController = gameObject.AddComponent<scr_CharacterController>();
    }

    [Test]
    public void Jump()
    {
        GameObject gameObject = new GameObject();
        var characterController = gameObject.AddComponent<scr_CharacterController>();
    }

    [Test]
    public void Crouch()
    {
        GameObject gameObject = new GameObject();
        var characterController = gameObject.AddComponent<scr_CharacterController>();
    }

}
