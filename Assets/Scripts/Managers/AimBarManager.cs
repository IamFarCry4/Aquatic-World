using UnityEngine;
using Signals;

namespace Managers
{
    public class AimBarManager : MonoBehaviour
    {
        [SerializeField] private RectTransform aimIndicator;
        private float m_speed=150f;
        [SerializeField]private float min_anchoredPos =-90f;
        [SerializeField]private float max_anchoredPos = 90f;
        private float cur_anchoredPos = 0f;

        [Header("Correct Aim")]
        [SerializeField] private float m_minCorrect=-24f;
        [SerializeField] private float m_maxCorrect=24f;
        private int aim_value = 0;
        private bool applyFireCooldown = false;

        #region Event Subscriptions
        private void OnEnable()
        {
            SubscribeEvents();
            OnResetAimBar();
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            WeaponSignals.Instance.onResetAimBar += OnResetAimBar;
        }

        private void UnsubscribeEvents()
        {
            WeaponSignals.Instance.onResetAimBar -= OnResetAimBar;
        }
        #endregion

        private void Update()
        {
            if(!applyFireCooldown)
            {
                m_speed = (cur_anchoredPos >= max_anchoredPos || cur_anchoredPos <= min_anchoredPos) ? -m_speed : m_speed;
                cur_anchoredPos = Mathf.Clamp(cur_anchoredPos + m_speed * Time.deltaTime, min_anchoredPos, max_anchoredPos);
                aimIndicator.anchoredPosition = new Vector3(cur_anchoredPos, aimIndicator.anchoredPosition.y, 0f);
                if (Input.GetMouseButtonDown(0))
                {
                    FireProjectile(cur_anchoredPos);
                }
            }
        }

        void FireProjectile(float aimValue)
        {
            m_speed = 0f;
            aim_value = (aimValue >= m_minCorrect && aimValue <= m_maxCorrect) ? 1 : 0;
            WeaponSignals.Instance.onFireProjectile?.Invoke(aim_value);
            applyFireCooldown = true;
        }

        void OnResetAimBar()
        {
            m_speed = 150f;
            aim_value = 0;
            applyFireCooldown = false;
        }
    }
}
