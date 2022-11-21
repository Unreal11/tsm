using Core.Contracts;
using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Core.Client
{
    [DisallowMultipleComponent]
    public class MatchClientController : MonoBehaviour
    {
        public static MatchClientController instance;
        public static UnityEvent matchWin;
        public static UnityEvent matchLose;
        public static UnityEvent matchDraw;

        public float timeSearchingLag;

        [Scene]
        public string battleScene;
        [Scene]
        public string menuScene;

        private Coroutine _searchingLag;

        private void Start()
        {
            instance = this;
            matchWin = new UnityEvent();
            matchLose = new UnityEvent();
            matchDraw = new UnityEvent();

            NetworkClient.RegisterHandler<RequestMatchDto>((requestMatchDto) => 
            {
                switch (requestMatchDto.RequestType)
                {
                    case MatchRequestType.FindingMatch:
                        SceneManager.LoadScene(battleScene);
                        break;
                    case MatchRequestType.WinMatch:
                        matchWin.Invoke();
                        break;
                    case MatchRequestType.LoseMatch:
                        matchLose.Invoke();
                        break;
                    case MatchRequestType.DrawMatch:
                        matchDraw.Invoke();
                        break;
                    case MatchRequestType.EndTurn:
                        BattleClientManager.SetTurn(false);
                        break;
                    case MatchRequestType.StartTurn:
                        BattleClientManager.SetTurn(true);
                        break;
                    case MatchRequestType.ExitMatch:
                        //BattleClientManager.SetTurn(false);
                        break;
                }
            }, false);
        }

        public static void EndTurn()
        {
            NetworkClient.Send(new RequestMatchDto
            {
                AccountId = MainClient.GetClientId(),
                RequestType = MatchRequestType.EndTurn
            });
        }

        public static void SearchingMatch()
        {
            DebugManager.AddLineDebugText("Searching match...", nameof(SearchingMatch));

            if (instance._searchingLag != null)
                instance.StopCoroutine(instance._searchingLag);

            instance._searchingLag = instance.StartCoroutine(instance.SearhicngLag());
        }

        public static void CancelSearchingMatch()
        {
            DebugManager.RemoveLineDebugText(nameof(SearchingMatch));

            if (instance._searchingLag != null)
                instance.StopCoroutine(instance._searchingLag);

            NetworkClient.Send(new RequestMatchDto
            {
                AccountId = MainClient.GetClientId(),
                RequestType = MatchRequestType.CancelFindingMatch
            });
        }

        private IEnumerator SearhicngLag()
        {
            yield return new WaitForSeconds(timeSearchingLag);

            NetworkClient.Send(new RequestMatchDto
            {
                AccountId = MainClient.GetClientId(),
                RequestType = MatchRequestType.FindingMatch
            });
        }
    }
}