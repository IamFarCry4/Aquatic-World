using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using Signals;
using Data.UnityObject;

namespace Managers
{
    public class PlayerWeaponManager : MonoBehaviour
    {
        [SerializeField] private int m_weapoinIndex = 0;//since we have only 1 weapon index should be 0
        private CD_Weapons m_weaponData;
        [SerializeField] private Transform m_weaponPoint=null;//point where weapon will be attatched
        private Transform m_target = null;

        private float fireTimer = 0f;
        private bool projectileFired = false;

        #region initialization
        private void Awake()
        {
            GetReferences();
            //SpawnPlayerWeapon();//for spawnning player weapon at player hand
        }

        //getting refrence of weapon data list 
        private void GetReferences()
        {
            m_weaponData = GetWeaponData();
        }

        private CD_Weapons GetWeaponData()
        {
            return Resources.Load<CD_Weapons>("Data/CD_Weapons");
        }

        //spawn player weapon to player hand
        void SpawnPlayerWeapon()
        {
            Transform m_weapon = Instantiate(m_weaponData.m_weapons[m_weapoinIndex].m_weaponPrefab, m_weaponPoint);
            m_weapon.localEulerAngles = Vector3.zero;
            m_weapon.gameObject.SetActive(true);
        }
        #endregion

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
            WeaponSignals.Instance.onFireProjectile += OnFireProjectile;
            WeaponSignals.Instance.onUpdateWeaponTarget += OnUpdateWeaponTarget;
            WeaponSignals.Instance.onGetWeaponDamage += OnGetWeaponDamage;
        }

        private void UnsubscribeEvents()
        {
            WeaponSignals.Instance.onFireProjectile -= OnFireProjectile;
            WeaponSignals.Instance.onUpdateWeaponTarget -= OnUpdateWeaponTarget;
            WeaponSignals.Instance.onGetWeaponDamage -= OnGetWeaponDamage;
        }
        #endregion

        int OnGetWeaponDamage()
        {
            return m_weaponData.m_weapons[m_weapoinIndex].m_weaponDamage;
        }

        void OnUpdateWeaponTarget(Transform target)
        {
            m_target = target;
        }

        void OnFireProjectile(int aimValue)
        {
            if (aimValue == 0)
            {
                ShootMissTaget();
                GameSignals.Instance.onMissedWeaponShot?.Invoke();
                GameSignals.Instance.onChangeGameState?.Invoke(GameStates.ingame);
            }
            else
            {
                ShootOnTarget();
            }
            projectileFired = true;
        }

        private void Update()
        {
            if(GameSignals.Instance.onGetGameState()==GameStates.attack&&projectileFired)
            {
                fireTimer += Time.deltaTime;
                if(fireTimer>= m_weaponData.m_weapons[m_weapoinIndex].m_fireRate)
                {
                    WeaponSignals.Instance.onResetAimBar?.Invoke();
                    projectileFired = false;
                    fireTimer = 0f;
                }
            }
        }

        void ShootOnTarget()
        {
            Transform m_bullet = Instantiate(m_weaponData.m_weapons[m_weapoinIndex].m_weaponBullet, m_weaponPoint.position, Quaternion.identity);
            m_bullet.LookAt(m_target.position);
            m_bullet.gameObject.SetActive(true);
            m_bullet.GetComponent<Rigidbody>().AddForce(m_bullet.transform.forward * 10f, ForceMode.Impulse);
        }

        void ShootMissTaget()
        {
            Vector3 m_targetScale = m_target.localScale;
            int dirIndex = Random.Range(0, 4);
            Vector3 m_missedPos = m_target.position;

            switch (dirIndex)
            {
                case 0:
                    //lets choose left bound
                    m_missedPos.x  += (m_targetScale.x / 2f) + 1f;
                    break;
                case 1:
                    //lets choose right bound
                    m_missedPos.x -= (m_targetScale.x / 2f) + 1f;
                    break;
                case 2:
                    //lets choose up bound
                    m_missedPos.y += (m_targetScale.y / 2f) + 1f;
                    break;
                case 3:
                    //lets choose down bound
                    m_missedPos.y -= (m_targetScale.y / 2f) + 1f;
                    break;
                default:
                    //lets choose left bound
                    m_missedPos.x += (m_targetScale.x / 2f) + 1f;
                    break;
            }

            Transform m_bullet = Instantiate(m_weaponData.m_weapons[m_weapoinIndex].m_weaponBullet, m_weaponPoint.position, Quaternion.identity);
            m_bullet.LookAt(m_missedPos);
            m_bullet.gameObject.SetActive(true);
            m_bullet.GetComponent<Rigidbody>().AddForce(m_bullet.transform.forward * 10f, ForceMode.Impulse);
        }
    }
}
