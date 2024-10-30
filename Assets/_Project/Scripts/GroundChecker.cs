using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;

namespace LittleDinoLini
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField]
        float _groundDistance = 0.08f;

        [SerializeField]
        LayerMask _groundLayers;

        public bool IsGrounded { get; private set; }

        void Update()
        {
            IsGrounded = Physics.SphereCast(
                transform.position,
                _groundDistance,
                Vector3.down,
                out _,
                _groundDistance,
                _groundLayers
            );
        }
    }
}
