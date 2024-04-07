using UnityEngine;
using DG.Tweening;

namespace Controllers
{
    public class ArrowController : MonoBehaviour
    {
        void Start()
        {
            transform.DORotate(new Vector3(0f,180f,0f),1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        }
    }
}
