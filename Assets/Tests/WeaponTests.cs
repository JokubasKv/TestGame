using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class WeaponTests
    {
        GameObject characterPrefab = Resources.Load<GameObject>("Player");
        scr_CharacterController characterController;

        GameObject laserGunPrefab = Resources.Load<GameObject>("Weapon_LaserGun");
        scr_WeaponController laserGunController;
        scr_PickupController laserGunPickupController;


        [SetUp]
        public void Setup()
        {
            //Load Testing scene
            SceneManager.LoadScene("Scenes/EmptyScene");
        }

        [UnityTest]
        public IEnumerator Pick_Up_Item()
        {
            //Creating the objects
            GameObject character = MonoBehaviour.Instantiate(characterPrefab);
            characterController = character.GetComponent<scr_CharacterController>();


            GameObject laserGun = MonoBehaviour.Instantiate(laserGunPrefab, new Vector3(0, 0.7f, 6), Quaternion.identity, null);
            laserGunController = laserGun.GetComponent<scr_WeaponController>();
            laserGunPickupController = laserGun.GetComponent<scr_PickupController>();
            laserGunController.Initialise(characterController);
            laserGunPickupController.player = character.transform;

            characterController.PickUpPressed();


            yield return new WaitForSeconds(.5f);

            var expectedResult = true;

            Assert.AreEqual(expectedResult, characterController.slotFull);

            characterController.PickUpPressed();

            Assert.AreEqual(expectedResult, characterController.slotFull);
        }
        [UnityTest]
        public IEnumerator Shoot_Weapon_LaserGun()
        {
            //Creating the objects
            GameObject character = MonoBehaviour.Instantiate(characterPrefab);
            characterController = character.GetComponent<scr_CharacterController>();


            GameObject laserGun = MonoBehaviour.Instantiate(laserGunPrefab, new Vector3(0, 0.7f, 6), Quaternion.identity, null);
            laserGunController = laserGun.GetComponent<scr_WeaponController>();
            laserGunController.Initialise(characterController);
            laserGunController.fpsCam = Camera.main;

           

            var expectedResult = 99;
            laserGunController.Shoot();

            yield return null;

            Assert.AreEqual(expectedResult, laserGunController.bulletsLeft);

        }
        [UnityTest]
        public IEnumerator Shoot_Weapon_LaserGun_Burst()
        {
            //Creating the objects
            GameObject character = MonoBehaviour.Instantiate(characterPrefab);
            characterController = character.GetComponent<scr_CharacterController>();


            GameObject laserGun = MonoBehaviour.Instantiate(laserGunPrefab, new Vector3(0, 0.7f, 6), Quaternion.identity, null);
            laserGunController = laserGun.GetComponent<scr_WeaponController>();
            laserGunController.Initialise(characterController);
            laserGunController.fpsCam = Camera.main;

            laserGunController.bulletsPerTap = 5;


            var expectedResult = 95;
            laserGunController.Shoot();

            yield return new WaitForSeconds(1f);

            Assert.AreEqual(expectedResult, laserGunController.bulletsLeft);

        }


        [Test]
        public void Reload_10_Bullets_With_Laser_Gun()
        {
            //Creating the objects
            GameObject character = MonoBehaviour.Instantiate(characterPrefab);
            characterController = character.GetComponent<scr_CharacterController>();


            GameObject laserGun = MonoBehaviour.Instantiate(laserGunPrefab, new Vector3(0, 0.7f, 6), Quaternion.identity, null);
            laserGunController = laserGun.GetComponent<scr_WeaponController>();
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
            //Creating the objects
            GameObject character = MonoBehaviour.Instantiate(characterPrefab);
            characterController = character.GetComponent<scr_CharacterController>();


            GameObject laserGun = MonoBehaviour.Instantiate(laserGunPrefab, new Vector3(0, 0.7f, 6), Quaternion.identity, null);
            laserGunController = laserGun.GetComponent<scr_WeaponController>();
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


    }
}