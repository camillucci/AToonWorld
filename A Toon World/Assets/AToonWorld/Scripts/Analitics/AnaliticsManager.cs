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
}

public class AnaliticsManager : MonoBehaviour
{
    // todo salvare in modo persistente
    private Guid _user;
    private long _game = 0;

    void Awake()
    {
        DontDestroyOnLoad(this);
        _user = Guid.NewGuid();
        SubscribeToAnaliticsEvents();
    }

    private void SubscribeToAnaliticsEvents()
    {
        Events.AnaliticsEvents.PlayerDeath.AddListener(analitic => CompleteAndSend(EventName.PlayerDeath, analitic));
        Events.AnaliticsEvents.LevelStart.AddListener(analitic => SetLevelStart(analitic));
        Events.AnaliticsEvents.LevelEnd.AddListener(analitic => SetLevelEnd(analitic));
        Events.AnaliticsEvents.Checkpoint.AddListener(analitic => SetCheckpointTime(analitic));
        Events.AnaliticsEvents.InkFinished.AddListener(analitic => CompleteAndSend(EventName.InkFinished, analitic));
    }

    #region LevelTime and CheckpointTime
    private DateTime _startTime;
    private string _previousCheckpoint;
    private void SetLevelStart(Analitic analitic) => _startTime = analitic.dateTime;

    private void SetCheckpointTime(Analitic analitic)
    {
        string currentCheckpoint = analitic.value[0];
        analitic.value = new string[] { _previousCheckpoint, currentCheckpoint, (analitic.dateTime - _startTime).TotalSeconds.ToString() };
        _previousCheckpoint = currentCheckpoint;
        CompleteAndSend(EventName.CheckpointTime, analitic);
    }

    private void SetLevelEnd(Analitic analitic)
    {
        analitic.value = new string[] { (analitic.dateTime - _startTime).TotalSeconds.ToString() };
        CompleteAndSend(EventName.LevelTime, analitic);
        _game = (_game++) % long.MaxValue;

        #if UNITY_EDITOR
        string[] analiticsReadable = new string[analitics.Count];
        for (int i = 0; i < analitics.Count; i++)
            analiticsReadable[i] = analitics[i].ToString();
        File.WriteAllLines("analitics.txt", analiticsReadable);
        #endif
    }
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
        Debug.Log(analitic.ToString());
        #endif
        // todo
    }
}

#endif