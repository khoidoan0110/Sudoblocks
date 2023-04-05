using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverPopup : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;

    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.GameOver += OnGameOver;
    }

    private void OnDisable()
    {
        GameEvents.GameOver -= OnGameOver;
    }

    private void OnGameOver()
    {
        AudioManager.instance.PlaySFX("Lose", 1f);
        AudioManager.instance.StopMusic();
        gameOverPanel.SetActive(true);
    }
}
