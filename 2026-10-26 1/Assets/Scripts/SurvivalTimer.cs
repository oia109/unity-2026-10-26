using UnityEngine;
using UnityEngine.UI;

public class SurvivalTimer : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Text label;
    [SerializeField] private GameObject gameOverUI;

    private float elapsed;

    private void Start()
    {
        if (gameOverUI != null) gameOverUI.SetActive(false);
    }

    private void Update()
    {
        bool over = player != null && player.IsGameOver;
        if (!over) elapsed += Time.deltaTime;
        if (label != null) label.text = $"Time: {elapsed:F1}s";
        if (over && gameOverUI != null && !gameOverUI.activeSelf) gameOverUI.SetActive(true);
    }
}
