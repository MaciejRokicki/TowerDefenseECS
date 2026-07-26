using R3;
using System;
using TD.Logic.ECS.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TD.View
{
    public class HudViewManager : MonoBehaviour
    {
        [SerializeField]
        private Slider healthSlider;
        [SerializeField]
        private TextMeshProUGUI healthText;

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
                BaseHealthSystem.OnHealthChanged.Subscribe(BaseHealthSystem_OnHealthChanged),
                BaseHealthSystem.OnMaxHealthChanged.Subscribe(BaseHealthSystem_OnMaxHealthChanged),
                EnemyStatisticsSystem.OnKilledEnemiesCountChanged.Subscribe(EnemyStatisticsSystem_OnKilledEnemiesCountChanged),
                EnemyStatisticsSystem.OnEnemiesCountChanged.Subscribe(EnemyStatisticsSystem_OnEnemiesCountChanged),
                EnemyStatisticsSystem.OnTotalEnemiesCountChanged.Subscribe(EnemyStatisticsSystem_OnTotalEnemiesCountChanged)
            );
        }

        private void OnDestroy()
        {
            disposable.Dispose();
        }

        private void BaseHealthSystem_OnHealthChanged((float previousValue, float currentValue) tuple)
        {
            healthSlider.value = tuple.currentValue;
            healthText.text = string.Concat(tuple.currentValue, '/', BaseHealthSystem.Health.MaxValue);
        }

        private void BaseHealthSystem_OnMaxHealthChanged((float previousValue, float currentValue) tuple)
        {
            healthSlider.maxValue = tuple.currentValue;
            healthText.text = string.Concat(tuple.currentValue, '/', BaseHealthSystem.Health.MaxValue);
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
