using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;

namespace Tests
{
    public class PlayerMovement : InputTestFixture
    {
        GameObject characterPrefab  = Resources.Load<GameObject>("Player");
        scr_CharacterController characterController;

        GameObject laserGunPrefab = Resources.Load<GameObject>("Weapon_LaserGun");
        scr_WeaponController laserGunController;
        scr_PickupController laserGunPickupController;

        Mouse mouse;
        Keyboard keyboard;

        public override void Setup()
        {
            //Load Testing scene
            SceneManager.LoadScene("Scenes/EmptyScene");
            mouse = InputSystem.AddDevice<Mouse>();
            keyboard = InputSystem.AddDevice<Keyboard>();
        }

        [UnityTest]
        public IEnumerator Walk_Forward_Keyboard()
        {
            GameObject character = MonoBehaviour.Instantiate(characterPrefab);
            characterController = character.GetComponent<scr_CharacterController>();

            float startingZ = character.transform.position.z;

            Press(keyboard.wKey);

            yield return new WaitForSeconds(1f);

            float newZ = character.transform.position.z;
            float difference = newZ - startingZ;

            Assert.IsTrue(difference > 0, "Difference is" + difference.ToString());
        }

        [UnityTest]
        public IEnumerator Walk_Right()
        {
            GameObject character = MonoBehaviour.Instantiate(characterPrefab);
            characterController = character.GetComponent<scr_CharacterController>();

            float startingX = character.transform.position.x;
            characterController.input_Movement = new Vector2(1f, 0f);

            yield return new WaitForSeconds(1f);

            float newX = character.transform.position.x;
            float difference = newX - startingX ;

            Assert.IsTrue(difference > 0, "Difference is" + difference.ToString());
        }
        [UnityTest]
        public IEnumerator Walk_Left()
        {
            GameObject character = MonoBehaviour.Instantiate(characterPrefab);
            characterController = character.GetComponent<scr_CharacterController>();

            float startingX = character.transform.position.x;
            characterController.input_Movement = new Vector2(-1f, 0f);

            yield return new WaitForSeconds(1f);

            float newX = character.transform.position.x;
            float difference = newX - startingX;

            

            Assert.IsTrue(difference < 0, "Difference is" + difference.ToString());
        }
        [UnityTest]
        public IEnumerator Walk_Forward()
        {
            GameObject character = MonoBehaviour.Instantiate(characterPrefab);
            characterController = character.GetComponent<scr_CharacterController>();

            float startingZ = character.transform.position.z;
            characterController.input_Movement = new Vector2(0f, 1f);

            yield return new WaitForSeconds(1f);

            float newZ = character.transform.position.z;
            float difference = newZ - startingZ ;

            Assert.IsTrue(difference > 0, "Difference is" + difference.ToString());
        }
        [UnityTest]
        public IEnumerator Walk_Backward()
        {
            GameObject character = MonoBehaviour.Instantiate(characterPrefab);
            characterController = character.GetComponent<scr_CharacterController>();

            float startingZ = character.transform.position.z;
            characterController.input_Movement = new Vector2(0f, -1f);

            yield return new WaitForSeconds(1f);

            float newZ = character.transform.position.z;
            float difference = newZ - startingZ;

            Assert.IsTrue(difference < 0, "Difference is" + difference.ToString());
        }


        [UnityTest]
        public IEnumerator Sprinting()
        {
            GameObject character = MonoBehaviour.Instantiate(characterPrefab);
            characterController = character.GetComponent<scr_CharacterController>();

            characterController.input_Movement = new Vector2(0f, 1f);
            characterController.ToggleSprint();

            yield return new WaitForSeconds(1f);

            Assert.IsTrue(characterController.isSprinting, "Character not sprinting : " + characterController.isSprinting);
        }

        [UnityTest]
        public IEnumerator Jumping()
        {
            //Create Player Object
            GameObject character = MonoBehaviour.Instantiate(characterPrefab);
            characterController = character.GetComponent<scr_CharacterController>();
            //Wait to hit ground
            yield return new WaitForSeconds(1f);

            //Record starting postion and then jump
            float startingY = character.transform.position.y;
            characterController.JumpPressed();

            //Wait for jump
            yield return new WaitForSeconds(0.5f);
            //Record new position and calculate difference
            float newY = character.transform.position.y;
            float difference = newY - startingY;

            //The jump suceeded if the jump difference was positive and character number of jumps increased
            Assert.AreEqual(1, characterController.numOfJumps);
            Assert.IsTrue(difference > 0, "Jumping Difference is" + difference.ToString());
        }

        [UnityTest]
        public IEnumerator Pick_Up_Item()
        {
            //Creating the objects
            GameObject character = MonoBehaviour.Instantiate(characterPrefab);
            characterController = character.GetComponent<scr_CharacterController>();


            GameObject laserGun = MonoBehaviour.Instantiate(laserGunPrefab, new Vector3(0, 0.7f, 6), Quaternion.identity,null);
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
    }
}
