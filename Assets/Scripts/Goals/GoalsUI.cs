using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalsUI : MonoBehaviour
{
    [System.Serializable]
    public class CounterItem
    {
        public string key; // Kaydetme için benzersiz anahtar
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI counterText;
        public Button plusButton;
        [HideInInspector] public int currentValue = 0;
        public int maxValue = 100;
    }
    private bool rewardGivenGlobal = false;
    public CounterItem[] counters;
    public Button resetButton;
    private const string LastResetKey = "LastResetTime";

    void Start()
    {
        rewardGivenGlobal = PlayerPrefs.GetInt("RewardGiven", 0) == 1;
        // Değerleri geri yükle
        LoadCounters();

        // Günlük sıfırlama kontrolü
        CheckAndResetIfNeeded();

        for (int i = 0; i < counters.Length; i++)
        {
            int index = i;
            UpdateCounterText(index);

            counters[index].plusButton.onClick.AddListener(() =>
            {
                if (counters[index].currentValue < counters[index].maxValue)
                {
                    counters[index].currentValue += 10;
                    UpdateCounterText(index);
                    SaveCounter(index); // Değeri kaydet
                }
            });
        }

        resetButton.onClick.AddListener(() =>
        {
            ResetAllCounters();
            SaveAllCounters();
        });
    }

    void UpdateCounterText(int index)
    {
        counters[index].counterText.text = $"[{counters[index].currentValue}/{counters[index].maxValue}]";

        if (counters[index].currentValue >= counters[index].maxValue)
        {
            counters[index].counterText.color = Color.green;
            counters[index].TitleText.color = Color.green;
        }
        else
        {
            counters[index].counterText.color = Color.white;
            counters[index].TitleText.color = Color.white;
        }

        if (AllCountersComplete() && !rewardGivenGlobal)
        {
            rewardGivenGlobal = true;
            PlayerPrefs.SetInt("RewardGiven", 1);
            PlayerPrefs.Save();

            LevelManager.Instance.LevelUp();
        }
    }

    bool AllCountersComplete()
    {
        foreach (var counter in counters)
        {
            if (counter.currentValue < counter.maxValue)
                return false;
        }
        return true;
    }

    void ResetAllCounters()
    {
        foreach (var counter in counters)
        {
            counter.currentValue = 0;
            //counter.rewardGiven = false;
        }

        rewardGivenGlobal = false;
        PlayerPrefs.SetInt("RewardGiven", 0);
        PlayerPrefs.Save();

        for (int i = 0; i < counters.Length; i++)
        {
            UpdateCounterText(i);
        }

        PlayerPrefs.SetString(LastResetKey, DateTime.UtcNow.ToString("o"));
        PlayerPrefs.Save();
    }

    void CheckAndResetIfNeeded()
    {
        if (PlayerPrefs.HasKey(LastResetKey))
        {
            string lastTimeStr = PlayerPrefs.GetString(LastResetKey);
            if (DateTime.TryParse(lastTimeStr, out DateTime lastTime))
            {
                TimeSpan elapsed = DateTime.UtcNow - lastTime;
                if (elapsed.TotalHours >= 24)
                {
                    ResetAllCounters();
                    SaveAllCounters();
                }
            }
            else
            {
                ResetAllCounters();
                SaveAllCounters();
            }
        }
        else
        {
            ResetAllCounters();
            SaveAllCounters();
        }
    }

    void SaveCounter(int index)
    {
        string key = counters[index].key;
        PlayerPrefs.SetInt(key, counters[index].currentValue);
        PlayerPrefs.Save();
    }

    void SaveAllCounters()
    {
        foreach (var counter in counters)
        {
            PlayerPrefs.SetInt(counter.key, counter.currentValue);
        }
        PlayerPrefs.Save();
    }

    void LoadCounters()
    {
        foreach (var counter in counters)
        {
            if (PlayerPrefs.HasKey(counter.key))
            {
                counter.currentValue = PlayerPrefs.GetInt(counter.key);
            }
        }
    }
}
