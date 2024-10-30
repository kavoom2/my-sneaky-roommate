using UnityEngine;

namespace LittleDinoLini
{
    public class PlatformCollisionHandler : MonoBehaviour
    {
        Transform _platform;

        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("MovingPlatform"))
            {
                // 충돌 면의 법선 벡터의 y 성분이 0.5 미만이면, 해당 충돌이 플랫폼 위에 올라가 발생한 것이 아니라고 판단합니다.
                ContactPoint contact = other.GetContact(0);
                if (contact.normal.y < 0.5f)
                    return;

                _platform = other.transform;
                transform.SetParent(_platform);
            }
        }

        void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("MovingPlatform"))
            {
                _platform = null;
                transform.SetParent(null);
            }
        }
    }
}
