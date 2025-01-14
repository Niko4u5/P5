﻿using UnityEngine;
using UnityEngine.Events;

namespace Utility.Events
{
    public class GameEventListener : MonoBehaviour
    {
        public GameEvent Event;
        public UnityEvent Response;

        private void OnEnable()
        {
            Event.Register(this);
        }

        private void OnDisable()
        {
            Event.Unregister(this);
        }

        public void OnEventRaised()
        {
            Response.Invoke();
        }
    }
}
