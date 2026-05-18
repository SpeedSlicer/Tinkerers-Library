using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class AuthInit : MonoBehaviour
{
    async void Awake()
    {
        await UnityServices.InitializeAsync();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
}