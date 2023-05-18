using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class BestScoreData
{
    public int score = 0;
}

public class Scores : MonoBehaviour
{
    public TextMeshProUGUI scoreTxt, gameOverScoreTxt;
    private BestScoreData _bestScore = new BestScoreData();
    private int _currentScore;
    private string _bestScoreKey = "bsdat";

    private void Awake()
    {
        if (BinaryDataStream.Exist(_bestScoreKey))
        {
            StartCoroutine(ReadDataFile());
        }
    }

    private IEnumerator ReadDataFile()
    {
        _bestScore = BinaryDataStream.Load<BestScoreData>(_bestScoreKey);
        yield return new WaitForEndOfFrame();
        GameEvents.UpdateBestScoreBar(_currentScore, _bestScore.score);
    }

    void Start()
    {
        _currentScore = 0;
        UpdateScoreText();
    }

    private void OnEnable()
    {
        GameEvents.AddScores += AddScores;
        GameEvents.GameOver += SaveBestScore;
    }

    private void OnDisable()
    {
        GameEvents.AddScores -= AddScores;
        GameEvents.GameOver -= SaveBestScore;
    }

    public void SaveBestScore()
    {
        BinaryDataStream.Save<BestScoreData>(_bestScore, _bestScoreKey);
    }

    private void AddScores(int amount)
    {
        _currentScore += amount;
        if (_currentScore > _bestScore.score)
        {
            _bestScore.score = _currentScore;
            SaveBestScore();
        }

        //UpdateSquareColor();
        GameEvents.UpdateBestScoreBar(_currentScore, _bestScore.score);
        UpdateScoreText();
    }

    // private void UpdateSquareColor(){
    //     if(GameEvents.UpdateSquareColor != null && _currentScore >= squareTextureData.thresholdVal){
    //         squareTextureData.UpdateColors(_currentScore);
    //         GameEvents.UpdateSquareColor(squareTextureData.currentColor);
    //     }
    // }

    private void UpdateScoreText()
    {
        scoreTxt.text = _currentScore.ToString();
        gameOverScoreTxt.text = _currentScore.ToString();
    }
}
