using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CryptoQuest.Audio.AudioData
{
    [Serializable]
    public class AudioClipsGroup
    {
        [SerializeField] private ESequenceMode _mode = ESequenceMode.Sequential;
        [SerializeField] private AssetReferenceT<AudioClip>[] _audioClips;
        private IListIndex _clipIndex;

        private IListIndex ClipIndex
        {
            get
            {
                _clipIndex ??= ListIndexFactory.Create(_mode);
                return _clipIndex;
            }
        }

        public AudioClipsGroup() { }

        public AudioClipsGroup(AssetReferenceT<AudioClip>[] audioClips, ESequenceMode mode)
        {
            _audioClips = audioClips;
            _mode = mode;
        }

        public AssetReferenceT<AudioClip> CurrentClip => _audioClips[ClipIndex.Value];

        public  AssetReferenceT<AudioClip> SwitchToNextClip()
        {
            ClipIndex.GoForward(_audioClips.Length);
            return _audioClips[ClipIndex.Value];
        }
    }
}