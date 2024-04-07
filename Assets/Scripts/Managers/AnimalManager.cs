using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.UnityObject;
using Signals;

namespace Managers
{
    public class AnimalManager : MonoBehaviour
    {
        [SerializeField] private int m_animalIndex = 0;
        private CD_Animals m_animaData;

        //[SerializeField]private Transform pond = null;

        [SerializeField] private float yPos = -0.5f;

        [Header("Pond Info")]
        [SerializeField] private float min_pondX;
        [SerializeField] private float max_pondX;
        [SerializeField] private float min_pondZ;
        [SerializeField] private float max_pondZ;

        #region initialization
        private void Start()
        {
            GetReferences();
            SpawnAnimal();
        }

        //getting refrence of weapon data list 
        private void GetReferences()
        {
            m_animaData = GetAnimalData();
        }

        private CD_Animals GetAnimalData()
        {
            return Resources.Load<CD_Animals>("Data/CD_Animals");
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
            GameSignals.Instance.onGetRandomPondPosition += OnGetRandomPondPosition;
        }

        private void UnsubscribeEvents()
        {
            GameSignals.Instance.onGetRandomPondPosition -= OnGetRandomPondPosition;
        }
        #endregion

        Vector3 OnGetRandomPondPosition()
        {
           return new Vector3(Random.Range(min_pondX, max_pondX), yPos, Random.Range(min_pondZ, max_pondZ));
        }

        //spawn player weapon to player hand
        void SpawnAnimal()
        {
            Vector3 randomSpawnPos = OnGetRandomPondPosition();
            Transform m_animal = Instantiate(m_animaData.m_animals[m_animalIndex].animalPrefab, randomSpawnPos,Quaternion.identity);
            m_animal.localEulerAngles = Vector3.zero;
            m_animal.gameObject.SetActive(true);
            WeaponSignals.Instance.onUpdateWeaponTarget?.Invoke(m_animal);
        }
        #endregion
    }
}
