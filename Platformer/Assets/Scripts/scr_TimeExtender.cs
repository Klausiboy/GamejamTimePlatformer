using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_TimeExtender : MonoBehaviour
{
    public scr_GameMan gameManager;
    private void Start()
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<scr_GameMan>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            gameManager.IncreaseTime(UnityEngine.Random.Range(2,7));
            Destroy(this.gameObject);
        }
    }
}
