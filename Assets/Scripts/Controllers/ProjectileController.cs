using UnityEngine;

namespace Controllers
{
    public class ProjectileController : MonoBehaviour
    {
        [SerializeField] private float maxDist = 10f;

        private Vector3 initialPos;

        private void Start()
        {
            initialPos = transform.position;
        }

        void Update()
        {
            if(Vector3.Distance(transform.position,initialPos)>=maxDist)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
