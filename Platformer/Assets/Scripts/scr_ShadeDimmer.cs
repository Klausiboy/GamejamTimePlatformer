using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_ShadeDimmer : MonoBehaviour
{
    scr_GameMan gameManager;
    Renderer rend;
    public float lerp;

    void Start()
    {
        rend = GetComponent<Renderer>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<scr_GameMan>();
    }

    void Update()
    {
        lerp = gameManager.timeLeft/gameManager.maxTime;
        rend.material.color = Color.Lerp(Color.black, Color.cyan, lerp);
    }
}
