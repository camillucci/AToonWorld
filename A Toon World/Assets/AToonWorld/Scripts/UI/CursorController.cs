using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.UI;
using Assets.AToonWorld.Scripts.Utils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Assets.AToonWorld.Scripts.UI
{
    public class CursorController : MonoBehaviour
    {
        public enum CursorType { Game, Menu}

        [SerializeField] private Texture2D _GameTexture;
        private Vector2 _targetPoint;

        void Awake()
        {
            _targetPoint = new Vector2(_GameTexture.width / 2, _GameTexture.height / 2);
            Events.InterfaceEvents.CursorChangeRequest.AddListener(cursorType => 
            {
                if (cursorType == CursorType.Game) ApplyGameCursor();
                if (cursorType == CursorType.Menu) ApplyMenuCursor();
            });
        }

        private void ApplyGameCursor()
        {
            Cursor.SetCursor(_GameTexture, _targetPoint, CursorMode.Auto);
        }

        private void ApplyMenuCursor()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}
