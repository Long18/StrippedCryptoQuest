using System;

namespace Indigames.AbilitySystem
{
    [Serializable]
    public class AttributeValue
    {
        public AttributeScriptableObject Attribute;
        public float BaseValue;
        public float CurrentValue;
        /// <summary>
        ///For skill/effect external stats
        /// </summary>
        public Modifier Modifier = new Modifier();
        public Modifier CoreModifier = new Modifier();

        public AttributeValue()
        {
        }

        public AttributeValue(AttributeScriptableObject attribute)
        {
            Attribute = attribute;
        }

        public AttributeValue Clone()
        {
            return new AttributeValue()
            {
                CurrentValue = CurrentValue,
                BaseValue = BaseValue,
                Modifier =  Modifier,
                CoreModifier = CoreModifier,
                Attribute = Attribute
            };
        }
    }

    [Serializable]
    public class Modifier
    {
        public EAttributeModifierType currentType;
        public float Additive;
        public float Multiplicative;
        public float Overriding;

        /// <summary>
        /// Will use the last modifier Override value
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Modifier operator +(Modifier a, Modifier b)
        {
            return new Modifier()
            {
                Additive = a.Additive + b.Additive,
                Multiplicative = a.Multiplicative + b.Multiplicative,
                Overriding = b.Overriding
            };
        }
    }

    [Serializable]
    public struct AttributeModifier
    {
        public AttributeScriptableObject Attribute;
        public Modifier Modifier;
    }
}