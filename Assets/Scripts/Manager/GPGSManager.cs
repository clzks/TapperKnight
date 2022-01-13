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
    private DataManager _dataManager;
    private bool _isLogin = false;
    private JObject _data;
    private bool _isDone = false;
    private NetworkRequestStatus _requestStatus = NetworkRequestStatus.Count;
    private void Awake()
    {
        _dataManager = DataManager.Get();
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

    // �α��� �õ�
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

    /// <summary>
    /// �α��� ���ΰ�?
    /// </summary>
    /// <returns></returns>
    public bool IsAuthenticated()
    {
#if UNITY_EDITOR
        return false;
#else
        return PlayGamesPlatform.Instance.IsAuthenticated();
#endif
    }

    public void OpenGameData(GameDataCallbackType type)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        _requestStatus = NetworkRequestStatus.Progress;

        if (GameDataCallbackType.Load == type)
        {
            savedGameClient.OpenWithAutomaticConflictResolution("playerData", DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime, OnSaveGameOpenedToLoad);
        }
        else if(GameDataCallbackType.Save == type)
        {
            savedGameClient.OpenWithAutomaticConflictResolution("playerData", DataSource.ReadCacheOrNetwork,
               ConflictResolutionStrategy.UseLongestPlaytime, OnSaveGameOpenedToSave);
        }
    }

    private void OnSaveGameOpenedToLoad(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // ���� �ҷ����� ����. ������ ����ų� null�̰ų� ����
            LoadGame(game);
        }
        else
        {
            // ���� �ҷ����� ��û�� ����
            _requestStatus = NetworkRequestStatus.Fail;
        }
    }

    private void LoadGame(ISavedGameMetadata game)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (null != savedGameClient)
        {
            Debug.Log("����� ���� �ε� ����, ���̳ʸ� ����Ÿ �м� ����");
            savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
        }
        else
        {
            _requestStatus = NetworkRequestStatus.Done;
        }
    }

    // ���� �õ�
    private void OnSaveGameOpenedToSave(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // ���� 1�ܰ� ����
            SaveGame(game, _dataManager.GetPlayerModelToByteArray());
        }
        else
        {
            // ���� �ҷ����� ����
            _requestStatus = NetworkRequestStatus.Fail;
        }
    }

  

    private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            LoadGameData(data);
            _requestStatus = NetworkRequestStatus.Done;
        }
        else
        {
            // ����
            _requestStatus = NetworkRequestStatus.Fail;
        }
    }

    private void LoadGameData(byte[] data)
    {
        var stringData = Encoding.Default.GetString(data);

        if (null != data)
        {
            _data = JObject.Parse(stringData);
        }
        else
        {
            // ���ӵ����Ͱ� ���� ���
            _data = null;
        }
    }

    // ������ ���� �����͸� �����ϴ� �Լ�
    private void SaveGame(ISavedGameMetadata game, byte[] savedData)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
        builder = builder
            .WithUpdatedPlayedTime(TimeSpan.FromMinutes(game.TotalTimePlayed.Minutes + 1))
            .WithUpdatedDescription("Saved game at " + DateTime.Now);

        SavedGameMetadataUpdate updatedMetadata = builder.Build();
        savedGameClient.CommitUpdate(game, updatedMetadata, savedData, OnSavedGameWritten);
    }

    private void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // ���� 2�ܰ� ����
            _requestStatus = NetworkRequestStatus.Done;
        }
        else
        {
            // ����
            _requestStatus = NetworkRequestStatus.Fail;
        }
    }

    public JObject GetGameData()
    {
        return _data;
    }

    public NetworkRequestStatus GetRequestStatus()
    {
        return _requestStatus;
    }
}