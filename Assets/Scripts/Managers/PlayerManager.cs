using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Signals;
using Enums;
using DG.Tweening;

namespace Managers
{
    public class PlayerManager : MonoBehaviour
    {
        private Rigidbody m_playerRb;
        //[SerializeField] private Transform m_player;
        [Header("Character")]
        [SerializeField] private float char_moveSpeed;
        [SerializeField] private float char_turnSpeed;
        private Vector3 m_dirVec;

        //[Header("Boat")]
        //[SerializeField] private float boat_moveSpeed;
        //[SerializeField] private float boat_turnSpeed;
        //private float cur_moveSpeed;
        //private float cur_turnSpeed;

        //[HideInInspector]
        public bool isFishingStarted = false;//disable movement while fishing
        [SerializeField]private Transform baitPlacer = null;
        private Vector3 baitInitialPos = Vector3.zero;
        private Vector3 lastBaitPos;
        [SerializeField] private float baitThrowRadius = 2f;
        [SerializeField] private GameObject placeBaitUI;
        private bool isBaitPlaced = false;
      
        private Animator m_animator;
        private int runId = Animator.StringToHash("run");
        private int aimID= Animator.StringToHash("aim");
        private int fireID = Animator.StringToHash("fire");


        [Header("Bait")]
        [SerializeField] private Transform baitPrefab = null;
        private List<Vector3> trajectoryPoints = new List<Vector3>();
        private float h = 2.5f;
        private float gravity = -10f;
        private int m_baitIndex = 0;
        private Transform m_bait = null;

        private void Start()
        {
            m_playerRb = GetComponent<Rigidbody>();
            m_animator = transform.GetChild(0).GetComponent<Animator>();
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
            //GameSignals.Instance.onUpdatePlayer += OnUpdatePlayer;
            InputSignals.Instance.onUpdateDirectionalVector += OnUpdateDirectionalVector;
            GameSignals.Instance.onUpdateFishingStatus += OnUpdateFishingStatus;
            //GameSignals.Instance.onMissedWeaponShot += OnMissedWeaponShot;
            GameSignals.Instance.onHandlePlayerBet += OnHandlePlayerBet;
            GameSignals.Instance.onBaitCatched += OnBaitCatched;
        }

        private void UnsubscribeEvents()
        {
            //GameSignals.Instance.onUpdatePlayer -= OnUpdatePlayer;
            InputSignals.Instance.onUpdateDirectionalVector -= OnUpdateDirectionalVector;
            GameSignals.Instance.onUpdateFishingStatus -= OnUpdateFishingStatus;
            //GameSignals.Instance.onMissedWeaponShot -= OnMissedWeaponShot;
            GameSignals.Instance.onHandlePlayerBet -= OnHandlePlayerBet;
            GameSignals.Instance.onBaitCatched -= OnBaitCatched;
        }
        #endregion

        void OnUpdateFishingStatus(bool fishingVal)
        {
            isFishingStarted = fishingVal;
            baitInitialPos = baitPlacer.position;
        }

        void OnHandlePlayerBet(bool val)
        {
            isBaitPlaced = val;
        }

        /*
        void OnUpdatePlayer(Transform cur_player)
        {
            m_player = cur_player;
            if(cur_player.gameObject.layer==LayerMask.NameToLayer("player"))
            {
                cur_moveSpeed= char_moveSpeed;
                cur_turnSpeed= char_turnSpeed;
            }
            else if (cur_player.gameObject.layer == LayerMask.NameToLayer("boat"))
            {
                cur_moveSpeed = boat_moveSpeed;
                cur_turnSpeed = boat_turnSpeed;
            }
        }
        */

        void OnUpdateDirectionalVector(Vector3 dirVector)
        {
            m_dirVec = dirVector;
        }

