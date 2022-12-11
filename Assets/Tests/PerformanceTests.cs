using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class PerformanceTests
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

        [Test, Performance]
        public void TestTest()
        {
            Measure.Method(Counter).Run();
        }

        private static void Counter()
        {
            var sum = 0;
            for (var i = 0; i < 10000000; i++)
            {
                sum += i * 3 / 3;
            }
        }


    }
}