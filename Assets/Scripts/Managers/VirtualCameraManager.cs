using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Signals;
using Enums;

namespace Managers
{
    public class VirtualCameraManager : MonoBehaviour
    {
       [SerializeField]private CinemachineStateDrivenCamera stateDrivenCamera;

        private Animator m_animator;

        [System.Serializable]
        public class CamInfo
        {
            public Vector3 camPos;
            public Quaternion camRot;
        }
        private List<CamInfo> canInfoList = new List<CamInfo>();


        private void Awake()
        {
            m_animator = GetComponent<Animator>();
            GetInitialPositions();
        }
        #region Event Subscriptions
        private void OnEnable()
       {
           SubscribeEvents();
       }

       private void OnDisable()
       {
           UnsubscribeEvents();
       }

       private void SubscribeEvents()
       {
            GameSignals.Instance.onChangeGameState += OnChangeGameState;
            GameSignals.Instance.onChangeCameraTarget += OnChangeCameraTarget;
            GameSignals.Instance.onChangeCameraLookAt += OnChangeCameraLookAt;
       }

       private void UnsubscribeEvents()
       {
            GameSignals.Instance.onChangeGameState -= OnChangeGameState;
            GameSignals.Instance.onChangeCameraTarget -= OnChangeCameraTarget;
            GameSignals.Instance.onChangeCameraLookAt -= OnChangeCameraLookAt;
        }
        #endregion

        //for changing camera target player to boat and vice versa
        void OnChangeCameraTarget(Transform m_camTarget)
        {
            stateDrivenCamera.Follow = m_camTarget;
        }

        void OnChangeCameraLookAt(Transform lookTarget)
        {
            stateDrivenCamera.LookAt = lookTarget;
        }

        public void OnChangeGameState(GameStates states)
        {
            m_animator.SetTrigger(states.ToString());
            if(states!=GameStates.attack)
            {
                stateDrivenCamera.LookAt = null;
                SetInitialPositions();
            }
        }

        void GetInitialPositions()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                CamInfo info = new CamInfo();
                info.camPos = transform.GetChild(i).localPosition;
                info.camRot = transform.GetChild(i).localRotation;
                canInfoList.Add(info);
            }
        }

        void SetInitialPositions()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).localPosition = canInfoList[i].camPos;
                transform.GetChild(i).localRotation = canInfoList[i].camRot;
            }
        }
    }
}
