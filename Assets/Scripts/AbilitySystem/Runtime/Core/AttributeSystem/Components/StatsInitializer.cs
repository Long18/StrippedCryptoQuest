using UnityEngine;

namespace Indigames.AbilitySystem
{
    public class StatsInitializer : MonoBehaviour
    {
        [SerializeField] private AttributeSystemBehaviour _attributeSystem;
        [SerializeField] private InitializeAttributeDatabase _database;
        public InitializeAttributeDatabase DefaultStats => _database;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_attributeSystem != null) return;
            _attributeSystem = GetComponent<AttributeSystemBehaviour>();
        }
#endif

        private void Start()
        {
            InitStats();
        }

        public void InitStats()
        {
            InitStats(_database);
        }

        public void InitStats(InitializeAttributeDatabase stats)
        {
            _database = stats;
            AttributeScriptableObject[] attributes = new AttributeScriptableObject[stats.AttributesToInitialize.Length];
            for (int i = 0; i < stats.AttributesToInitialize.Length; i++)
            {
                attributes[i] = stats.AttributesToInitialize[i].Attribute;
            }
            _attributeSystem.AddAttributes(attributes);
            foreach (var initValue in stats.AttributesToInitialize)
            {
                _attributeSystem.SetAttributeBaseValue(initValue.Attribute, initValue.Value);
            }

            _attributeSystem.UpdateAllAttributeCurrentValues();
        }
    }
}