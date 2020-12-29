using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerInkController;

/// <summary>
/// Generic ink in the game
/// </summary>
public interface IInkHandler
{
    //Scriptable Properties
    GameObject InkPrefab { get; }
    InkType InkType { get; }
    Color InkColor { get; }
    int MinPoolSize { get; }
    int MaxPoolSize { get; }
    bool CanDraw { get; }
    void BindInkController(PlayerInkController playerInkController);

    bool OnDrawDown(Vector2 mouseWorldPosition);

    /// <summary>
    /// Called when the user keeps holding the mouse for this frame
    /// </summary>
    /// <param name="mouseWorldPosition">Mouse position</param>
    /// <returns>Returns if this ink handler can keep drawing</returns>
    bool OnDrawHeld(Vector2 mouseWorldPosition);
    
    void OnDrawReleased(Vector2 mouseWorldPosition);
}
