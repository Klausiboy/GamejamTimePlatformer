using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class scr_GameOverManager : MonoBehaviour
{
    public Canvas firstCanvas;
    public Text txt_highscores;
    public InputField inputField;
    string path = "Assets/Resources/hiscore.txt";
    SortedDictionary<float,string> scores;
    List<KeyValuePair<float, string>> scoresFixed;

    private void Start()
    {
        scores = new SortedDictionary<float, string>();
        scoresFixed = new List<KeyValuePair<float, string>>();
    }

    public void SaveHighscore()
    {
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(inputField.text + ": " + PlayerPrefs.GetFloat("HighScore"));
        writer.Close();
        LoadScores();
        firstCanvas.enabled = false;
        DisplayScores();
    }

    private void DisplayScores()
    {
        foreach (var score in scores)
        {
            scoresFixed.Add(new KeyValuePair<float, string>(score.Key,score.Value));
        }
        for (int i = scoresFixed.Count-1; i > scoresFixed.Count-7; i--)
        {
            txt_highscores.text += $"{scoresFixed[i].Value} {scoresFixed[i].Key} points.\n";
        }
    }

    private void LoadScores()
    {
        string line;
        StreamReader reader = new StreamReader(path);
        while ((line = reader.ReadLine()) != null)
        {
            string[] splitted = line.Split(' ');
            float score = float.Parse(splitted[1], CultureInfo.InvariantCulture.NumberFormat);

            if (!scores.ContainsKey(score))
            {
                scores.Add(score, splitted[0]);
            }
        }     
    }
}
