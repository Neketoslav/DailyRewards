using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContainerSlotRewardView : MonoBehaviour
{
    [SerializeField]
    private Image _backgroundSelect;

    [SerializeField]
    private Image _iconCurrecny;

    [SerializeField]
    private TMP_Text _textDays;

    [SerializeField]
    private TMP_Text _countReward;

    public void SetData(Reward reward, int countDay, bool isSelect)
    {
        _iconCurrecny.sprite = reward.IconCurrency;
        _textDays.text = $"Day {countDay}";
        _countReward.text = reward.CountCurrency.ToString();
        _backgroundSelect.gameObject.SetActive(isSelect);
    }
}
