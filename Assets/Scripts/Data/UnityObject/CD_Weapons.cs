using System.Collections.Generic;
using Data.ValueObject;
using UnityEngine;

namespace Data.UnityObject
{
    [CreateAssetMenu(fileName = "CD_Weapons", menuName = "Aquatic World/CD_Weapons", order = 0)]
    public class CD_Weapons : ScriptableObject
    {
        public List<WeaponData> m_weapons;
    }
}
