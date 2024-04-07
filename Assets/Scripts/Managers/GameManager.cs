using UnityEngine;
using Enums;
using Signals;
using Controllers;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public GameStates gameStates;
        [SerializeField] private Transform m_player=null;
        public bool m_boatPass = false;

        [Header("Aim Bar")]
        [SerializeField] private GameObject m_aimBar = null;

        [SerializeField] private GameObject exitFishingUI;
        [SerializeField] private GameObject gameOverUI;

        [SerializeField] private GameObject tapUI;

        private void Start()
        {
            Application.targetFrameRate = 60;
            GameSignals.Instance.onUpdatePlayer?.Invoke(m_player);
            HandleAimBar(false);
            tapUI.SetActive(true);
        }

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
            //GameSignals.Instance.onHopIntoBoat += OnHopIntoBoat;
            //GameSignals.Instance.onHopDownFromBoat += OnHopDownFromBoat;
            GameSignals.Instance.onGetPlayerBoatPass += OnGetPlayerBoatPass;
            GameSignals.Instance.onChangeGameState += OnChangeGameState;
            GameSignals.Instance.onGetGameState += OnGetGameState;
            GameSignals.Instance.onGameOver += GameOver;
            GameSignals.Instance.onUpdateFishingStatus += OnUpdateFishingStatus;
        }

        private void UnsubscribeEvents()
        {
            //GameSignals.Instance.onHopIntoBoat -= OnHopIntoBoat;
            //GameSignals.Instance.onHopDownFromBoat -= OnHopDownFromBoat;
            GameSignals.Instance.onGetPlayerBoatPass -= OnGetPlayerBoatPass;
            GameSignals.Instance.onChangeGameState -= OnChangeGameState;
            GameSignals.Instance.onGetGameState -= OnGetGameState;
            GameSignals.Instance.onGameOver -= GameOver;
            GameSignals.Instance.onUpdateFishingStatus -= OnUpdateFishingStatus;
        }
        #endregion

        void OnChangeGameState(GameStates states)
        {
            gameStates = states;
            HandleAimBar(gameStates == GameStates.attack);
            if(gameStates==GameStates.ingame)
            {
                tapUI.SetActive(false);
            }
        }

        GameStates OnGetGameState()
        {
            return gameStates;
        }

        void HandleAimBar(bool barVal)
        {
            if (m_aimBar != null)
            {
                m_aimBar.SetActive(barVal);
            }
        }

        bool OnGetPlayerBoatPass()
        {
            return m_boatPass;
        }

        void OnUpdateFishingStatus(bool f)
        {
            exitFishingUI.SetActive(f);
        }

        /*
        void OnHopIntoBoat(Transform m_player,Transform m_boat)
        {
            m_player.GetComponent<Rigidbody>().isKinematic = true;
            m_player.SetParent(m_boat);
            m_player.localPosition = Vector3.zero;
            m_player.localEulerAngles = Vector3.zero;
            //update player to boat
            GameSignals.Instance.onUpdatePlayer?.Invoke(m_boat);
            m_boatPass = false;//disable boat pass once player hops into boat
            exitFishingUI.SetActive(true);
        }

        void OnHopDownFromBoat(Transform m_boat)
        {
            m_player.parent = null;
            //Vector3 m_playerPos = m_player.position;
            //m_playerPos.y = 0f;
            m_player.position = new Vector3(0f,0f,5f);
            //disable boat controller after player hops down from boat
            m_player.GetComponent<Rigidbody>().isKinematic = false;
            GameSignals.Instance.onUpdatePlayer?.Invoke(m_player);
        }
        */

        #region exit from
        public void ExitFishingMode()
        {
            exitFishingUI.SetActive(false);
            //disable fishing mode
            GameSignals.Instance.onUpdateFishingStatus?.Invoke(false);
        }
        #endregion

        void GameOver()
        {
            //disable movement
            GameSignals.Instance.onChangeGameState?.Invoke(GameStates.gameOver);
            gameOverUI.SetActive(true);
        }

        public void Restart()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(sceneName);
        }
    }
}
