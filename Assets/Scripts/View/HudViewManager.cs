using R3;
using System;
using TD.Logic.ECS.Systems;
using TMPro;
using UnityEngine;

namespace TD.View
{
    public class HudViewManager : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI killedEnemiesCountText;
        [SerializeField]
        private TextMeshProUGUI enemiesCountText;
        [SerializeField]
        private TextMeshProUGUI totalEnemiesCountText;

        private IDisposable disposable;

        private void Start()
        {
            disposable = Disposable.Combine(
                EnemyStatisticsSystem.OnKilledEnemiesCountChanged.Subscribe(EnemyStatisticsSystem_OnKilledEnemiesCountChanged),
                EnemyStatisticsSystem.OnEnemiesCountChanged.Subscribe(EnemyStatisticsSystem_OnEnemiesCountChanged),
                EnemyStatisticsSystem.OnTotalEnemiesCountChanged.Subscribe(EnemyStatisticsSystem_OnTotalEnemiesCountChanged)
            );
        }

        private void OnDestroy()
        {
            disposable.Dispose();
        }

        private void EnemyStatisticsSystem_OnKilledEnemiesCountChanged((int previousValue, int currentValue) tuple)
        {
            killedEnemiesCountText.text = tuple.currentValue.ToString();
        }

        private void EnemyStatisticsSystem_OnEnemiesCountChanged((int previousValue, int currentValue) tuple)
        {
            enemiesCountText.text = tuple.currentValue.ToString();
        }

        private void EnemyStatisticsSystem_OnTotalEnemiesCountChanged((int previousValue, int currentValue) tuple)
        {
            totalEnemiesCountText.text = tuple.currentValue.ToString();
        }
    }
}
