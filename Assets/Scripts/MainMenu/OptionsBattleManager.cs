using Core;
using Core.Client;
using Core.Contracts;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainMenu
{
    public class OptionsBattleManager : MonoBehaviour
    {
        [Scene]
        public string sceneMenu;

        [SerializeField]
        private GameObject battleMenu;
        [SerializeField]
        private GameObject options;
        [SerializeField]
        private GameObject disconnectWindow;

        public void GoToMenu()
        {
            SceneManager.LoadScene(sceneMenu);
            NetworkClient.Send(new RequestMatchDto 
            {
                 AccountId = MainClient.GetClientId(),
                 RequestType = MatchRequestType.ExitMatch
            });
        }

        private void Start()
        {
            TowerSmashNetwork.ClientOnDisconnectEvent.AddListener(() => 
            {
                disconnectWindow.SetActive(true);
            });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (battleMenu.activeSelf)
                {
                    options.SetActive(false);
                    battleMenu.SetActive(false);
                }
                else
                {
                    battleMenu.SetActive(true);
                }
            }
        }
    }
}