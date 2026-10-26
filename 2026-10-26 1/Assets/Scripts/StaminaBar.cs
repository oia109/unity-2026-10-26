using UnityEngine;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private RectTransform fill;

    private void Update()
    {
        if (player == null || fill == null) return;
        fill.anchorMax = new Vector2(Mathf.Clamp01(player.Stamina / player.MaxStamina), 1f);
    }
}
