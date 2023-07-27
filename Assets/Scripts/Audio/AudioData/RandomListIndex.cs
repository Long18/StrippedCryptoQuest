using UnityEngine;

namespace CryptoQuest.Audio.AudioData
{
    public class RandomListIndex : IListIndex
    {
        public int Value { get; private set; } = 0;

        public IListIndex GoForward(int elementCount)
        {
            Value = GenerateRandomIndex(elementCount);
            return this;
        }

        public IListIndex GoBackward(int elementCount)
        {
            Value = GenerateRandomIndex(elementCount);
            return this;
        }

        protected virtual int GenerateRandomIndex(int elementCount) => Random.Range(0, elementCount);
    }
}