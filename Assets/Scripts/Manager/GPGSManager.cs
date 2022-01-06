using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using GooglePlayGames.BasicApi;

public class GPGSManager : Singleton<GPGSManager>
{
    private bool _isLogin = false;
    private void Awake()
    {
        InitGPGS();
    }

    private void InitGPGS()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
      // enables saving game progress.
      .EnableSavedGames()
      // requests the email address of the player be available.
      // Will bring up a prompt for consent.
      .RequestEmail()
      // requests a server auth code be generated so it can be passed to an
      //  associated back end server application and exchanged for an OAuth token.
      .RequestServerAuthCode(false)
      // requests an ID token be generated.  This OAuth token can be used to
      //  identify the player to other services such as Firebase.
      .RequestIdToken()
      .Build();

        PlayGamesPlatform.InitializeInstance(config);
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();
    }

    public void SignIn()
    {
#if UNITY_EDITOR

#else
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                _isLogin = true;
                Debug.Log("SignIn Success");
            }
        });
#endif
    }

    public bool IsAuthenticated()
    {
#if UNITY_EDITOR
        return false;
#else
        return PlayGamesPlatform.Instance.IsAuthenticated();
#endif
    }

    public void SetLogin(bool enabled)
    {
        _isLogin = enabled;
    }
}
