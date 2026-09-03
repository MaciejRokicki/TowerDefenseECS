using System;
using Unity.Properties;
using UnityEngine.UIElements;

namespace TD.UI.HUD.Runtime
{
    public class HudModel : IDataSourceViewHashProvider, INotifyBindablePropertyChanged
    {
        private long version;

        private float health;
        private float maxHealth;
        private string healthDisplay;

        private int killedEnemies;
        private int enemiesCount;
        private int totalEnemies;

        private float experience;
        private float maxExperience;

        [CreateProperty]
        public float Health { get => health; set => SetHealthInternal(value); }
        [CreateProperty]
        public float MaxHealth { get => maxHealth; set => SetMaxHealthInternal(value); }
        [CreateProperty]
        public string HealthDisplay => healthDisplay;

        [CreateProperty]
        public int KilledEnemies { get => killedEnemies; set => SetValueInternal(ref killedEnemies, value, "KilledEnemies"); }
        [CreateProperty]
        public int EnemiesCount { get => enemiesCount; set => SetValueInternal(ref enemiesCount, value, "EnemiesCount"); }
        [CreateProperty]
        public int TotalEnemies { get => totalEnemies; set => SetValueInternal(ref totalEnemies, value, "TotalEnemies"); }

        [CreateProperty]
        public float Experience { get => experience; set => SetValueInternal(ref experience, value, "Experience"); }
        [CreateProperty]
        public float MaxExperience { get => maxExperience; set => SetValueInternal(ref maxExperience, value, "MaxExperience"); }

        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

        public long GetViewHashCode()
        {
            return version;
        }

        private void SetValueInternal(ref int field, int value, string propertyName)
        {
            if (field == value)
                return;

            field = value;
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(propertyName));
            ++version;
        }

        private void SetValueInternal(ref float field, float value, string propertyName)
        {
            if (field == value)
                return;

            field = value;
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(propertyName));
            ++version;
        }

        private void SetHealthInternal(float value)
        {
            if (health == value)
                return;

            health = value;
            healthDisplay = string.Format("{0}/{1}", health, maxHealth);
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs("Health"));
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs("HealthDisplay"));
            ++version;
        }

        private void SetMaxHealthInternal(float value)
        {
            if (maxHealth == value)
                return;

            maxHealth = value;
            healthDisplay = string.Format("{0}/{1}", health, maxHealth);
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs("MaxHealth"));
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs("HealthDisplay"));
            ++version;
        }
    }
}
