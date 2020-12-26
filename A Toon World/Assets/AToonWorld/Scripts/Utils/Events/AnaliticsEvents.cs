using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

#if AnaliticsEnabled
namespace Events
{
    public static class AnaliticsEvents
    {
        /// <summary>Event raised when the player dies</summary>
        public static Event<Analitic> PlayerDeath = new Event<Analitic>();
        
        /// <summary>Event raised when a level starts</summary>
        public static Event<Analitic> LevelStart = new Event<Analitic>();

        /// <summary>Event raised when the player finishes a level</summary>
        public static Event<Analitic> LevelEnd = new Event<Analitic>();
        
        /// <summary>Event raised when the player touches a checkpoint</summary>
        public static Event<Analitic> Checkpoint = new Event<Analitic>();

        /// <summary>Event raised when the player finishes an ink</summary>
        public static Event<Analitic> InkFinished = new Event<Analitic>();

        /// <summary>Event raised when the player touches a checkpoint</summary>
        public static Event<Analitic> InksLevelAtCheckpoint = new Event<Analitic>();
        
        /// <summary>Event raised when the player clicks a feedback in the end level menu</summary>
        public static Event<Analitic> FeedbackSurvey = new Event<Analitic>();
        
        /// <summary>Event raised when an enemy is killed</summary>
        public static Event<Analitic> EnemyKilled = new Event<Analitic>();
    }
}
#endif