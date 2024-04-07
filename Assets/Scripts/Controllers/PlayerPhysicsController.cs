using UnityEngine;
using Signals;
using Enums;

namespace Controllers
{
    public class PlayerPhysicsController : MonoBehaviour
    {
        private void OnTriggerEnter(Collider col)
        {
            if(col.gameObject.layer==LayerMask.NameToLayer("fishRadar"))
            {
                GameSignals.Instance.onChangeGameState?.Invoke(GameStates.attack);
            }
            else if(col.gameObject.layer==LayerMask.NameToLayer("pond")|| col.gameObject.layer == LayerMask.NameToLayer("deathZone"))
            {
                GameSignals.Instance.onGameOver?.Invoke();
            }
        }

        private void OnTriggerExit(Collider col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("fishRadar"))
            {
                GameSignals.Instance.onChangeGameState?.Invoke(GameStates.ingame);
            }
        }
    }
}
