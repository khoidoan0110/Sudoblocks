using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BestScoreBar : MonoBehaviour
{
    public Image fillImage;
    public TextMeshProUGUI bestScoreText;

    private void OnEnable(){
        GameEvents.UpdateBestScoreBar += UpdateBestScoreBar;
    }

    private void OnDisable(){
        GameEvents.UpdateBestScoreBar -= UpdateBestScoreBar;
    }

    private void UpdateBestScoreBar(int currentScore, int bestScore){
        float currentPercentage = (float)currentScore / (float)bestScore;
        fillImage.fillAmount = currentPercentage;
        bestScoreText.text = bestScore.ToString();
    }
}
