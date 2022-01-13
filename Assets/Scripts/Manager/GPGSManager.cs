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

    // 로그인 시도
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
    /// 로그인 중인가?
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
            // 게임 불러오기 성공. 게임이 존재거나 null이거나 성공
            LoadGame(game);
        }
        else
        {
            // 게임 불러오는 요청을 실패
            _requestStatus = NetworkRequestStatus.Fail;
        }
    }

    private void LoadGame(ISavedGameMetadata game)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (null != savedGameClient)
        {
            Debug.Log("저장된 게임 로드 성공, 바이너리 데이타 분석 시작");
            savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
        }
        else
        {
            _requestStatus = NetworkRequestStatus.Done;
        }
    }

    // 저장 시도
    private void OnSaveGameOpenedToSave(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // 저장 1단계 성공
            SaveGame(game, _dataManager.GetPlayerModelToByteArray());
        }
        else
        {
            // 게임 불러오기 실패
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
            // 실패
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
            // 게임데이터가 없는 경우
            _data = null;
        }
    }

    // 서버에 게임 데이터를 저장하는 함수
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
            // 저장 2단계 성공
            _requestStatus = NetworkRequestStatus.Done;
        }
        else
        {
            // 실패
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