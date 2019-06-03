using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Pickup : MonoBehaviour
{
    public scr_GameMan gameManager;
    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<scr_GameMan>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            gameManager.IncreaseScore();
            Destroy(this.gameObject);
        }
    }
}
