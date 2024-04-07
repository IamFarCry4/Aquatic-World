using UnityEngine;
using UnityEngine.UI;
using Signals;
using Managers;

namespace Controllers
{
    public class BoatDeckController : MonoBehaviour
    {
        [SerializeField] private Image m_timerImg = null;
        private float hopWaitTime = 1f;
        private float hopTimer = 0f;
        [SerializeField] private Transform m_arrow = null;

        #region collision
        private void OnTriggerStay(Collider col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("player"))
            {
                HandleArrow(false);
                if (col.gameObject.GetComponent<PlayerManager>().isFishingStarted)
                {
                    return;
                }

                if (GameSignals.Instance.onGetPlayerBoatPass())
                {
                    hopTimer += Time.deltaTime;
                    if (hopTimer >= hopWaitTime)
                    {
                        hopTimer = 0f;
                        GameSignals.Instance.onUpdateFishingStatus?.Invoke(true);
                    }
                    UpdateTimerUI(hopTimer);
                }
            }
        }

        private void OnTriggerExit(Collider col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("player"))
            {
                hopTimer = 0f;
                UpdateTimerUI(hopTimer);
                HandleArrow(true);
            }
        }

        void UpdateTimerUI(float m_hopTime)
        {
            if (m_timerImg != null)
            {
                m_timerImg.fillAmount = m_hopTime;
            }
        }

        void HandleArrow(bool arrowValue)
        {
            if (m_arrow != null)
            {
                m_arrow.gameObject.SetActive(arrowValue);
            }
        }
        #endregion
    }
}
