using System;
using System.Collections;
using Cinemachine;
using KBCore.Refs;
using UnityEngine;

namespace LittleDinoLini
{
    public class CameraManager : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Anywhere]
        InputReader _input;

        [SerializeField, Anywhere]
        CinemachineFreeLook _freeLookCamera;

        [Header("Settings")]
        [SerializeField, Range(0.5f, 3f)]
        float _speedMultiplier = 1f;

        // 오른쪽 마우스 버튼이 눌려 있는지 여부를 나타내는 변수입니다.
        bool _isRMBPressed;

        bool _cameraMovementLock;

        void OnEnable()
        {
            _input.Look += OnLook;
            _input.EnableMouseControlCamera += OnEnableMouseControlCamera;
            _input.DisableMouseControlCamera += OnDisableMouseControlCamera;
        }

        void OnDisable()
        {
            _input.Look -= OnLook;
            _input.EnableMouseControlCamera -= OnEnableMouseControlCamera;
            _input.DisableMouseControlCamera -= OnDisableMouseControlCamera;
        }

        void Start() => _input.EnablePlayerActions();

        void OnLook(Vector2 cameraMovement, bool isDeviceMouse)
        {
            if (_cameraMovementLock)
                return;

            if (isDeviceMouse && !_isRMBPressed)
                return;

            // 디바이스 마우스를 사용하는 경우, 고정 시간 간격(Time.fixedDeltaTime)을 사용합니다.
            // 그렇지 않으면, 델타 시간 간격(Time.deltaTime)을 사용합니다.
            // 이렇게 하는 이유는 마우스 입력이 일반적으로 고정된 시간 간격으로 발생하는 반면,
            // 다른 입력은 프레임 속도에 따라 달라질 수 있기 때문입니다.
            // 고정 시간 간격을 사용하면 마우스 입력의 처리가 더 안정적이고 예측 가능해집니다.
            float deviceMultiplier = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime;

            // 카메라의 X,Y축 입력 값을 계산합니다. 카메라 이동, 속도 배수, 디바이스 배수를 곱합니다.
            //
            // inputAxisValue는 카메라의 X축과 Y축 입력 값을 나타내는 변수입니다.
            // 이 값은 카메라의 이동 속도와 방향을 결정하는 데 사용됩니다.
            // X축과 Y축 입력 값은 각각 카메라의 좌우 이동과 상하 이동을 제어합니다.
            _freeLookCamera.m_XAxis.m_InputAxisValue =
                cameraMovement.x * _speedMultiplier * deviceMultiplier;
            _freeLookCamera.m_YAxis.m_InputAxisValue =
                cameraMovement.y * _speedMultiplier * deviceMultiplier;
        }

        void OnEnableMouseControlCamera()
        {
            _isRMBPressed = true;

            // 커서를 중앙에 고정하고 화면에 보이지 않게 합니다.
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            StartCoroutine(DisableMouseForFrame());
        }

        void OnDisableMouseControlCamera()
        {
            _isRMBPressed = false;

            // 커서를 잠금 해제하고 화면에 표시합니다.
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // 마우스 제어를 잠시 해제한 후 다시 잠금할 때,
            // 카메라의 X축과 Y축 입력 값을 0으로 초기화하여
            // 카메라의 이동을 초기화하고, 재활성화 시 카메라의 갑작스러운 이동을 방지합니다.
            _freeLookCamera.m_XAxis.m_InputAxisValue = 0f;
            _freeLookCamera.m_YAxis.m_InputAxisValue = 0f;
        }

        // DisableMouseForFrame는 마우스 제어를 잠시 해제하는 코루틴입니다.
        // 이 코루틴은 마우스 제어를 잠시 중단하고, 이 프레임이 끝날 때까지 기다린 후에 다시 활성화합니다.
        IEnumerator DisableMouseForFrame()
        {
            _cameraMovementLock = true;

            // 이 프레임이 끝날 때까지 기다립니다.
            // WaitForEndOfFrame는 이 프레임이 끝날 때까지 코루틴을 일시 중단하는 유니티의 코루틴입니다.
            yield return new WaitForEndOfFrame();

            _cameraMovementLock = false;
        }
    }
}
