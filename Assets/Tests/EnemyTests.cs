using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class EnemyTests
    {
        GameObject characterPrefab = Resources.Load<GameObject>("Player");
        scr_CharacterController characterController;

        GameObject enemyPrefab = Resources.Load<GameObject>("Ghost");
        GameObject hordePrefab = Resources.Load<GameObject>("HordeSpawner");


        [SetUp]
        public void Setup()
        {
            //Load Testing scene
            SceneManager.LoadScene("Scenes/EmptyScene");
        }

        [UnityTest]
        public IEnumerator Enemy_Attack_Player()
        {
            //Creating the objects
            GameObject character = MonoBehaviour.Instantiate(characterPrefab);
            characterController = character.GetComponent<scr_CharacterController>();


            GameObject enemy = MonoBehaviour.Instantiate(enemyPrefab, new Vector3(0, -2f, 6), Quaternion.LookRotation(character.transform.position,Vector3.up), null);

            float startingHealth = characterController.hitpoints;

            yield return new WaitForSeconds(5f);


            Assert.AreNotEqual(startingHealth, characterController.hitpoints);
        }

        [Test]
        public void Enemy_Take_Damage()
        {
            GameObject enemy = MonoBehaviour.Instantiate(enemyPrefab, new Vector3(0, -2f, 6),Quaternion.identity, null);
            scr_EnemyAi enemyScript = enemy.GetComponent<scr_EnemyAi>();

            float startingHealth = enemyScript.health;


            float damage = 10;
            enemyScript.TakeDamage(damage);




            Assert.AreEqual(enemyScript.health, startingHealth - damage);
        }

        [UnityTest]
        public IEnumerator Horde_Spawner()
        {
            //Creating the objects
            GameObject horde = MonoBehaviour.Instantiate(hordePrefab);
            scr_HordeController hordeController = horde.GetComponent<scr_HordeController>();

            hordeController.hordeEnemySpawnInterval = 1;
            hordeController.hordeMaxEnemyCount = 2;

            hordeController.StartHorde();

            yield return new WaitForSeconds(3f);

            Assert.AreEqual(2, hordeController.hordeEnemyCount);
        }




    }
}

