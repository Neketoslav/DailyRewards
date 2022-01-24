using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class WeeklyRewardController
{
    private WeeklyRewardView _weeklyRewardView;
    private List<ContainerSlotRewardView> _slot = new List<ContainerSlotRewardView>();
    private bool _isGetReward;
    public WeeklyRewardController(WeeklyRewardView weeklyRewardView)
    {
        _weeklyRewardView = weeklyRewardView;
    }

    public void RefreshView()
    {
        InitSlots();

        _weeklyRewardView.StartCoroutine(RewardsStartUpdater());

        RefreshUI();
        SubscribesButtons();
    }

    private void SubscribesButtons()
    {
        _weeklyRewardView.GetRewardButton.onClick.AddListener(ClaimReward);
        _weeklyRewardView.ResetButton.onClick.AddListener(ResetTimer);
    }

    private void ClaimReward()
    {
        if (!_isGetReward)
            return;

        var reward = _weeklyRewardView.Rewards[_weeklyRewardView.CurrentSlotInActive];

        switch (reward.RewardType)
        {
            case RewardType.Wood:
                CurrencyView.Instance.AddWood(reward.CountCurrency);
                break;
            case RewardType.Diamond:
                CurrencyView.Instance.AddDiamond(reward.CountCurrency);
                break;
        }

        _weeklyRewardView.TimeGetReward = DateTime.UtcNow;
        _weeklyRewardView.CurrentSlotInActive = (_weeklyRewardView.CurrentSlotInActive + 1) % _weeklyRewardView.Rewards.Count;

        RefreshRewardState();
    }

    private void ResetTimer()
    {
        PlayerPrefs.DeleteAll();
    }

    private IEnumerator RewardsStartUpdater()
    {
        while (true)
        {
            RefreshRewardState();
            yield return new WaitForSeconds(1);
        }
    }

    private void RefreshRewardState()
    {
        _isGetReward = true;

        if (_weeklyRewardView.TimeGetReward.HasValue)
        {
            var timeSpan = DateTime.UtcNow - _weeklyRewardView.TimeGetReward.Value;

            if (timeSpan.Seconds > _weeklyRewardView.TimeDeadline)
            {
                _weeklyRewardView.TimeGetReward = null;
                _weeklyRewardView.CurrentSlotInActive = 0;
            }
            else if (timeSpan.Seconds < _weeklyRewardView.TimeWeekCooldown)
            {
                _isGetReward = false;
            }
        }

        RefreshUI();
    }
    private void RefreshUI()
    {
        _weeklyRewardView.GetRewardButton.interactable = _isGetReward;
        if (_isGetReward)
        {
            _weeklyRewardView.TimerNewReward.text = "Reward recived";
        }
        else
        {
            if (_weeklyRewardView.TimeGetReward != null)
            {
                var nextClaimTime = _weeklyRewardView.TimeGetReward.Value.AddSeconds(_weeklyRewardView.TimeWeekCooldown);
                var currentClaimCooldown = nextClaimTime - DateTime.UtcNow;
                var timeGetReward = $"{currentClaimCooldown.Days:D2}:{currentClaimCooldown.Hours:D2}:{currentClaimCooldown.Minutes:D2}:{currentClaimCooldown.Seconds:D2}";
                _weeklyRewardView.TimerNewReward.text = timeGetReward;
            }
        }
        for (var i = 0; i < _slot.Count; i++)
        {
            _slot[i].SetData(_weeklyRewardView.Rewards[i], i + 1, i == _weeklyRewardView.CurrentSlotInActive);

        }
    }

    private void InitSlots()
    {
        for (var i = 0; i < _weeklyRewardView.Rewards.Count; i++)
        {
            var instanceSlot = Object.Instantiate(_weeklyRewardView.ContainerSlotRewardView, _weeklyRewardView.MountRootSlotsReward, false);
            _slot.Add(instanceSlot);
        }
    }

}
