using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic ink in the game
/// </summary>
public interface IInkHandler
{
    void OnDrawDown(Vector2 mouseWorldPosition);

    /// <summary>
    /// Called when the user keeps holding the mouse for this frame
    /// </summary>
    /// <param name="mouseWorldPosition">Mouse position</param>
    /// <returns>Returns if this ink handler can keep drawing</returns>
    bool OnDrawHeld(Vector2 mouseWorldPosition);
    
    void OnDrawReleased(Vector2 mouseWorldPosition);
}
