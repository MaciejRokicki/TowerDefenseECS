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

        private void Start()
        {
            EnemyStatisticsSystem.OnKilledEnemiesCountChanged += EnemyStatisticsSystem_OnKilledEnemiesCountChanged;
            EnemyStatisticsSystem.OnEnemiesCountChanged += EnemyStatisticsSystem_OnEnemiesCountChanged;
            EnemyStatisticsSystem.OnTotalEnemiesCountChanged += EnemyStatisticsSystem_OnTotalEnemiesCountChanged;
        }

        private void OnDestroy()
        {
            EnemyStatisticsSystem.OnKilledEnemiesCountChanged -= EnemyStatisticsSystem_OnKilledEnemiesCountChanged;
            EnemyStatisticsSystem.OnEnemiesCountChanged -= EnemyStatisticsSystem_OnEnemiesCountChanged;
            EnemyStatisticsSystem.OnTotalEnemiesCountChanged -= EnemyStatisticsSystem_OnTotalEnemiesCountChanged;
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
    }
}
