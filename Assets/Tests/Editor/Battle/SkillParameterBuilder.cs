using CryptoQuest.Gameplay.Battle.Core;

namespace CryptoQuest.Tests.Editor.Battle
{
    public class SkillParameterBuilder
    {
        private float _basePower;
        private float _powerUpperLimit;
        private float _powerLowerLimit;
        private float _skillPowerThreshold;
        private float _powerValueAdded;
        private float _powerValueReduced;
        private int _continuesTurn;

        public SkillParameters Build()
        {
            return new SkillParameters()
            {
                BasePower = _basePower,
                PowerUpperLimit = _powerUpperLimit,
                PowerLowerLimit = _powerLowerLimit,
                SkillPowerThreshold = _skillPowerThreshold,
                PowerValueAdded = _powerValueAdded,
                PowerValueReduced = _powerValueReduced,
                ContinuesTurn = _continuesTurn
            };
        }

        public SkillParameterBuilder WithBasePower(float basePower)
        {
            _basePower = basePower;
            return this;
        }

        public SkillParameterBuilder WithPowerUpperLimit(float powerUpperLimit)
        {
            _powerUpperLimit = powerUpperLimit;
            return this;
        }

        public SkillParameterBuilder WithPowerLowerLimit(float powerLowerLimit)
        {
            _powerLowerLimit = powerLowerLimit;
            return this;
        }

        public SkillParameterBuilder WithSkillPowerThreshold(float skillPowerThreshold)
        {
            _skillPowerThreshold = skillPowerThreshold;
            return this;
        }

        public SkillParameterBuilder WithPowerValueAdded(float powerValueAdded)
        {
            _powerValueAdded = powerValueAdded;
            return this;
        }

        public SkillParameterBuilder WithPowerValueReduced(float powerValueReduced)
        {
            _powerValueReduced = powerValueReduced;
            return this;
        }

        public SkillParameterBuilder WithContinuesTurn(int continuesTurn)
        {
            _continuesTurn = continuesTurn;
            return this;
        }
    }
}