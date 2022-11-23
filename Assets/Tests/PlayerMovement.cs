using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PlayerMovement
    {
        GameObject character;
        scr_CharacterController characterController;

        [SetUp]
        public void Setup()
        {
            // Instantiate a Player GameObject.
            // (This MUST be a prefab to create it in the test scene.)
            character = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Player"));
            characterController = character.GetComponent<scr_CharacterController>();
        }
        // A Test behaves as an ordinary method
        [Test]
        public void PlayerMovementSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator PlayerMovementWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }

        [UnityTest]
        public IEnumerator Walk_Forwards()
        {
            float startingX = character.transform.position.x;
            characterController.input_Movement = new Vector2(1f, 0f);

            yield return new WaitForSeconds(1f);

            float newX = character.transform.position.x;

            Assert.AreNotEqual(startingX, newX);
        }
    }
}
