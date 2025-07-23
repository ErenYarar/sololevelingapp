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
    [Header("Text Referanslarý")]
    public TextMeshProUGUI nameDisplay;
    public TextMeshProUGUI ageDisplay;
    public TextMeshProUGUI weightDisplay;
    public TextMeshProUGUI heightDisplay;

    [Space]
    [Header("Input Referanslarý")]
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
        // Sadece harfleri tut (Türkçe karakterler dahil deðilse)
        string filtered = new string(input.Where(char.IsLetter).ToArray());

        if (filtered != input)
        {
            nameInput.text = filtered;
        }
    }

    void Start()
    {
        nameInput.characterLimit = 15;
        // Dropdown'larý doldur
        PopulateDropdown(ageDropdown, 18, 50);
        PopulateDropdown(weightDropdown, 50, 120);
        PopulateDropdown(heightDropdown, 155, 210);

        if (PlayerPrefs.HasKey("UserInfoSaved"))
        {
            // Bilgiler zaten kayýtlý, paneli kapat ve verileri göster
            userInfoCanvas.SetActive(false);
            ShowUserInfo();
        }
        else
        {
            // Ýlk giriþ, panel açýk
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
            // Kullanýcýya bir uyarý göstermek istiyorsan buraya TMP Text referansý ile mesaj da gösterebilirsin.
            return;
        }
        else
        {
            warningForNameField.text = ""; // Önceki uyarýyý temizle
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
        dropdown.value = 0; // varsayýlan olarak ilk deðer seçili
        dropdown.RefreshShownValue();
    }

    public void EditUserInfo()
    {
        // Daha önce girilen bilgileri InputField ve Dropdown'lara tekrar doldur
        nameInput.text = PlayerPrefs.GetString("UserName");
        ageDropdown.value = ageDropdown.options.FindIndex(option => option.text == PlayerPrefs.GetInt("UserAge").ToString());
        weightDropdown.value = weightDropdown.options.FindIndex(option => option.text == PlayerPrefs.GetInt("UserWeight").ToString());
        heightDropdown.value = heightDropdown.options.FindIndex(option => option.text == PlayerPrefs.GetInt("UserHeight").ToString());

        StatusCanvas.SetActive(false);
        userInfoCanvas.SetActive(true);
    }

}
