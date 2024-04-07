using UnityEngine;
using Signals;
using Enums;

namespace Managers
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private GameObject m_inputJoystick;//SimpleInput joystick gameObject
        private GameStates gameStates;

        #region Event Subscriptions
        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            GameSignals.Instance.onChangeGameState += OnChangeGamestate;
        }

        private void UnsubscribeEvents()
        {
            GameSignals.Instance.onChangeGameState -= OnChangeGamestate;
        }
        #endregion

        void OnChangeGamestate(GameStates states)
        {
            gameStates = states;
            if(states==GameStates.attack)
            {
               m_inputJoystick.SetActive(false);
            }
            else
            {
                m_inputJoystick.SetActive(true);
            }
        }

        private void Update()
        {
            //if game has not been started then allow player to tap on screen to start the game
            if(Input.GetMouseButtonDown(0)&&gameStates==GameStates.mainMenu)
            {
                GameSignals.Instance.onChangeGameState?.Invoke(GameStates.ingame);
            }

            if(gameStates==GameStates.ingame)
            {
                float input_x = SimpleInput.GetAxis("Horizontal");
                float input_y = SimpleInput.GetAxis("Vertical");
                Vector3 dir_vec = new Vector3(input_x, 0f, input_y);
                //Vector3 dir_vec = new Vector3(input_x, 0f, input_y).normalized;//directional vector
                InputSignals.Instance.onUpdateDirectionalVector?.Invoke(dir_vec);
            }
        }
    }
}
