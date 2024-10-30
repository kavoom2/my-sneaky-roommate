using DG.Tweening;
using UnityEngine;

namespace LittleDinoLini
{
    public class PlatformMover : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _moveTo;

        [SerializeField]
        private float _moveTime;

        [SerializeField]
        private Ease _ease = Ease.InOutQuad;

        Vector3 _startPosition;

        void Start()
        {
            _startPosition = transform.position;
            Move();
        }

        void Move()
        {
            transform
                .DOMove(_startPosition + _moveTo, _moveTime)
                .SetEase(_ease)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}
