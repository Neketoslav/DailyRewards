using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstalView : MonoBehaviour
{
    [SerializeField]
    private DailyRewardView _dailyRewardView;

    [SerializeField]
    private WeeklyRewardView _weeklyRewardView;

    private DailyRewardController _dailyRewardController;
    private WeeklyRewardController _weeklyRewardController;

    private void Awake()
    {
        _dailyRewardController = new DailyRewardController(_dailyRewardView);
        _weeklyRewardController = new WeeklyRewardController(_weeklyRewardView);
    }

    private void Start()
    {
        _dailyRewardController.RefreshView();
        _weeklyRewardController.RefreshView();
    }

}
