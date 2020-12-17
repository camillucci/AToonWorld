using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Analitic
{
    public Guid user;
    public string level;
    public DateTime dateTime;
    public EventName eventName;
    public Int32 game;
    public string[] value;

    public Analitic(string analiticRow)
    {
        string[] columns = analiticRow.Split(';');
        char[] trimChars = new char[3] { ' ', '\\', '\"'};
        this.user = Guid.Parse(columns[1].Trim(trimChars));
        this.level = columns[2].Trim(trimChars);
        this.game = int.Parse(columns[3].Trim(trimChars));
        this.dateTime = DateTime.Parse(columns[4].Trim(trimChars));
        this.eventName = (EventName)Enum.Parse(typeof(EventName), columns[5].Trim(trimChars), true);

        this.value = new string[columns.Length - 6];
        for(int i = 6; i < columns.Length; i++)
            this.value[i-6] = columns[i].Trim(trimChars);
    }

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
        return $"{user}|{level}|{game}|{dateTime.ToString("dd/MM/yy HH:mm:ss")}|{eventName.ToString()}|{string.Join("|", value)}";
    }
}