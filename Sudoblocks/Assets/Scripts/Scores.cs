using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scores : MonoBehaviour
{
    public TextMeshProUGUI scoreTxt;
    private int currentScores;
    void Start(){
        currentScores = 0;
        UpdateScoreText();
    }

    private void OnEnable(){
        GameEvents.AddScores += AddScores;
    }

    private void OnDisable(){
        GameEvents.AddScores -= AddScores;
    }

    private void AddScores(int score){
        currentScores += score;
        UpdateScoreText();
    }

    private void UpdateScoreText(){
        scoreTxt.text = currentScores.ToString();
    }
}
