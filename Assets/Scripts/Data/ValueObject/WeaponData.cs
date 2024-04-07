using UnityEngine;

namespace Data.ValueObject
{
    [System.Serializable]
    public class WeaponData
    {
        public string m_weaponName;
        public float m_fireRate;
        public Transform m_weaponPrefab;
        public Transform m_weaponBullet;
        public int m_weaponDamage;
    }
}
