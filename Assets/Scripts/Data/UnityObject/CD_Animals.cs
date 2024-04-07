using System.Collections.Generic;
using Data.ValueObject;
using UnityEngine;

namespace Data.UnityObject
{
    [CreateAssetMenu(fileName = "CD_Animals", menuName = "Aquatic World/CD_Animals", order = 0)]
    public class CD_Animals : ScriptableObject
    {
        public List<AnimalData> m_animals;
    }
}

