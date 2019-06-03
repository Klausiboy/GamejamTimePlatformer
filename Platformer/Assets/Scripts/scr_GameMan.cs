using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_GameMan : MonoBehaviour
{
    public float maxTime = 60;
    public float timeLeft;
    public float score = 0;
    public GameObject timebooster;

    int spawnCounter;

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond+1);
        timeLeft = maxTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeft > 0)
            timeLeft -= 1 * Time.deltaTime;
        else
            GameOver();
        if (spawnCounter<25)
            SpawnTimePickup();
    }

    private void GameOver()
    {
        PlayerPrefs.SetFloat("HighScore", score);
        SceneManager.LoadScene("Game Over");
    }

    void SpawnTimePickup()
    {
        if (UnityEngine.Random.Range(0,75) == 50)
        {
            Instantiate(timebooster, new Vector3(UnityEngine.Random.Range(-20, 20), UnityEngine.Random.Range(-20, 20), 0),Quaternion.identity);
            spawnCounter++;
        }
    }

    public void IncreaseScore()
    {
        score += timeLeft;
    }

    public void IncreaseTime(float amount)
    {
        timeLeft += amount;
        spawnCounter--;
    }
}
