using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using GooglePlayGames.BasicApi;
using Newtonsoft.Json.Linq;
using GooglePlayGames.BasicApi.SavedGame;
using System.Text;
using System;

public class GPGSManager : Singleton<GPGSManager>
{
    private bool _isLogin = false;
    private JObject _data;
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

                 ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

                savedGameClient.OpenWithAutomaticConflictResolution("playerData", DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime, OnSaveGameOpenedToLoad);
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

    public void OnSaveGameOpenedToLoad(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            StartCoroutine(LoadGame(game));
        }
        else
        {
            // ���� �ҷ����� ����
        }
    }


    public IEnumerator LoadGame(ISavedGameMetadata game)
    {
        float timer = 0f;
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        while (null == savedGameClient)
        {
            timer += Time.deltaTime;
            savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            yield return null;

            if (timer >= 10f)
            {
                Debug.Log("����� ������ �����ϴ�");
                _data = null;
                yield break;
            }
        }

        Debug.Log("����� ���� �ε� ����, ���̳ʸ� ����Ÿ �м� ����");
        savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
    }

    public void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            LoadGameData(data);
        }
    }

    public void LoadGameData(byte[] data)
    {
        var stringData = Encoding.Default.GetString(data);

        if (null != data)
        {
            _data = JObject.Parse(stringData);
        }
        else
        {
            // �÷��̾� ���� �����Ͱ� ������ �ö����� �ҷ����� ���� ����

        }
    }

    // ������ ���� �����͸� �����ϴ� �Լ�
    public void SaveGame(ISavedGameMetadata game, byte[] savedData)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
        builder = builder
            .WithUpdatedPlayedTime(TimeSpan.FromMinutes(game.TotalTimePlayed.Minutes + 1))
            .WithUpdatedDescription("Saved game at " + DateTime.Now);

        SavedGameMetadataUpdate updatedMetadata = builder.Build();
        savedGameClient.CommitUpdate(game, updatedMetadata, savedData, OnSavedGameWritten);
    }

    public void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // ����
            Debug.Log("OnSaveGameWrittenSuccess");
        }
        else
        {
            // ����
            Debug.Log("OnSaveGameWrittenError");
        }
    }

    public JObject GetGameData()
    {
        return _data;
    }


    public void SetLogin(bool enabled)
    {
        _isLogin = enabled;
    }
}
