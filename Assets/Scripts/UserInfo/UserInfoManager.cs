using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class UserInfoManager : MonoBehaviour
{
    [Header("Canvases")]
    public GameObject userInfoCanvas;
    public GameObject StatusCanvas;

    [Header("WarningTMP")]
    public TextMeshProUGUI warningForNameField;

    [Space]
    [Header("Text Referanslar�")]
    public TextMeshProUGUI nameDisplay;
    public TextMeshProUGUI ageDisplay;
    public TextMeshProUGUI weightDisplay;
    public TextMeshProUGUI heightDisplay;

    [Space]
    [Header("Input Referanslar�")]
    public TMP_InputField nameInput;

    [Space]
    [Header("Dropdown References")]
    public TMP_Dropdown ageDropdown;
    public TMP_Dropdown weightDropdown;
    public TMP_Dropdown heightDropdown;

    void Awake()
    {
        nameInput.onValueChanged.AddListener(ValidateNameInput);
    }

    void ValidateNameInput(string input)
    {
        // Sadece harfleri tut (T�rk�e karakterler dahil de�ilse)
        string filtered = new string(input.Where(char.IsLetter).ToArray());

        if (filtered != input)
        {
            nameInput.text = filtered;
        }
    }

    void Start()
    {
        nameInput.characterLimit = 15;
        // Dropdown'lar� doldur
        PopulateDropdown(ageDropdown, 18, 50);
        PopulateDropdown(weightDropdown, 50, 120);
        PopulateDropdown(heightDropdown, 155, 210);

        if (PlayerPrefs.HasKey("UserInfoSaved"))
        {
            // Bilgiler zaten kay�tl�, paneli kapat ve verileri g�ster
            userInfoCanvas.SetActive(false);
            ShowUserInfo();
        }
        else
        {
            // �lk giri�, panel a��k
            userInfoCanvas.SetActive(true);
            StatusCanvas.SetActive(false);
        }
    }

    public void SubmitInfo()
    {
        string trimmedName = nameInput.text.Trim();

        if (string.IsNullOrEmpty(trimmedName))
        {
            Debug.LogWarning("The name field cannot be left blank.");
            warningForNameField.text = "The name field cannot be left blank.";
            // Kullan�c�ya bir uyar� g�stermek istiyorsan buraya TMP Text referans� ile mesaj da g�sterebilirsin.
            return;
        }
        else
        {
            warningForNameField.text = ""; // �nceki uyar�y� temizle
        }

        PlayerPrefs.SetString("UserName", nameInput.text);
        PlayerPrefs.SetInt("UserAge", int.Parse(ageDropdown.options[ageDropdown.value].text));
        PlayerPrefs.SetInt("UserWeight", int.Parse(weightDropdown.options[weightDropdown.value].text));
        PlayerPrefs.SetInt("UserHeight", int.Parse(heightDropdown.options[heightDropdown.value].text));
        PlayerPrefs.SetInt("UserInfoSaved", 1);
        PlayerPrefs.Save();

        userInfoCanvas.SetActive(false);
        ShowUserInfo();
    }

    void ShowUserInfo()
    {
        nameDisplay.text = PlayerPrefs.GetString("UserName");
        ageDisplay.text = PlayerPrefs.GetInt("UserAge").ToString();
        weightDisplay.text = PlayerPrefs.GetInt("UserWeight").ToString() + " kg";
        heightDisplay.text = PlayerPrefs.GetInt("UserHeight").ToString() + " cm";

        StatusCanvas.SetActive(true);
    }

    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    void PopulateDropdown(TMP_Dropdown dropdown, int min, int max)
    {
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        for (int i = min; i <= max; i++)
        {
            options.Add(new TMP_Dropdown.OptionData(i.ToString()));
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.value = 0; // varsay�lan olarak ilk de�er se�ili
        dropdown.RefreshShownValue();
    }

    public void EditUserInfo()
    {
        // Daha �nce girilen bilgileri InputField ve Dropdown'lara tekrar doldur
        nameInput.text = PlayerPrefs.GetString("UserName");
        ageDropdown.value = ageDropdown.options.FindIndex(option => option.text == PlayerPrefs.GetInt("UserAge").ToString());
        weightDropdown.value = weightDropdown.options.FindIndex(option => option.text == PlayerPrefs.GetInt("UserWeight").ToString());
        heightDropdown.value = heightDropdown.options.FindIndex(option => option.text == PlayerPrefs.GetInt("UserHeight").ToString());

        StatusCanvas.SetActive(false);
        userInfoCanvas.SetActive(true);
    }

}
