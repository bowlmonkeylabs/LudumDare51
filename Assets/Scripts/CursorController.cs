using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BML.Scripts
{
    public class CursorController : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Image _cursorDefault;
        [SerializeField] private Image _cursorInteractable;

        [SerializeField] private LayerMask _interactableLayerMask;

        [Required, SerializeField] private ProcessMinigameInput _inputProcessor;

        [ShowInInspector, ReadOnly] private bool _didHitInteractableLayer;

        #endregion

        #region Unity lifecycle

        private void Awake()
        {
            Cursor.visible = false;
        }

        private void FixedUpdate()
        {
            // Raycast for interactables in the minigame scene
            var minigameInteractableObject = _inputProcessor.TryRaycastIntoMinigame(_interactableLayerMask);
            
            // Raycast for interactables in the main UI
            Vector3 mousePos = Mouse.current.position.ReadValue();
            var pointerEventData = ProcessMinigameInput.ScreenPosToPointerData(mousePos);
            var uiRaycast = ProcessMinigameInput.UIRaycast(pointerEventData);
            var uiInteractableObject = uiRaycast.gameObject;

            _didHitInteractableLayer = (minigameInteractableObject != null) || (uiInteractableObject != null);
            UpdateCursorType();
        }

        #endregion

        #region Cursor image

        private void UpdateCursorType()
        {
            _cursorDefault.enabled = !_didHitInteractableLayer;
            _cursorInteractable.enabled = _didHitInteractableLayer;
        }

        #endregion
    }
}