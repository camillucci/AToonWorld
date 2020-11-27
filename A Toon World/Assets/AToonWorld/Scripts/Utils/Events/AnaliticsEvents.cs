using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Events
{
    public static class AnaliticsEvents
    {
        /// <summary>Evento scatenato quando muore il player</summary>
        public static Event<Analitic> PlayerDeath = new Event<Analitic>();
        
        /// <summary>Evento scatenato quando viene fatto partire un livello</summary>
        public static Event<Analitic> LevelStart = new Event<Analitic>();

        /// <summary>Evento scatenato quando il giocatore finisce un livello</summary>
        public static Event<Analitic> LevelEnd = new Event<Analitic>();
        
        /// <summary>Evento scatenato quando il giocatore passa sopra un chekpoint</summary>
        public static Event<Analitic> Checkpoint = new Event<Analitic>();

        /// <summary>Evento scatenato quando il giocatore finisce un inchiostro</summary>
        public static Event<Analitic> InkFinished = new Event<Analitic>();
    }
}