using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class scr_HordeController : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyReference;
    public List<GameObject> spawnPoints;
    public UnityEvent hordeDefeated;


    [Header("Settings")]
    public int hordeEnemyNumberToKill;
    public int hordeMaxEnemyCount;
    public int hordeEnemySpawnInterval;
    public Transform initializedWalkPoint;

    [HideInInspector]
    public int hordeEnemyKilledCount =0;
    [HideInInspector]
    public int hordeEnemyCount = 0;

    bool hordeActive;
    bool alreadySpawned;
    public List<GameObject> enemyList;

    public void StartHorde()
    {
        hordeActive = true;
    }
    private void Update()
    {
        if (hordeActive)
        {
            if (spawnPoints.Count==0)
            {
                Debug.Log("Need Spawnpoints");
                return;
            }

            if (!alreadySpawned && hordeEnemyCount < hordeMaxEnemyCount && hordeEnemyNumberToKill >= hordeEnemyKilledCount + hordeEnemyCount + 1) //If enough time passed and not at max number of enemiess spawn enemies
            {
                int index = Random.Range(0, spawnPoints.Count);
                GameObject enemy = Instantiate(enemyReference, spawnPoints[index].transform.position, Quaternion.identity);
                scr_EnemyAi enemyAi = enemy.GetComponent<scr_EnemyAi>();
                enemyAi.hordeController = this;
                enemyAi.SetWalkpoint(initializedWalkPoint.position);

                enemyList.Add(enemy);
                hordeEnemyCount += 1;
                alreadySpawned = true;
                Invoke(nameof(ResetSpawn), hordeEnemySpawnInterval);
            }

            if(hordeEnemyNumberToKill == hordeEnemyKilledCount)
            {
                hordeDefeated.Invoke();
            }
        }
    }

    public void EnemyDied()
    {
        hordeEnemyKilledCount += 1;
        hordeEnemyCount -= 1;
    }

    private void ResetSpawn()
    {
        alreadySpawned = false;
    }
}
