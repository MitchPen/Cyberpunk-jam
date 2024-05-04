using System;
using System.Collections.Generic;
using Main.Scripts.InputSystem;
using UnityEngine;
using Zenject;

namespace Main.Scripts.Input
{
    public class InputSystem : MonoBehaviour
    {
        [Inject] private InputConfig _inputConfig;
        public const string MouseXAxis = "Mouse X";
        public const string MouseYAxis = "Mouse Y";
        public const string Horizontal = "Horizontal";
        public const string Vertical = "Vertical";
        private Vector2 _keyboardAxis = Vector2.zero;
        private Vector2 _mouseAxis = Vector2.zero;
        private ButtonPair[] _mouseButton;
        private ButtonPair[] _keyboardButtons;
        private bool _enable;
        public Vector2 GetGeyboardAxisRaw() => _keyboardAxis;
        public Vector2 GetMouseAxisRaw() => _mouseAxis;

        private Dictionary<KeyEvents, List<Action>> _inputActionTable =
            new Dictionary<KeyEvents, List<Action>>();

        public void Enable()
        {
            _enable = true;
            _keyboardAxis = Vector2.zero;
            _mouseAxis = Vector2.zero;
            UnityEngine.Input.ResetInputAxes();
        }

        public void Disable()
        {
            _enable = false;
            _keyboardAxis = Vector2.zero;
            _mouseAxis = Vector2.zero;
            UnityEngine.Input.ResetInputAxes();
        }

        private void Awake()
        {
            _keyboardButtons = _inputConfig.KeyboardButtonPairs.ToArray();
        }

        private void Update()
        {
            if (_enable == false) return;
            HandleMouse();
            HandleKeyboard();
        }

        private void HandleKeyboard()
        {
            if (_enable == false) return;
            _keyboardAxis.x = UnityEngine.Input.GetAxisRaw(Horizontal);
            _keyboardAxis.y = UnityEngine.Input.GetAxisRaw(Vertical);
            HandleButtonInput();
        }
        
        private void HandleMouse()
        {
            if (_enable == false) return;
            _mouseAxis.x = UnityEngine.Input.GetAxisRaw(MouseXAxis);
            _mouseAxis.y = UnityEngine.Input.GetAxisRaw(MouseYAxis);
            _mouseAxis *= _inputConfig.MouseSensitivity;
            
            switch (UnityEngine.Input.mouseScrollDelta.y)
            {
                case < 0f:
                    CastAction(KeyEvents.M_SCROLL_UP);
                    break;
                case > 0f:
                    CastAction(KeyEvents.M_SCROLL_DOWN);
                    break;
            }
        }
        
        private void HandleButtonInput()
        {
            void CheckKeyInput(ButtonPair[] buttonPairs)
            {
                foreach (var buttonPair in buttonPairs)
                {
                    switch (buttonPair.InteractionType)
                    {
                        case InputInteractionType.DOWN:
                        {
                            if (HandleKeyDown(buttonPair.KeyType))
                                CastAction(buttonPair.EventType);
                            break;
                        }

                        case InputInteractionType.UP:
                        {
                            if (HandleKeyUp(buttonPair.KeyType))
                                CastAction(buttonPair.EventType);
                            break;
                        }
                        
                        case InputInteractionType.HOLD:
                        {
                            if (HandleKeyHold(buttonPair.KeyType))
                                CastAction(buttonPair.EventType);
                            break;
                        }
                    }
                }
            }

            if (_mouseButton != null)
                CheckKeyInput(_mouseButton);
            if (_keyboardButtons != null)
                CheckKeyInput(_keyboardButtons);
        }
        
        private bool HandleKeyDown(KeyCode key) => UnityEngine.Input.GetKeyDown(key);
        private bool HandleKeyUp(KeyCode key) => UnityEngine.Input.GetKeyUp(key);
        private bool HandleKeyHold(KeyCode key) => UnityEngine.Input.GetKey(key);

        private void CastAction(KeyEvents type)
        {
            if (_inputActionTable.ContainsKey(type))
                _inputActionTable[type].ForEach(action => action?.Invoke());
        }

        public void ClearEventHandlerOn(KeyEvents type)
        {
            if (_inputActionTable.ContainsKey(type))
                _inputActionTable.Remove(type);
        }

        public void SubscribeOnInputEvent(KeyEvents type, Action action)
        {
            if (_inputActionTable.ContainsKey(type))
            {
                if (!_inputActionTable[type].Contains(action))
                    _inputActionTable[type].Add(action);
            }

            else
            {
                _inputActionTable.Add(type, new List<Action>());
                _inputActionTable[type].Add(action);
            }
        }

        public void UnsubscribeFromInputEvent(KeyEvents type, Action action)
        {
            if (!_inputActionTable.ContainsKey(type)) return;

            if (_inputActionTable[type].Contains(action))
                _inputActionTable[type].Remove(action);
        }
    }
}