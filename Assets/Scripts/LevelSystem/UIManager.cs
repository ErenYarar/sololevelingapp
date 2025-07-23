using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI levelText;
    public TextMeshProUGUI remainingPointsText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Tüm sahnelerde kalsýn
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateLevel(int level)
    {
        if (levelText != null)
            levelText.text = $"{level}";
    }

    public void UpdateRemainingPoints(int points)
    {
        if (remainingPointsText != null)
            remainingPointsText.text = $"{points}";
    }
}
