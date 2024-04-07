using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Signals;

namespace Controllers
{
    public class BoaParkController : MonoBehaviour
    {
        [SerializeField] private Transform m_boat = null;
        [SerializeField] private Image m_timerImg = null;
        private float parkWaitTime = 1f;
        private float parkTimer = 0f;
        public bool canPark = false;

        #region collision
        private void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("boat"))
            {
                m_boat = col.transform;
                HandleTimerUI(true);
            }
        }

        private void OnTriggerExit(Collider col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("boat")&& !canPark)
            {
                Debug.Log("now we can park boat here!");
                m_boat = null;
                parkTimer = 0f;
                canPark = true;
                HandleTimerUI(false);
            }
        }

        private void OnTriggerStay(Collider col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("boat")&&canPark)
            {
                parkTimer += Time.deltaTime;
                if (parkTimer >= parkWaitTime)
                {
                    if (m_boat != null)
                    {
                        Debug.Log("parked successfully!");
                        //GameSignals.Instance.onHopDownFromBoat?.Invoke(col.transform);
                    }
                }
                if (m_timerImg != null)
                {
                    m_timerImg.fillAmount = parkTimer;
                }
            }
        }

        void HandlePark()
        {
            m_boat.position = transform.position;
            m_boat.rotation = transform.rotation;
        }

        void HandleTimerUI(bool tval)
        {
            if (m_timerImg != null)
            {
                m_timerImg.gameObject.SetActive(tval);
            }
        }
        #endregion
    }
}