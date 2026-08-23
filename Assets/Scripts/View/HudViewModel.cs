using TD.Logic.ECS.Systems;
using UnityEngine;
using UnityEngine.UIElements;

namespace TD.View
{
    public class HudViewModel : MonoBehaviour
    {
        [SerializeField]
        private PanelRenderer panelRenderer;

        private HudModel model;

        private void Awake()
        {
            model = new HudModel();

            panelRenderer.RegisterUIReloadCallback(PanelRenderer_OnUIReloaded);
        }

        private void OnDestroy()
        {
            panelRenderer.UnregisterUIReloadCallback(PanelRenderer_OnUIReloaded);
        }

        private void Start()
        {
            BaseHealthSystem.OnHealthChanged += BaseHealthSystem_OnHealthChanged;
            BaseHealthSystem.OnMaxHealthChanged += BaseHealthSystem_OnMaxHealthChanged;
            EnemyStatisticsSystem.OnKilledEnemiesCountChanged += EnemyStatisticsSystem_OnKilledEnemiesCountChanged;
            EnemyStatisticsSystem.OnEnemiesCountChanged += EnemyStatisticsSystem_OnEnemiesCountChanged;
            EnemyStatisticsSystem.OnTotalEnemiesCountChanged += EnemyStatisticsSystem_OnTotalEnemiesCountChanged;
            ExperienceSystem.OnExperienceChanged += ExperienceSystem_OnExperienceChanged;
        }

        private void PanelRenderer_OnUIReloaded(PanelRenderer panelRenderer, VisualElement rootElement, int version)
        {
            rootElement.Q<ProgressBar>("HealthProgressBar").dataSource = model;
            rootElement.Q<Label>("HealthLabel").dataSource = model;

            rootElement.Q<VisualElement>("KilledEnemiesStatisticsElement").Q<Label>("StatisticsElementValue").dataSource = model;
            rootElement.Q<VisualElement>("EnemiesStatisticsElement").Q<Label>("StatisticsElementValue").dataSource = model;
            rootElement.Q<VisualElement>("TotalEnemiesStatisticsElement").Q<Label>("StatisticsElementValue").dataSource = model;

            rootElement.Q<ProgressBar>("ExperienceProgressBar").dataSource = model;
        }

        private void BaseHealthSystem_OnHealthChanged(float previousValue, float currentValue)
        {
            model.Health = currentValue;
        }

        private void BaseHealthSystem_OnMaxHealthChanged(float previousValue, float currentValue)
        {
            model.MaxHealth = currentValue;
        }

        private void EnemyStatisticsSystem_OnKilledEnemiesCountChanged(int previousValue, int currentValue)
        {
            model.KilledEnemies = currentValue;
        }

        private void EnemyStatisticsSystem_OnEnemiesCountChanged(int previousValue, int currentValue)
        {
            model.EnemiesCount = currentValue;
        }

        private void EnemyStatisticsSystem_OnTotalEnemiesCountChanged(int previousValue, int currentValue)
        {
            model.TotalEnemies = currentValue;
        }

        private void ExperienceSystem_OnExperienceChanged(int previousValue, int currentValue)
        {
            model.MaxExperience = ExperienceSystem.MaxExperience;
            model.Experience = currentValue;
        }
    }
}
