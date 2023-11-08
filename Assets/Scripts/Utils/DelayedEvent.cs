using System;
using UnityEngine;
using UnityEngine.Events;

namespace Utils
{
    public class DelayedEvent : MonoBehaviour
    {
        [TextArea] [SerializeField] private string description;
        [SerializeField] private UnityEvent action;

        private bool started;
        private float delay;
        private float timeCounter;

        public void InvokeDelayed(float delaySeconds)
        {
            delay = delaySeconds;
            started = true;
            timeCounter = 0;
        }

        private void Update()
        {
            if (!started) return;

            if (timeCounter >= delay)
            {
                action.Invoke();
                started = false;
                timeCounter = 0;
            }

            timeCounter += Time.deltaTime;
        }
    }
}
