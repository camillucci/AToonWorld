using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if AnaliticsEnabled

public enum EventName
{
    PlayerDeath,
    LevelTime,                      // Seconds spent on a level
    CheckpointTime,                 // Seconds spent to reach a checkpoint from the previous one - [ 2, 3, 26 ]
    InkFinished,                    // A specific ink is finished
    InkStatusAtCheckpoint,          // Inks status (amount) when the player reach a checkpoint
}

public class AnaliticsManager : MonoBehaviour
{
    
    private const string userKey = "UserGuid";
    private const string gamesKey = "UserGames";

    private Guid _user;
    private Int32 _game;

    void Awake()
    {
        DontDestroyOnLoad(this);
        InitUserAndGames();
        SubscribeToAnaliticsEvents();
    }

    private void InitUserAndGames()
    {
        if (PlayerPrefs.HasKey(userKey) && PlayerPrefs.HasKey(gamesKey))
        {
            _user = Guid.Parse(PlayerPrefs.GetString(userKey));
            _game = (Int32)PlayerPrefs.GetInt(gamesKey) + 1;
        }
        else
        {
            _user = Guid.NewGuid();
            _game = 0;
            PlayerPrefs.SetString(userKey, _user.ToString());
            PlayerPrefs.SetInt(gamesKey, _game);
        }
    }

    private void UpdateGames() => PlayerPrefs.SetInt(gamesKey, _game = (_game++) % Int32.MaxValue);

    private void SubscribeToAnaliticsEvents()
    {
        Events.AnaliticsEvents.PlayerDeath.AddListener(analitic => CompleteAndSend(EventName.PlayerDeath, analitic));
        Events.AnaliticsEvents.LevelStart.AddListener(analitic => SetLevelStart(analitic));
        Events.AnaliticsEvents.LevelEnd.AddListener(analitic => SetLevelEnd(analitic));
        Events.AnaliticsEvents.Checkpoint.AddListener(analitic => SetCheckpointTime(analitic));
        Events.AnaliticsEvents.InksLevelAtCheckpoint.AddListener(analitic => CompleteAndSend(EventName.InkStatusAtCheckpoint, analitic));
        Events.AnaliticsEvents.InkFinished.AddListener(analitic => CompleteAndSend(EventName.InkFinished, analitic));
    }

    #region LevelTime and CheckpointTime
    private DateTime _startTime;
    private (string, DateTime) _previousCheckpoint;
    private void SetLevelStart(Analitic analitic)
    {
        _startTime = analitic.dateTime;
        _previousCheckpoint = ("0", _startTime);
    }

    private void SetCheckpointTime(Analitic analitic)
    {
        string currentCheckpoint = analitic.value[0];
        analitic.value = new string[] { _previousCheckpoint.Item1, currentCheckpoint, DateTimeDifferenceInSeconds(analitic.dateTime, _previousCheckpoint.Item2) };
        _previousCheckpoint = (currentCheckpoint, analitic.dateTime);
        CompleteAndSend(EventName.CheckpointTime, analitic);
    }

    private void SetLevelEnd(Analitic analitic)
    {
        analitic.value = new string[] { DateTimeDifferenceInSeconds(analitic.dateTime, _startTime) };
        CompleteAndSend(EventName.LevelTime, analitic);
        UpdateGames();
    }

    private string DateTimeDifferenceInSeconds(DateTime d1, DateTime d2) => ((int)(d1 - d2).TotalSeconds).ToString();
    #endregion

    private void CompleteAndSend(EventName eventName, Analitic analitic)
    {
        analitic.user = _user;
        analitic.eventName = eventName;
        analitic.game = _game;
        StartCoroutine(UploadToRemoteForm(analitic));
    }

    private IEnumerator UploadToRemoteForm(Analitic analitic)
    {        
        RemoteFormUploader.Create(analitic).Upload();

        #if UNITY_EDITOR
        File.AppendAllText("analitics.txt", analitic.ToString());
        #endif

        yield return null;
    }
}

#endif