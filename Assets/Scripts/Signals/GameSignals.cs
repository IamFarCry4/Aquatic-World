using UnityEngine;
using UnityEngine.Events;
using Extentions;
using Enums;
using System;

namespace Signals
{
    public class GameSignals : MonoSingleton<GameSignals>
    {
        public UnityAction<GameStates> onChangeGameState=delegate { };
        public UnityAction<bool> onUpdateFishingStatus= delegate { };
        //public UnityAction<Transform,Transform> onHopIntoBoat = delegate { };
        //public UnityAction<Transform> onHopDownFromBoat = delegate { };
        //
        public UnityAction onGameOver= delegate { };
        public Func<GameStates> onGetGameState = delegate { return 0; };
        public UnityAction<Transform> onUpdatePlayer = delegate { };
        public UnityAction<Transform> onChangeCameraTarget = delegate { };
        public UnityAction<Transform> onChangeCameraLookAt = delegate { };
        public Func<bool> onGetPlayerBoatPass = delegate { return false; };
        public UnityAction<bool> onHandlePlayerBet= delegate { };
        //weapons
        public UnityAction onMissedWeaponShot = delegate { };
        public Func<Vector3> onGetRandomPondPosition = delegate { return Vector3.zero; };
        public UnityAction<Vector3> onFishBaitPlaced = delegate { };

        public UnityAction onBaitCatched = delegate { };
}
}