        private void Update()
        {
            //disabling boat control code
            //if (m_player == null) return;
            //if (m_dirVec != Vector3.zero&&GameSignals.Instance.onGetGameState()==Enums.GameStates.ingame)
            //{
            //    Quaternion toRotation = Quaternion.LookRotation(m_dirVec, Vector3.up);
            //    m_player.rotation = Quaternion.Lerp(m_player.rotation, toRotation, cur_turnSpeed * Time.deltaTime);
            //    m_player.position += m_player.forward * cur_moveSpeed * Time.deltaTime;
            //    //m_player.GetComponent<Rigidbody>().MovePosition(m_player.forward * cur_moveSpeed * Time.deltaTime);
            //}

            if(GameSignals.Instance.onGetGameState() == GameStates.ingame)
            {
                if(!isFishingStarted)
                {
                    if(m_dirVec != Vector3.zero)
                    {
                        Quaternion toRotation = Quaternion.LookRotation(m_dirVec.normalized, Vector3.up);
                        transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, toRotation, char_turnSpeed * Time.deltaTime);
                        m_animator.SetBool(runId, true);
                    }
                    else
                    {
                        m_animator.SetBool(runId, false);
                    }
                }
                else
                {
                    m_animator.SetBool(runId, false);
                    if (!baitPlacer.gameObject.activeInHierarchy)
                    {
                        baitPlacer.gameObject.SetActive(true);
                    }

                    if (m_dirVec.magnitude > 0f)
                    {
                        Vector3 diff = (lastBaitPos + m_dirVec*2f) - baitInitialPos;
                        Vector3 m_baitTarget = baitInitialPos + Vector3.ClampMagnitude(diff, baitThrowRadius);
                        baitPlacer.position = Vector3.Lerp(baitPlacer.position, m_baitTarget, Time.deltaTime * 5f);
                        lastBaitPos = baitPlacer.position;
                        placeBaitUI.SetActive(false);
                        isBaitTargetUpdated = true;
                    }
                    else
                    {
                        //activate bait ui from here
                        if(!isBaitPlaced)
                        {
                            placeBaitUI.SetActive(true);
                        }
                    }

                    //lets lock bait place when finger is lifted from screen
                    //lets throw bait once bait placer is moved
                    if (isBaitTargetUpdated&&Input.GetMouseButtonUp(0)&& !isBaitPlaced)
                    {
                        if(m_lastBaitPos!= baitPlacer.position)
                        {
                            OnThrowBait();
                            m_lastBaitPos = baitPlacer.position;
                            isBaitTargetUpdated = false;
                        }
                    }
                }
            }

            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    OnThrowBait();
            //}
        }

        private Vector3 m_lastBaitPos = Vector3.zero;
        private bool isBaitTargetUpdated = false;

        private void FixedUpdate()
        {
            if (GameSignals.Instance.onGetGameState() == GameStates.ingame)
            {
                if (!isFishingStarted)
                {
                    if (m_dirVec != Vector3.zero)
                    {
                        m_playerRb.MovePosition(m_playerRb.position + transform.TransformDirection(m_dirVec.normalized) * Time.fixedDeltaTime * char_moveSpeed);
                    }
                }
            }
        }

        void OnThrowBait()
        {
            m_bait = Instantiate(baitPrefab, transform.position,Quaternion.identity);
            m_bait.gameObject.SetActive(true);
            //
            m_baitIndex = 0;
            trajectoryPoints.Clear();
            Vector3 baitPos = new Vector3(baitPlacer.position.x,-0.6f,baitPlacer.position.z);
            LunchData lunchData = CalculateLunchData(baitPos);
            int resolution = 20;
            for (int i = 1; i < resolution; i++)
            {
                float simulationTime = i / (float)resolution * lunchData.timeToTarget;
                Vector3 displacement = lunchData.initialVelocity * simulationTime + Vector3.up * gravity * simulationTime * simulationTime / 2f;
                Vector3 drawPoint = transform.position + displacement;
                trajectoryPoints.Add(drawPoint);
            }
            MoveBaitOnTrajectory();
        }

        LunchData CalculateLunchData(Vector3 targetPos)
        {
            float displacementY = targetPos.y - transform.position.y;
            Vector3 displacementXZ = new Vector3(targetPos.x - transform.position.x,
            0, targetPos.z - transform.position.z);
            float time = Mathf.Sqrt(-2f * h / gravity) + Mathf.Sqrt(2f * (displacementY - h) / gravity);
            Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * h);
            Vector3 velocityXZ = displacementXZ / time;
            return new LunchData(velocityXZ + velocityY, time);
        }

        void MoveBaitOnTrajectory()
        {
            Sequence baitSeq = DOTween.Sequence();
            baitSeq.Append(m_bait.DOMove(trajectoryPoints[m_baitIndex], 0.02f).SetEase(Ease.Linear));
            baitSeq.Join(m_bait.DOLookAt(trajectoryPoints[m_baitIndex], 0.02f).SetEase(Ease.Linear));
            baitSeq.OnComplete(() =>
            {
                if (m_baitIndex < trajectoryPoints.Count - 1)
                {
                    m_baitIndex++;
                    MoveBaitOnTrajectory();
                }
                else
                {
                    StartCoroutine(FinalizeBaitPos());
                }
            });
        }

        //disable active bait
        void OnBaitCatched()
        {
            if(m_bait!=null)
            {
                m_bait.gameObject.SetActive(false);
            }
        }


        IEnumerator FinalizeBaitPos()
        {
            yield return new WaitForSeconds(0.5f);
            GameSignals.Instance.onFishBaitPlaced?.Invoke(baitPlacer.position);
            placeBaitUI.SetActive(false);
            isBaitPlaced = true;
        }

        struct LunchData
        {
            public readonly Vector3 initialVelocity;
            public readonly float timeToTarget;
            public LunchData(Vector3 initialVelocity, float timeToTarget)
            {
                this.initialVelocity = initialVelocity;
                this.timeToTarget = timeToTarget;
            }
        }
    }
}
