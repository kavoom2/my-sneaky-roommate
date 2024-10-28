using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;

namespace LittleDinoLini
{
    [CreateAssetMenu(menuName = "LittleDinoLini/Input/InputReader", fileName = "InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions
    {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2, bool> Look = delegate { };
        public event UnityAction EnableMouseControlCamera = delegate { };
        public event UnityAction DisableMouseControlCamera = delegate { };

        PlayerInputActions _inputActions;

        private void OnEnable()
        {
            if (_inputActions == null)
            {
                _inputActions = new PlayerInputActions();
                _inputActions.Player.SetCallbacks(this);
            }
        }

        public void EnablePlayerActions()
        {
            _inputActions.Enable();
        }

        public Vector3 Direction => (Vector3)_inputActions.Player.Move.ReadValue<Vector2>();

        public void OnFire(InputAction.CallbackContext context)
        {
            // noop
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            // noop
        }

        private bool IsDeviceMouse(InputAction.CallbackContext context)
        {
            return context.control.device.name.Contains("Mouse");
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
        }

        public void OnMouseControlCamera(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                {
                    EnableMouseControlCamera.Invoke();
                    break;
                }
                case InputActionPhase.Canceled:
                {
                    DisableMouseControlCamera.Invoke();
                    break;
                }
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Move.Invoke(context.ReadValue<Vector2>());
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            // noop
        }
    }
}
