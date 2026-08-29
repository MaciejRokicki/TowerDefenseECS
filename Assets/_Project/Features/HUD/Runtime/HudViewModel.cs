using TD.Features.Base.Runtime.Systems;
using TD.Features.Experience.Runtime.Systems;
using TD.Features.Statistics.Runtime.Systems;
using UnityEngine;
using UnityEngine.UIElements;

namespace TD.Features.HUD.Runtime
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
            EnemyStatisticsSystem.OnKilledEnemiesCountChanged += EnemyStatisticsSystem_OnKilledEnemiesCountChanged;
            EnemyStatisticsSystem.OnEnemiesCountChanged += EnemyStatisticsSystem_OnEnemiesCountChanged;
            EnemyStatisticsSystem.OnTotalEnemiesCountChanged += EnemyStatisticsSystem_OnTotalEnemiesCountChanged;
            ExperienceSystem.OnExperienceChanged += ExperienceSystem_OnExperienceChanged;
        }

        private void Update()
        {
            model.Health = PlayerHealthSystem.Health.Value;
            model.MaxHealth = PlayerHealthSystem.Health.MaxValue;
        }

        private void PanelRenderer_OnUIReloaded(PanelRenderer panelRenderer, VisualElement rootElement, int version)
        {
            rootElement.Q<ProgressBar>("HealthProgressBar").dataSource = model;
            rootElement.Q<Label>("HealthLabel").dataSource = model;

            rootElement.Q<VisualElement>("KilledEnemiesStatisticsElement").dataSource = model;
            rootElement.Q<VisualElement>("EnemiesStatisticsElement").dataSource = model;
            rootElement.Q<VisualElement>("TotalEnemiesStatisticsElement").dataSource = model;

            rootElement.Q<ProgressBar>("ExperienceProgressBar").dataSource = model;
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
