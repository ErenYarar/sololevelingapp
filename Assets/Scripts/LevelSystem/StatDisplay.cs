using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatDisplay : MonoBehaviour
{
    public enum StatType
    {
        STRENGTH,
        VITALITY,
        AGILITY,
        INTELLIGENCE,
        SENSE
    }

    public StatType statType;
    public PlayerStatsUI playerStatsUI;
    public TextMeshProUGUI statNameText;
    public TextMeshProUGUI statValueText;
    public Button increaseButton;

    private int statValue = 10;

    private void Start()
    {
        increaseButton.onClick.AddListener(OnIncreaseButtonClicked);
        if (LevelManager.Instance != null)
            LevelManager.Instance.OnPointsChanged += UpdateUI;
        int vitality = PlayerPrefs.GetInt("Stat_Vitality", 10);
        int intelligence = PlayerPrefs.GetInt("Stat_Intelligence", 10);
        playerStatsUI.LoadInitialValues(vitality, intelligence);
        LoadStatValue();
        UpdateUI();
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.OnPointsChanged -= UpdateUI;
    }

    #region Save Stats
    private void SaveStatValue()
    {
        PlayerPrefs.SetInt($"Stat_{statType}", statValue);
    }

    private void LoadStatValue()
    {
        statValue = PlayerPrefs.GetInt($"Stat_{statType}", 10); // Varsayýlan 10
    }
    #endregion

    private void OnIncreaseButtonClicked()
    {
        if (LevelManager.Instance.RemainingPoints > 0)
        {
            statValue++;
            SaveStatValue();
            LevelManager.Instance.UsePoint();

            if (playerStatsUI != null)
            {
                if (statType == StatType.VITALITY)
                    playerStatsUI.IncreaseHP(10);
                else if (statType == StatType.INTELLIGENCE)
                    playerStatsUI.IncreaseMP(10);
            }

            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (LevelManager.Instance == null || statNameText == null || statValueText == null || increaseButton == null)
            return;

        statNameText.text = $"{statType.ToString().ToUpper()}:";
        statValueText.text = $"{statValue}";
        increaseButton.interactable = LevelManager.Instance.RemainingPoints > 0;
    }
}
