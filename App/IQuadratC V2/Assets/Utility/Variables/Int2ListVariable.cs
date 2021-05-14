﻿using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Utility.Variables
{
    [CreateAssetMenu(fileName = "Int2List", menuName = "Utility/Varibles/Int2List")] 
    public class Int2ListVariable : ScriptableObject, ISerializationCallbackReceiver
    {
        public List<int2> Value;
        public List<int2> InitialValue;
        
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            Value = InitialValue;
        }
        
        public void Set(List<int2> value)
        {
            Value = value;
        }
    }
}
