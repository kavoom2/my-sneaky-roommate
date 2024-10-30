using DG.Tweening;
using UnityEngine;

namespace LittleDinoLini
{
    [RequireComponent(typeof(AudioSource))]
    public class SpawnEffects : MonoBehaviour
    {
        [SerializeField]
        GameObject _spawnVFX;

        [SerializeField]
        float _animationDuration;

        void Start()
        {
            // 초기 크기를 0으로 지정하여 보이지 않도록 합니다.
            transform.localScale = Vector3.zero;

            // 애니메이션 시간 동안 크기를 1로 증가시키고, 약간 튀어오르는 효과를 줍니다.
            transform.DOScale(Vector3.one, _animationDuration).SetEase(Ease.OutBack);

            if (_spawnVFX != null)
            {
                Instantiate(_spawnVFX, transform.position, Quaternion.identity);
            }

            GetComponent<AudioSource>().Play();
        }
    }
}
