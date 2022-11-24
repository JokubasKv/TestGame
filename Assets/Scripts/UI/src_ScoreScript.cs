using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class src_ScoreScript : MonoBehaviour
{
    public static int scoreValue = 0;
    public Text score;

    // Start is called before the first frame update
    void Start()
    {
        if (score != null)
        {
            score = GetComponent<Text>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (score != null)
        {
            score.text = "Score " + scoreValue;
        }
    }

    public static int GetScore()
    {
        return scoreValue;
    }

    public int GetScoreValue()
    {
        return scoreValue;
    }

    public static void SetScore(int value)
    {
        scoreValue = value;
    }

    public void SetScoreValue(int value)
    {
        Debug.Log("PING");
        scoreValue = value;
    }

    public static void IncreaseScore(int value)
    {
        scoreValue += value;
    }

    public void IncreaseScoreValue(int value)
    {
        Debug.Log("PING");
        scoreValue += value;
    }
}
