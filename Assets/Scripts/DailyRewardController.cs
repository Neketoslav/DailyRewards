using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class DailyRewardController
{
    private DailyRewardView _dailyRewardView;
    private List<ContainerSlotRewardView> _slot = new List<ContainerSlotRewardView>();
    private bool _isGetReward;
    public DailyRewardController(DailyRewardView dailyRewardView)
    {
        _dailyRewardView = dailyRewardView;
    }

    public void RefreshView()
    {
        InitSlots();

        _dailyRewardView.StartCoroutine(RewardsStartUpdater());

        RefreshUI();
        SubscribesButtons();
    }

    private void SubscribesButtons()
    {
        _dailyRewardView.GetRewardButton.onClick.AddListener(ClaimReward);
        _dailyRewardView.ResetButton.onClick.AddListener(ResetTimer);
    }

    private void ClaimReward()
    {
        if (!_isGetReward)
            return;

        var reward = _dailyRewardView.Rewards[_dailyRewardView.CurrentSlotInActive];

        switch (reward.RewardType)
        {
            case RewardType.Wood:
                CurrencyView.Instance.AddWood(reward.CountCurrency);
                break;
            case RewardType.Diamond:
                CurrencyView.Instance.AddDiamond(reward.CountCurrency);
                break;
        }

        _dailyRewardView.TimeGetReward = DateTime.UtcNow;
        _dailyRewardView.CurrentSlotInActive = (_dailyRewardView.CurrentSlotInActive + 1) % _dailyRewardView.Rewards.Count;

        ProgressBar(1);

        RefreshRewardState();
    }

    private void ResetTimer()
    {
        PlayerPrefs.DeleteAll();
        ProgressBar(0);
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

        if (_dailyRewardView.TimeGetReward.HasValue)
        {
            var timeSpan = DateTime.UtcNow - _dailyRewardView.TimeGetReward.Value;

            if(timeSpan.Seconds > _dailyRewardView.TimeDeadline)
            {
                _dailyRewardView.TimeGetReward = null;
                _dailyRewardView.CurrentSlotInActive = 0;
            }
            else if(timeSpan.Seconds < _dailyRewardView.TimeCooldown)
            {
                _isGetReward = false;
            }
        }

        RefreshUI();
    }
    private void RefreshUI()
    {
        _dailyRewardView.GetRewardButton.interactable = _isGetReward;
        if (_isGetReward)
        {
            _dailyRewardView.TimerNewReward.text = "Reward recived";
        }
        else
        {
            if(_dailyRewardView.TimeGetReward != null)
            {
                var nextClaimTime = _dailyRewardView.TimeGetReward.Value.AddSeconds(_dailyRewardView.TimeCooldown);
                var currentClaimCooldown = nextClaimTime - DateTime.UtcNow;
                var timeGetReward = $"{currentClaimCooldown.Days:D2}:{currentClaimCooldown.Hours:D2}:{currentClaimCooldown.Minutes:D2}:{currentClaimCooldown.Seconds:D2}";
                _dailyRewardView.TimerNewReward.text = timeGetReward;     
            }
        }
        for(var i = 0; i<_slot.Count; i++)
        {
            _slot[i].SetData(_dailyRewardView.Rewards[i], i + 1, i == _dailyRewardView.CurrentSlotInActive);

        }
    }

    private void InitSlots()
    {
        for(var i=0; i<_dailyRewardView.Rewards.Count; i++)
        {
            var instanceSlot = Object.Instantiate(_dailyRewardView.ContainerSlotRewardView, _dailyRewardView.MountRootSlotsReward, false);
            _slot.Add(instanceSlot);
        }
    }

    private void ProgressBar(int value)
    {
        if(value == 0)
        {
            _dailyRewardView._value = 0;
            _dailyRewardView.ProgressBar.fillAmount = (float)_dailyRewardView._value / _dailyRewardView.MaxValue;
        }
        else
        _dailyRewardView._value += value;
        _dailyRewardView.ProgressBar.fillAmount = (float)_dailyRewardView._value / _dailyRewardView.MaxValue;
    }
}
