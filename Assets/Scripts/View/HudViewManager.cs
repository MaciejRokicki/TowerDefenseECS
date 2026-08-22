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

        [SerializeField]
        private Slider experienceSlider;

        private void Start()
        {
            BaseHealthSystem.OnHealthChanged += BaseHealthSystem_OnHealthChanged;
            BaseHealthSystem.OnMaxHealthChanged += BaseHealthSystem_OnMaxHealthChanged;
            EnemyStatisticsSystem.OnKilledEnemiesCountChanged += EnemyStatisticsSystem_OnKilledEnemiesCountChanged;
            EnemyStatisticsSystem.OnEnemiesCountChanged += EnemyStatisticsSystem_OnEnemiesCountChanged;
            EnemyStatisticsSystem.OnTotalEnemiesCountChanged += EnemyStatisticsSystem_OnTotalEnemiesCountChanged;
            ExperienceSystem.OnExperienceChanged += ExperienceSystem_OnExperienceChanged;
        }

        private void BaseHealthSystem_OnHealthChanged(float previousValue, float currentValue)
        {
            healthSlider.value = currentValue;
            healthText.text = string.Concat(currentValue, '/', BaseHealthSystem.Health.MaxValue);
        }

        private void BaseHealthSystem_OnMaxHealthChanged(float previousValue, float currentValue)
        {
            healthSlider.maxValue = currentValue;
            healthText.text = string.Concat(currentValue, '/', BaseHealthSystem.Health.MaxValue);
        }

        private void EnemyStatisticsSystem_OnKilledEnemiesCountChanged(int previousValue, int currentValue)
        {
            killedEnemiesCountText.text = currentValue.ToString();
        }

        private void EnemyStatisticsSystem_OnEnemiesCountChanged(int previousValue, int currentValue)
        {
            enemiesCountText.text = currentValue.ToString();
        }

        private void EnemyStatisticsSystem_OnTotalEnemiesCountChanged(int previousValue, int currentValue)
        {
            totalEnemiesCountText.text = currentValue.ToString();
        }

        private void ExperienceSystem_OnExperienceChanged(int previousValue, int currentValue)
        {
            experienceSlider.maxValue = ExperienceSystem.MaxExperience;
            experienceSlider.value = currentValue;
        }
    }
}
