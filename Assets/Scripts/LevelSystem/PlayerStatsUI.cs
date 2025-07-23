using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    public TextMeshProUGUI hpText;
    public Slider hpSlider;
    public TextMeshProUGUI mpText;
    public Slider mpSlider;

    private int maxHP = 100;
    private int maxMP = 50;

    public void IncreaseHP(int amount)
    {
        maxHP += amount;
        UpdateHPUI();
    }

    public void IncreaseMP(int amount)
    {
        maxMP += amount;
        UpdateMPUI();
    }

    private void UpdateHPUI()
    {
        hpText.text = maxHP.ToString();
        hpSlider.maxValue = maxHP;
        hpSlider.value = maxHP;
    }

    private void UpdateMPUI()
    {
        mpText.text = maxMP.ToString();
        mpSlider.maxValue = maxMP;
        mpSlider.value = maxMP;
    }

    public void LoadInitialValues(int vitality, int intelligence)
    {
        maxHP = 100 + (vitality - 10) * 10;
        maxMP = 50 + (intelligence - 10) * 10;
        UpdateHPUI();
        UpdateMPUI();
    }
}
