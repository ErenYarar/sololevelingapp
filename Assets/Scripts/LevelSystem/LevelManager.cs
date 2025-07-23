using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public event System.Action OnPointsChanged;
    public static LevelManager Instance;

    public int currentLevel = 0;
    public TextMeshProUGUI levelText;
    public int RemainingPoints { get; private set; } = 0;

    private bool hasLeveledUpThisSession = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentLevel = PlayerPrefs.GetInt("PlayerLevel", 0);
        RemainingPoints = PlayerPrefs.GetInt("RemainingPoints", 0);

        UpdateLevelUI();
        UIManager.Instance.UpdateRemainingPoints(RemainingPoints);
        UIManager.Instance.UpdateLevel(currentLevel);
        OnPointsChanged?.Invoke();
    }

    public void LevelUp()
    {
        // Bu kontrol tekrar level atlamayı engeller
        if (hasLeveledUpThisSession)
        {
            Debug.Log("[LevelManager] Already leveled up this session, skipping.");
            return;
        }

        currentLevel++;
        hasLeveledUpThisSession = true;

        PlayerPrefs.SetInt("PlayerLevel", currentLevel);
        AddPoints(10);

        UpdateLevelUI();
        UIManager.Instance.UpdateLevel(currentLevel);
        UIManager.Instance.UpdateRemainingPoints(RemainingPoints);

        Debug.Log("[LevelManager] LevelUp executed!");
    }

    public void UsePoint()
    {
        if (RemainingPoints > 0)
        {
            RemainingPoints--;
            PlayerPrefs.SetInt("RemainingPoints", RemainingPoints);
            UIManager.Instance.UpdateRemainingPoints(RemainingPoints);
            OnPointsChanged?.Invoke();
        }
    }

    public void AddPoints(int amount)
    {
        RemainingPoints += amount;
        PlayerPrefs.SetInt("RemainingPoints", RemainingPoints);

        UIManager.Instance.UpdateRemainingPoints(RemainingPoints);
        Debug.Log($"[LevelManager] AddPoints: {RemainingPoints}");
        OnPointsChanged?.Invoke();
    }

    public void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = "" + currentLevel;
        }
    }
}
