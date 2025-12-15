using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    public TMP_Text scoreText;

    private int score = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        UpdateScoreUI();
    }
    
    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void AddScore(int amount = 1)
    {
        score += amount;
        UpdateScoreUI();
        Debug.Log($"Score: {score}");
    }

    public int GetScore() => score;

    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Сбрасываем паузу
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
}