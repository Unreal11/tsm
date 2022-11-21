using UnityEngine;
using Mirror;
using Core.Contracts;
using UnityEngine.Events;

namespace Core.Client
{
    public class AuthClientController : MonoBehaviour
    {
        public static AuthClientController instance;
        public static UnityEvent authSuccessEvent;

        public void ExitGame()
        {
            Application.Quit();
        }

        private void Start()
        {
            instance = this;
            authSuccessEvent = new UnityEvent();

            NetworkClient.RegisterHandler<AccountDto>(HandleResponceAuth, false);
        }

        public void AuthRequestAsGuest()
        {
            NetworkClient.Send(new AuthDto
            {
                Login = "GuestLogin",
                Password = "GuestPassword",
                IsGuest = true
            });
        }
        
        public void HandleResponceAuth(AccountDto accountDto)
        {
            MainClient.SetAuth(true);
            MainClient.SetClientId(accountDto.Id);
            DebugManager.AddLineDebugText($"{accountDto.Login}: {accountDto.Id}", "Account");

            authSuccessEvent.Invoke();
        }
    }
}