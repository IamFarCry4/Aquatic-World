using Extentions;
using UnityEngine.Events;
using System;
using UnityEngine;

namespace Signals
{
    public class WeaponSignals : MonoSingleton<WeaponSignals>
    {
        public UnityAction<int> onFireProjectile=delegate { };
        public UnityAction onResetAimBar = delegate { };
        public Func<int> onGetWeaponDamage= delegate { return 0; };
        public UnityAction<Transform> onUpdateWeaponTarget = delegate { };
    }
}
