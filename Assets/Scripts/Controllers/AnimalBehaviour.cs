using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Signals;
using DG.Tweening;
using Enums;
using UnityEngine.UI;

namespace Controllers
{
    public class AnimalBehaviour : MonoBehaviour
    {
        private Rigidbody animalRb;
        [Header("Animal Health")]
        [SerializeField] private int health=100;
        [SerializeField] private Transform healtBar;
        [SerializeField] private Image healthBarImg;
        private int curHealth = 0;

        private Transform m_player = null;
        private Vector3 m_playTarget = Vector3.zero;

        [Header("Animal Movement")]
        Sequence seq;
        private float m_restTime;
        private float restTimer = 0f;
        private float moveTime = 0f;

        [Header("Jump Towards Bait")]
        [SerializeField] private float h = 5f;
        [SerializeField] private float gravity = -10f;

        public AnimalStates animalState;
        private Vector3 startPos;

        private void Start()
        {
            animalRb = GetComponent<Rigidbody>();
            animalRb.useGravity = false;
            curHealth = health;
            UpateHealthBar();
            m_player = GameObject.FindGameObjectWithTag("Player").transform;
            animalState = AnimalStates.findTarget;//initially move around pond
            startPos = transform.position;
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
            GameSignals.Instance.onMissedWeaponShot += OnMissedWeaponShot;
            GameSignals.Instance.onFishBaitPlaced += OnFishBaitPlaced;
        }

        private void UnsubscribeEvents()
        {
            GameSignals.Instance.onMissedWeaponShot -= OnMissedWeaponShot;
            GameSignals.Instance.onFishBaitPlaced -= OnFishBaitPlaced;
        }
        #endregion

        private void Update()
        {
            if(animalState != AnimalStates.jumpTowardsBait)
            {
                if(animalState == AnimalStates.takeRest)
                {
                    restTimer += Time.deltaTime;
                    if (restTimer >= m_restTime)
                    {
                        animalState = AnimalStates.roamAround;
                    }
                }
                else
                {
                    //find new target and move towards it
                    if (animalState==AnimalStates.findTarget)
                    {
                        m_playTarget = GameSignals.Instance.onGetRandomPondPosition();
                        moveTime = Random.Range(0.8f, 1.5f);
                        animalState = AnimalStates.roamAround;
                    }
                    else
                    {
                        transform.position = Vector3.MoveTowards(transform.position, m_playTarget, Time.deltaTime * moveTime);
                        Quaternion lookRotation = Quaternion.LookRotation(m_playTarget - transform.position);
                        transform.rotation=Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime*2f);
                        if (Vector3.Distance(transform.position,m_playTarget)<=0.1f)
                        {
                            if(Random.Range(0,5)>=2)
                            {
                                restTimer = 0f;
                                m_restTime = Random.Range(1.5f, 2.5f);
                                animalState = AnimalStates.takeRest;
                            }
                            else
                            {
                                animalState=AnimalStates.findTarget;
                            }
                        }
                    }
                }
            }

            //make healthbar always look at player
            if (m_player!=null)
            {
                Vector3 diff = (m_player.position - healtBar.position).normalized;
                healtBar.LookAt(diff);
            }
        }

        private void OnTriggerEnter(Collider col)
        {
            if(col.gameObject.layer==LayerMask.NameToLayer("projectile"))
            {
                Debug.Log("damage taken!");
                ApplyDamage();
                col.gameObject.SetActive(false);
            }
        }

        void ApplyDamage()
        {
            curHealth -= WeaponSignals.Instance.onGetWeaponDamage();
            if(curHealth <= 0f)
            {
                healtBar.gameObject.SetActive(false);
                Death();
            }
            UpateHealthBar();
        }

        void Death()
        {
            //show death effect from here
            //GameObject deathEffect = Instantiate(,transform.position);
            //deathEffect.GetComponent<ParticleSystem>().Play();
            //this.gameObject.SetActive(false);
            Debug.Log("Fish has been killed!");
            GameSignals.Instance.onChangeGameState?.Invoke(GameStates.ingame);
            this.gameObject.SetActive(false);
            //lets instantiate new fish from here
        }

        void UpateHealthBar()
        {
            healthBarImg.fillAmount = (float)curHealth /(float)health;
        }

        void OnMissedWeaponShot()
        {
            seq.Kill();
            StartCoroutine(AllowThrowingBet());
            jumpTime = normalTimeToJump;
        }

        void JumpCompleted()
        {
            m_index = 0;
            trajectoryList.Clear();
            animalRb.velocity = Vector3.zero;
            animalRb.useGravity = false;
            transform.position = new Vector3(transform.position.x, startPos.y, transform.position.z);
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            StartCoroutine(AllowThrowingBet());
            GameSignals.Instance.onChangeGameState(GameStates.ingame);
        }

        IEnumerator AllowThrowingBet()
        {
            restTimer = 0f;
            m_restTime = Random.Range(1.5f, 2.5f);
            animalState = AnimalStates.takeRest;
            GameSignals.Instance.onBaitCatched?.Invoke();
            yield return new WaitForSeconds(2f);
            GameSignals.Instance.onHandlePlayerBet?.Invoke(false);
        }

        #region trajectory path calculation and moving through it
        //lets find out
        [SerializeField]private List<Vector3> trajectoryList = new List<Vector3>();
        private float normalTimeToJump = 0.05f;
        private float slowedTime = 1.5f;
        private float jumpTime = 0f;
        private int m_index = 0;

        void OnFishBaitPlaced(Vector3 baitPos)
        {
            m_index = 0;
            animalState = AnimalStates.jumpTowardsBait;
            seq.Kill();//killing animation sequence if any running
            trajectoryList.Clear();//clear previous trajectory
            //calculating trajectory
            LunchData lunchData = CalculateLunchData(baitPos);
            int resolution = 20;
            for (int i = 1; i < resolution; i++)
            {
                float simulationTime = i / (float)resolution * lunchData.timeToTarget;
                Vector3 displacement = lunchData.initialVelocity * simulationTime + Vector3.up * gravity * simulationTime * simulationTime / 2f;
                Vector3 drawPoint = transform.position + displacement;
                trajectoryList.Add(drawPoint);
            }
            jumpTime = normalTimeToJump;
            //now move
            MoveThroughTrajectoryPoints();
            //enable slow motion
            StartCoroutine(EnableSlowMo());
            //
            GameSignals.Instance.onChangeCameraLookAt?.Invoke(this.transform);
            GameSignals.Instance.onChangeGameState(GameStates.attack);
        }

        IEnumerator EnableSlowMo()
        {
            yield return new WaitForSeconds(0.5f);
            jumpTime = slowedTime;
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

        void MoveThroughTrajectoryPoints()
        {
            Sequence tjSeq = DOTween.Sequence();
            tjSeq.Append(transform.DOMove(trajectoryList[m_index], jumpTime).SetEase(Ease.Linear));
            tjSeq.Join(transform.DOLookAt(trajectoryList[m_index], jumpTime).SetEase(Ease.Linear));
            tjSeq.OnComplete(() =>
            {
                if (m_index < trajectoryList.Count - 1)
                {
                    m_index++;
                    MoveThroughTrajectoryPoints();
                }
                else
                {
                    JumpCompleted();
                }
            });
        }
        #endregion

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
