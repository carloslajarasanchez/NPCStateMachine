using UnityEngine;
using TMPro; // Requerido para TextMeshPro
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI - Durante el Juego (HUD)")]
    [SerializeField] private TextMeshProUGUI hudTimeText;

    [Header("UI - Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalTimeText;

    private float _timer;
    private bool _isGameOver;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (!_isGameOver)
        {
            _timer += Time.deltaTime;
            UpdateHUD();
        }
    }

    private void UpdateHUD()
    {
        if (hudTimeText != null)
        {
            hudTimeText.text = FormatTime(_timer);
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        // Retorna el formato 00:00
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ShowGameOver()
    {
        _isGameOver = true;

        // Ocultamos el contador del HUD al morir para que no estorbe
        if (hudTimeText != null) hudTimeText.gameObject.SetActive(false);

        gameOverPanel.SetActive(true);
        finalTimeText.text = $"Tiempo aguantado: {FormatTime(_timer)}";

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}