using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace BML.Scripts
{
    public class CursorController : MonoBehaviour
    {
        #region Inspector

        [Required, SerializeField] private RectTransform _cursorRoot;
        [FormerlySerializedAs("_cursorDefault")] [Required, SerializeField] private Image _cursorDefaultImage;
        [FormerlySerializedAs("_cursorInteractable")] [Required, SerializeField] private Image _cursorInteractableImage;

        [Required, SerializeField] private Vector2Int _cursorTextureHotspot;
        [Required, SerializeField] private Texture2D _cursorDefault;
        [Required, SerializeField] private Texture2D _cursorInteractable;

        [Required, SerializeField] private LayerMask _interactableLayerMask;

        [Required, SerializeField] private ProcessMinigameInput _inputProcessor;

        [ShowInInspector, ReadOnly] private bool _didHitInteractableLayer;
        [ShowInInspector, ReadOnly] private bool _didHitInteractableLayerLastCheck;

        #endregion

        #region Unity lifecycle

        private void Awake()
        {
            // Cursor.visible = false;
            UpdateCursorType();
        }

        private void FixedUpdate()
        {
            // Raycast for interactables in the minigame scene
            var minigameInteractableObject = _inputProcessor.TryRaycastIntoMinigame(_interactableLayerMask);
            
            // Raycast for interactables in the main UI
            Vector3 mousePos = Mouse.current.position.ReadValue();
            var pointerEventData = ProcessMinigameInput.ScreenPosToPointerData(mousePos);
            var uiRaycast = ProcessMinigameInput.UIRaycast(pointerEventData, _interactableLayerMask);
            var uiInteractableObject = uiRaycast?.Any() ?? false
                ? uiRaycast.First().gameObject
                : null;

            // 
            _didHitInteractableLayerLastCheck = _didHitInteractableLayer;
            _didHitInteractableLayer = (minigameInteractableObject != null) || (uiInteractableObject != null);
            bool interactableChanged = (_didHitInteractableLayerLastCheck != _didHitInteractableLayer);
            if (interactableChanged)
            {
                UpdateCursorType();
            }
            // Debug.Log($"CursorController DidHitInteractableLayer {_didHitInteractableLayer} | {minigameInteractableObject?.name} | {uiInteractableObject?.name}");

            // UpdateCursorPosition();
        }

        #endregion

        #region Cursor image

        private void UpdateCursorType()
        {
            // _cursorDefaultImage.enabled = !_didHitInteractableLayer;
            // _cursorInteractableImage.enabled = _didHitInteractableLayer;

            var cursorTexture = (_didHitInteractableLayer
                ? _cursorInteractable
                : _cursorDefault);
            Cursor.SetCursor(cursorTexture, _cursorTextureHotspot, CursorMode.Auto);
        }

        private void UpdateCursorPosition()
        {
            Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

            _cursorRoot.position = mouseWorldPos;

        }

        #endregion
    }
}