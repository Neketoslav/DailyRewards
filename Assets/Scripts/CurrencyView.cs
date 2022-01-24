using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyView : MonoBehaviour
{
    private const string WoodKey = nameof(WoodKey);
    private const string DiamondKey = nameof(DiamondKey);

    public static CurrencyView Instance { get; private set; }

    [SerializeField]
    private TMP_Text _currentCoumtWood;

    [SerializeField]
    private TMP_Text _currentCoumtDiamond;

    private void Awake()
    {
        Instance = this;
    }
    
    public void AddWood(int value)
    {
        SaveNewCountInPlayerPrefs(WoodKey, value);

        _currentCoumtWood.text = PlayerPrefs.GetInt(WoodKey, 0).ToString();
    }

    public void AddDiamond(int value)
    {
        SaveNewCountInPlayerPrefs(DiamondKey, value);

        _currentCoumtDiamond.text = PlayerPrefs.GetInt(DiamondKey, 0).ToString();
    }

    private void SaveNewCountInPlayerPrefs(string key, int value)
    {
        var currentCount = PlayerPrefs.GetInt(key, 0);
        var newCount = currentCount + value;
        PlayerPrefs.SetInt(key, newCount);
    }

}
