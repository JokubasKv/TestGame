using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class PlayerMovement
    {
        GameObject characterPrefab  = Resources.Load<GameObject>("Player");
        scr_CharacterController characterController;

        [SetUp]
        public void Setup()
        {
            //Load Testing scene
            SceneManager.LoadScene("Scenes/EmptyScene");
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
            GameObject character = MonoBehaviour.Instantiate(characterPrefab);
            characterController = character.GetComponent<scr_CharacterController>();
            yield return new WaitForSeconds(1f);

            float startingY = character.transform.position.y;
            characterController.JumpPressed();

            yield return new WaitForSeconds(0.5f);
            float newY = character.transform.position.y;
            float difference = newY - startingY;

            Assert.AreEqual(1, characterController.numOfJumps);
            Assert.IsTrue(difference > 0, "Jumping Difference is" + difference.ToString());
        }
    }
}
