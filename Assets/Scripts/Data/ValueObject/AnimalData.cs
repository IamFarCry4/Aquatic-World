using UnityEngine;

namespace Data.ValueObject
{
    [System.Serializable]
    public class AnimalData
    {
        public string animalName;
        public int health;
        public Transform animalPrefab;
        public float speed;
        public float playRadius;
        public float playTime;
    }
}

