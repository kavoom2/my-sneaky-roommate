using System;
using System.Collections.Generic;
using Cinemachine;
using KBCore.Refs;
using UnityEngine;
using Utilities;

namespace LittleDinoLini
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self]
        Rigidbody _rb;

        [SerializeField, Self]
        GroundChecker _groundChecker;

        [SerializeField, Self]
        Animator _animator;

        [SerializeField, Anywhere]
        CinemachineFreeLook _freeLookCamera;

        [SerializeField, Anywhere]
        InputReader _input;

        [Header("Movement Settings")]
        [SerializeField]
        float _moveSpeed = 300f;

        [SerializeField]
        float _rotationSpeed = 450f;

        [SerializeField]
        float _smoothTime = 0.2f;

        [Header("Jump Settings")]
        [SerializeField]
        float _jumpForce = 10f;

        [SerializeField]
        float _jumpCooldown = 0.5f;

        [SerializeField]
        float _jumpDuration = 0.5f;

        [SerializeField]
        float _jumpMaxHeight = 2f;

        [SerializeField]
        float _gravityMultiplier = 3f;

        Transform _mainCam;

        const float ZeroF = 0f;

        float _speed;
        float _velocity;
        float _jumpVelocity;

        Vector3 _movement;

        List<Timer> _timers;
        CountdownTimer _jumpTimer;
        CountdownTimer _jumpCooldownTimer;

        static readonly int Speed = Animator.StringToHash("Speed");

        void Awake()
        {
            // 메인 카메라의 위치를 가져와서 _mainCam에 저장합니다.
            _mainCam = Camera.main.transform;
            // _freeLookCamera의 Follow와 LookAt을 현재 플레이어 캐릭터로 설정합니다.
            _freeLookCamera.Follow = transform;
            _freeLookCamera.LookAt = transform;

            // _freeLookCamera의 OnTargetObjectWarped 이벤트에 현재 플레이어 캐릭터와 카메라 사이의 거리를 설정합니다.
            //
            // OnTargetObjectWarped 이벤트는 카메라가 목표물 주위를 돌 때 호출됩니다.
            // 이 이벤트에 현재 플레이어 캐릭터와 카메라 사이의 거리를 설정합니다.
            // 이를 통해 카메라가 플레이어 캐릭터 주위를 원활하게 돌 수 있습니다.
            // Vector3.forward는 카메라가 플레이어 캐릭터 앞쪽에서 시작하여 주위를 돌게 합니다.
            // 이 값은 카메라가 플레이어 캐릭터를 중심으로 돌 때의 초기 위치를 결정합니다.
            _freeLookCamera.OnTargetObjectWarped(
                transform,
                transform.position - _freeLookCamera.transform.position - Vector3.forward
            );

            _rb.freezeRotation = true;

            // 타이머를 초기화합니다.
            _jumpTimer = new CountdownTimer(_jumpDuration);
            _jumpCooldownTimer = new CountdownTimer(_jumpCooldown);
            _timers = new List<Timer>(2) { _jumpTimer, _jumpCooldownTimer };

            _jumpTimer.OnTimerStop += () => _jumpCooldownTimer.Start();
        }

        void Start()
        {
            _input.EnablePlayerActions();
        }

        void OnEnable()
        {
            _input.Jump += OnJump;
        }

        void OnDisable()
        {
            _input.Jump -= OnJump;
        }

        void OnJump(bool performed)
        {
            if (
                performed
                && !_jumpTimer.IsRunning
                && !_jumpCooldownTimer.IsRunning
                && _groundChecker.IsGrounded
            )
            {
                _jumpTimer.Start();
            }
            else if (!performed && _jumpTimer.IsRunning)
            {
                _jumpTimer.Stop();
            }
        }

        void Update()
        {
            _movement = new Vector3(_input.Direction.x, ZeroF, _input.Direction.y);

            HandleTimer();
            UpdateAnimator();
        }

        void FixedUpdate()
        {
            // 이 함수는 플레이어 캐릭터의 이동을 처리하며, FixedUpdate에서 호출해야 하는 이유는 물리 엔진과 관련된 처리를 하기 때문입니다.
            // FixedUpdate는 물리 엔진의 업데이트 주기에 맞춰 호출되기 때문에, 이동 처리를 여기서 하게 되면 물리 엔진과 동기화된 이동이 가능합니다. (ex. 0.02초 마다 호출)
            HandleJump();
            HandleMovement();
        }

        void UpdateAnimator()
        {
            _animator.SetFloat(Speed, _speed);
        }

        void HandleTimer()
        {
            foreach (var timer in _timers)
            {
                timer.Tick(Time.deltaTime);
            }
        }

        void HandleJump()
        {
            // Jump 중이지 않고 땅에 붙어있는 경우 -- Jump Velocity를 0으로 초기화합니다.
            if (!_jumpTimer.IsRunning && _groundChecker.IsGrounded)
            {
                _jumpVelocity = ZeroF;
                _jumpTimer.Stop();
                return;
            }

            // Jumping OR Falling 상태인 경우
            if (_jumpTimer.IsRunning)
            {
                float launchPoint = 0.9f;
                // 초기 상승하는 구간까지의 지점 [1, 0.9]
                if (_jumpTimer.Progress > launchPoint)
                {
                    // 자유낙하 운동 공식: `g = 2 * h / t^2` 에서 `t = sqrt(2 * h / g)` 계산
                    _jumpVelocity = Mathf.Sqrt(2 * _jumpMaxHeight * Mathf.Abs(Physics.gravity.y));
                }
                // 초기 상승하는 구간이 아닌 경우 [0.9, 0] -- 점프가 진행될 수록 중력의 영향을 받아 가속도가 줄어듭니다.
                else
                {
                    _jumpVelocity += (1 - _jumpTimer.Progress) * _jumpForce * Time.fixedDeltaTime;
                }
            }
            // Falling 상태인 경우 -- 중력의 영향을 받아 속도가 점차 줄어 낙하하게 됩니다.
            else
            {
                _jumpVelocity += Physics.gravity.y * _gravityMultiplier * Time.fixedDeltaTime;
            }

            // 최종 속도를 적용합니다.
            _rb.velocity = new Vector3(_rb.velocity.x, _jumpVelocity, _rb.velocity.z);
        }

        void HandleMovement()
        {
            // 메인 카메라의 y축 각도를 사용하여 방향을 조정합니다.
            //
            // Quaternion.AngleAxis를 사용하여 메인 카메라의 y축 각도만큼 movementDirection을 회전시킵니다.
            // Vector3.up은 y축 방향을 나타내는 단위 벡터입니다.
            // _mainCam.eulerAngles.y는 메인 카메라의 y축 각도를 나타내는 값입니다.
            // 이 함수는 movementDirection을 _mainCam.eulerAngles.y만큼 Vector3.up 방향으로 회전시킨 결과를 반환합니다.
            var adjustedDirection =
                Quaternion.AngleAxis(_mainCam.eulerAngles.y, Vector3.up) * _movement;

            // 이동 방향의 크기가 0보다 큰 경우, 즉 이동 방향이 있는 경우
            if (adjustedDirection.magnitude > ZeroF)
            {
                HandleRotation(adjustedDirection);
                HandleHorizontalMovement(adjustedDirection);
                SmoothSpeed(adjustedDirection.magnitude);
            }
            else
            {
                // 이동 방향이 없는 경우, 즉 정지 상태인 경우 속도를 0으로 설정합니다.
                SmoothSpeed(ZeroF);

                // Rigidbody의 속도를 0으로 설정하여 캐릭터가 정지하도록 합니다.
                _rb.velocity = new Vector3(ZeroF, _rb.velocity.y, ZeroF);
            }
        }

        /// <summary>
        /// 조정된 방향으로 캐릭터 컨트롤러를 이동시킵니다.
        /// </summary>
        void HandleHorizontalMovement(Vector3 adjustedDirection)
        {
            Vector3 velocity = (adjustedDirection * Time.fixedDeltaTime) * _moveSpeed;
            _rb.velocity = new Vector3(velocity.x, _rb.velocity.y, velocity.z);
        }

        /// <summary>
        /// 조정된 방향으로 캐릭터의 회전을 처리합니다.
        /// </summary>
        void HandleRotation(Vector3 adjustedDirection)
        {
            // 조정된 방향을 기준으로 회전을 계산합니다.
            var targetRotation = Quaternion.LookRotation(adjustedDirection);

            // 현재 회전에서 목표 회전으로 부드럽게 회전합니다.
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                _rotationSpeed * Time.deltaTime
            );
        }

        /// <summary>
        /// 목표 속도까지 부드럽게 속도를 조정합니다.
        /// </summary>
        void SmoothSpeed(float targetSpeed)
        {
            // 현재 속도에서 목표 속도로 부드럽게 변경합니다.
            _speed = Mathf.SmoothDamp(_speed, targetSpeed, ref _velocity, _smoothTime);
        }
    }
}
