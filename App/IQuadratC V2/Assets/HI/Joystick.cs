﻿using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility.Events;
using Utility.Variables;

namespace HI
{
    public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
    
        [SerializeField] private Transform stick;
        [SerializeField] private Vec2Variable direction;
        [SerializeField] private float maxDistance;
        [SerializeField] private StringVariable logMessage;
        [SerializeField] private GameEvent logEvent;
    
        private bool pressed;
        private float3 lastPos;
        private float2 fingerPos;
    
        public void OnPointerDown(PointerEventData eventData){
            lastPos = Input.mousePosition;
            pressed = true;
            logMessage.Value = "Graped Joystick";
            logEvent.Raise();
        }
     
        public void OnPointerUp(PointerEventData eventData){
            pressed = false;
            logMessage.Value = "Released Joystick";
            logEvent.Raise();
        }

        public void Update()
        {
            if (pressed)
            {
                // calculate finger position
                if (Input.touchCount > 0)
                {
                    fingerPos += (float2)Input.touches[0].deltaPosition;
                }
                else
                {
                    float3 pos = Input.mousePosition;
                    fingerPos += (pos.xy - lastPos.xy);;
                    lastPos = pos;
                }


                // format point to maxDistance 
                if (math.length(fingerPos.xy) > maxDistance)
                {
                    direction.Value = math.normalize(fingerPos);
                    stick.localPosition = new float3(math.normalize(fingerPos), 0) * maxDistance;
                }
                else
                {
                    direction.Value = (fingerPos) / maxDistance;
                    stick.localPosition = new float3(fingerPos, 0);
                }
            }
            else
            {
                // resets the stick position
                direction.Value = float2.zero;
                stick.localPosition = Vector3.zero;
                fingerPos = float2.zero;
            }
        }
    }
}
