using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class SavedCalendarData
{
    public List<MonthSelection> months = new List<MonthSelection>();
}

[Serializable]
public class MonthSelection
{
    public string monthKey;
    public List<int> selectedDays;
}

public class Calendar : MonoBehaviour
{
    public class Day
    {
        public int dayNum;
        public GameObject obj;

        public Image tickImage;
        public bool isSelected = false;

        public Day(int dayNum, GameObject obj)
        {
            this.dayNum = dayNum;
            this.obj = obj;

            tickImage = obj.transform.Find("TickImage").GetComponent<Image>();
            tickImage.gameObject.SetActive(false);

            var btn = obj.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(ToggleSelected);
        }

        public void ToggleSelected()
        {
            isSelected = !isSelected;

            if (isSelected)
            {
                tickImage.gameObject.SetActive(true);
                Calendar.Instance.AddSelectedDay(dayNum);
            }
            else
            {
                tickImage.gameObject.SetActive(false);
                Calendar.Instance.RemoveSelectedDay(dayNum);
            }
        }

        public void ToggleSelectionWithoutNotify(bool selected)
        {
            isSelected = selected;
            tickImage.gameObject.SetActive(selected);
        }

        public void SetAsActiveDay()
        {
            var img = obj.GetComponent<Image>();
            var text = obj.GetComponentInChildren<TextMeshProUGUI>();

            img.color = new Color(1, 1, 1, 0);
            text.text = (dayNum + 1).ToString();
            text.color = Color.white;
            obj.GetComponent<Button>().interactable = true;
        }

        public void SetAsInactiveDay()
        {
            var img = obj.GetComponent<Image>();
            var text = obj.GetComponentInChildren<TextMeshProUGUI>();

            img.color = Color.grey;
            text.text = "";
            obj.GetComponent<Button>().interactable = false;
        }

        public void SetAsToday()
        {
            var img = obj.GetComponent<Image>();
            var text = obj.GetComponentInChildren<TextMeshProUGUI>();

            img.color = Color.green;
            text.text = (dayNum + 1).ToString();
            text.color = Color.black;
            obj.GetComponent<Button>().interactable = false;
        }
    }

    public static Calendar Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private List<Day> days = new List<Day>();

    public Transform[] weeks;
    public TextMeshProUGUI MonthAndYear;
    public DateTime currDate = DateTime.Now;

    private Dictionary<string, List<int>> selectedDaysPerMonth = new Dictionary<string, List<int>>();
    private const string SelectedDaysKey = "Calendar_SelectedDays";

    private void Start()
    {
        LoadSelectedDays();
        UpdateCalendar(DateTime.Now.Year, DateTime.Now.Month);
    }

    void UpdateCalendar(int year, int month)
    {
        DateTime temp = new DateTime(year, month, 1);
        int startDay = GetMonthStartDay(year, month);
        int endDay = GetTotalNumberOfDays(year, month);

        string key = $"{year:D4}-{month:D2}";

        if (days.Count == 0)
        {
            for (int w = 0; w < 6; w++)
                for (int i = 0; i < 7; i++)
                {
                    int idx = (w * 7) + i;
                    int dayIndex = idx - startDay;
                    var slot = weeks[w].GetChild(i).gameObject;
                    var d = new Day(dayIndex, slot);
                    if (idx < startDay || dayIndex >= endDay)
                        d.SetAsInactiveDay();
                    else
                        d.SetAsActiveDay();

                    if (selectedDaysPerMonth.ContainsKey(key) && selectedDaysPerMonth[key].Contains(dayIndex))
                        d.ToggleSelectionWithoutNotify(true);

                    days.Add(d);
                }
        }
        else
        {
            for (int idx = 0; idx < days.Count; idx++)
            {
                int dayIndex = idx - startDay;
                if (idx < startDay || dayIndex >= endDay)
                    days[idx].SetAsInactiveDay();
                else
                    days[idx].SetAsActiveDay();

                if (selectedDaysPerMonth.ContainsKey(key) && selectedDaysPerMonth[key].Contains(dayIndex))
                    days[idx].ToggleSelectionWithoutNotify(true);
                else
                    days[idx].ToggleSelectionWithoutNotify(false);
            }
        }

        if (DateTime.Now.Year == year && DateTime.Now.Month == month)
        {
            int todayIdx = (DateTime.Now.Day - 1) + startDay;
            days[todayIdx].SetAsToday();
        }

        MonthAndYear.text = temp.ToString("MMMM") + " " + temp.Year;
    }

    int GetMonthStartDay(int year, int month)
    {
        DateTime temp = new DateTime(year, month, 1);
        return (int)temp.DayOfWeek;
    }

    int GetTotalNumberOfDays(int year, int month)
    {
        return DateTime.DaysInMonth(year, month);
    }

    public void SwitchMonth(int direction)
    {
        currDate = currDate.AddMonths(direction);
        UpdateCalendar(currDate.Year, currDate.Month);
    }

    public void AddSelectedDay(int day)
    {
        string key = $"{currDate.Year:D4}-{currDate.Month:D2}";

        if (!selectedDaysPerMonth.ContainsKey(key))
            selectedDaysPerMonth[key] = new List<int>();

        if (!selectedDaysPerMonth[key].Contains(day))
            selectedDaysPerMonth[key].Add(day);

        SaveSelectedDays();
    }

    public void RemoveSelectedDay(int day)
    {
        string key = $"{currDate.Year:D4}-{currDate.Month:D2}";

        if (selectedDaysPerMonth.ContainsKey(key))
        {
            selectedDaysPerMonth[key].Remove(day);
            if (selectedDaysPerMonth[key].Count == 0)
                selectedDaysPerMonth.Remove(key);

            SaveSelectedDays();
        }
    }

    void SaveSelectedDays()
    {
        var data = new SavedCalendarData();

        foreach (var kvp in selectedDaysPerMonth)
        {
            data.months.Add(new MonthSelection { monthKey = kvp.Key, selectedDays = kvp.Value });
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SelectedDaysKey, json);
        PlayerPrefs.Save();
    }

    void LoadSelectedDays()
    {
        selectedDaysPerMonth.Clear();

        if (PlayerPrefs.HasKey(SelectedDaysKey))
        {
            string json = PlayerPrefs.GetString(SelectedDaysKey);
            var data = JsonUtility.FromJson<SavedCalendarData>(json);

            foreach (var month in data.months)
            {
                selectedDaysPerMonth[month.monthKey] = new List<int>(month.selectedDays);
            }
        }
    }
}
