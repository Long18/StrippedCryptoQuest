using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CryptoQuest
{
    [CreateAssetMenu(menuName = "Core/Events/String Event Channel")]
    public class StringEventChannelSO : ScriptableObject
    {
        public UnityAction<string> EventRaised;

        public void RaiseEvent(string value)
        {
            OnRaiseEvent(value);
        }

        private void OnRaiseEvent(string value)
        {
            if (EventRaised == null)
            {
                Debug.LogWarning($"Event was raised on {name} but no one was listening.");
                return;
            }

            EventRaised.Invoke(value);
        }
    }
}