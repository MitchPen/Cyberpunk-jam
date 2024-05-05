using System;
using System.Collections.Generic;
using Main.Scripts.InputSystem;
using UnityEngine;

namespace Main.Scripts.Input
{
    [CreateAssetMenu(fileName = "Input_CFG", menuName = "Configuration/Create new InputConfig", order = 1)]
    public class InputConfig : ScriptableObject
    {
        [Range(1f, 5f)] public float MouseSensitivity;
        public List<ButtonPair> KeyboardButtonPairs = new List<ButtonPair>();
    }

    [Serializable]
    public struct ButtonPair
    {
        public InputInteractionType InteractionType;
        public KeyEvents EventType;
        public KeyCode KeyType;
    }
    
    public enum InputInteractionType
    {
        UP,
        DOWN,
        HOLD
    }
}