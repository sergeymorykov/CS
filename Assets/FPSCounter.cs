using UnityEngine;
using TMPro;

/// <summary>
/// Простой и оптимизированный счетчик FPS
/// </summary>
public class FPSCounter : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text fpsText;
    
    [Header("Настройки")]
    [Tooltip("Частота обновления отображения FPS (в секундах)")]
    public float updateInterval = 0.5f;

    private float deltaTime = 0f;
    private float fps = 0f;
    private float timer = 0f;

    void Update()
    {
        // Накапливаем delta time
        deltaTime += Time.unscaledDeltaTime;
        timer += Time.unscaledDeltaTime;

        // Обновляем FPS с заданным интервалом
        if (timer >= updateInterval)
        {
            fps = 1f / (deltaTime / Mathf.Max(1, Mathf.RoundToInt(timer / Time.unscaledDeltaTime)));
            
            if (fpsText != null)
            {
                fpsText.text = $"FPS: {Mathf.RoundToInt(fps)}";
                
                // Цветовая индикация производительности
                if (fps >= 60)
                    fpsText.color = Color.green;
                else if (fps >= 30)
                    fpsText.color = Color.yellow;
                else
                    fpsText.color = Color.red;
            }

            deltaTime = 0f;
            timer = 0f;
        }
    }
}

