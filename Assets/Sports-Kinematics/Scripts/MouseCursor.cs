using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allow mouse cursor to be segt to another cursor
/// </summary>
public class MouseCursor : MonoBehaviour {

    /// <summary>
    /// Texture to set mouse cursor to
    /// </summary>
    public Texture2D cursorTexture;
    
    /// <summary>
    /// Mouse Cursor mode
    /// </summary>
    public CursorMode cursorMode = CursorMode.Auto;

    /// <summary>
    /// hot spot for mouse
    /// </summary>
    public Vector2 hotSpot = Vector2.zero;

    /// <summary>
    /// GameObject start function.
    /// </summary>
    // Use this for initialization
    void Start () {
        // when we mouse over this object, set the cursor
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }
}
