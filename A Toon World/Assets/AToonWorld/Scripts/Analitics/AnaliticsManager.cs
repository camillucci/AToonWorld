using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

        #if UNITY_EDITOR
        string[] analiticsReadable = new string[analitics.Count];
        for (int i = 0; i < analitics.Count; i++)
            analiticsReadable[i] = analitics[i].ToString();
        File.WriteAllLines("analitics.txt", analiticsReadable);
        #endif
    }

    private string DateTimeDifferenceInSeconds(DateTime d1, DateTime d2) => ((int)(d1 - d2).TotalSeconds).ToString();
    #endregion

    private void CompleteAndSend(EventName eventName, Analitic analitic)
    {
        analitic.user = _user;
        analitic.eventName = eventName;
        analitic.game = _game;
        UploadToRemoteSheet(analitic);
    }

    #if UNITY_EDITOR
    List<Analitic> analitics = new List<Analitic>();
    #endif

    private void UploadToRemoteSheet(Analitic analitic)
    {
        #if UNITY_EDITOR
        analitics.Add(analitic);
        #endif
        // todo
    }
}

#endif