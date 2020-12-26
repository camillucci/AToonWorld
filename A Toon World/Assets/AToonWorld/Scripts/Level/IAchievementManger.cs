using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface to manage achievements
public interface IAchievementManger
{
    // True if the player got the achievement
    bool GotAchievement();

    // Text to be displayed in the end level menu
    string AchievementText();
}
