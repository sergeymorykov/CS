using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public PlayerController playerController;
    public Gun gun;
    public CrosshairController crosshair;

    private bool isPaused = false;
    private InputSystem_Actions pauseActions;

    void Awake()
    {
        pauseActions = new InputSystem_Actions();
        pauseActions.UI.Enable(); // �������� ������ UI-�����
    }

    void Update()
    {
        if (pauseActions.UI.Pause.WasPressedThisFrame())
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        if (isPaused) return;

        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        playerController?.DisableInput();
        gun?.DisableInput();

        // �������� ������
        if (crosshair != null)
            crosshair.Hide();
    }

    public void Resume()
    {
        if (!isPaused) return;

        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        playerController?.EnableInput();
        gun?.EnableInput();

        // ���������� ������
        if (crosshair != null)
            crosshair.Show();
    }

    public void RestartGame()
    {
        Resume(); // Сначала снимаем паузу
        GameManager.Instance?.RestartGame();
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        UnityEngine.Application.Quit();
#endif
    }

    void OnDestroy()
    {
        pauseActions?.Dispose();
    }
}