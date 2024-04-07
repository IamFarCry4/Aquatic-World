using UnityEngine;
using Extentions;
using UnityEngine.Events;

namespace Signals
{
    public class InputSignals : MonoSingleton<InputSignals>
    {
        public UnityAction<Vector3> onUpdateDirectionalVector=delegate {};
    }
}
