using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

#if AnaliticsEnabled

public class Analitic
{
    public Guid user;
    public string level;
    public DateTime dateTime;
    public EventName eventName;
    public long game;
    public string[] value;

    public Analitic(params Object[] value)
    {
        this.level = SceneManager.GetActiveScene().name;
        this.dateTime = DateTime.Now;
        
        this.value = new string[value.Length];
        for (int i = 0; i < value.Length; i++)
            this.value[i] = value[i].ToString();
    }

    public override string ToString()
    {
        return $"{user} {level} {dateTime.ToShortDateString()} {dateTime.ToShortTimeString()} {eventName.ToString()} {string.Join(" ", value)}";
    }
}

#endif