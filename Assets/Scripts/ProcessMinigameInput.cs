using System;
using System.Collections.Generic;
using BML.ScriptableObjectCore.Scripts.Events;
using BML.ScriptableObjectCore.Scripts.SceneReferences;
using BML.ScriptableObjectCore.Scripts.Variables;
using BML.Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BML.Scripts
{
    public class ProcessMinigameInput : MonoBehaviour
    {
        [SerializeField] private LayerMask _hitMask;
        [Required, SerializeField] private RectTransform _minigameRect;
        [Required, SerializeField] private RectTransform _parentCanvasRect;
        [Required, SerializeField] private CameraSceneReference _minigameCamera;
        [SerializeField] private Vector3Variable _mouseWorldPosInMinigame;
        [SerializeField] private GameEvent _onCancelDrag;
        [SerializeField] private BoolVariable _isPaused;
        private bool IsMinigameRunning => _minigameCamera.Value != null;
        
        private bool _dragging;
        private Ray ray;

        #region Unity Lifecycle

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(ray.origin, ray.direction * 15f);
        }

        private void Update()
        {
            if (_minigameCamera.Value == null)
                return;
            
            Vector3 mousePos = Mouse.current.position.ReadValue();
            
            var minigameScreenSpacePos = Camera.main.WorldToScreenPoint(_minigameRect.position);
            minigameScreenSpacePos.z = 0;

            float realWidth = _minigameRect.sizeDelta.x * _parentCanvasRect.localScale.x;
            float realHeight = _minigameRect.sizeDelta.y * _parentCanvasRect.localScale.y;
            
            Vector2 minigameScreenToMouseDelta = mousePos - minigameScreenSpacePos;
            
            Vector2 minigameScreenCoord = new Vector2(
                (minigameScreenToMouseDelta.x + realWidth/2f) / realWidth,
                (minigameScreenToMouseDelta.y + realHeight/2f) / realHeight);
            
            _mouseWorldPosInMinigame.Value = _minigameCamera.Value.ViewportToWorldPoint(minigameScreenCoord);
        }

        #endregion

        #region Input Callbacks

        public void OnClick(InputAction.CallbackContext context)
        {
            if (!IsMinigameRunning)
                return;
            
            if (context.performed)
            {
                GameObject hitObj = TryRaycastIntoMinigame(_hitMask);
                if (hitObj == null)
                    return;
            
                Clickable clickable = hitObj.GetComponent<Clickable>();
                if(clickable != null) {
                    clickable.EmitClick();
                }
            }
        }
        
        public void OnDrag(InputAction.CallbackContext context) {
            if (!IsMinigameRunning)
                return;
            
            if(context.performed) {
                _dragging = true;
                
                GameObject hitObj = TryRaycastIntoMinigame(_hitMask);
                if (hitObj == null)
                    return;
                
                Draggable draggable = hitObj.GetComponent<Draggable>();
                if(draggable != null) {
                    draggable.StartDrag();
                }
                
            }
            if(_dragging && context.canceled) {
                _dragging = false;
                
                _onCancelDrag.Raise();
            }
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            _isPaused.Value = !_isPaused.Value;

            if (_isPaused.Value)
            {
                Time.timeScale = 0f;
                AudioListener.pause = true;
            }
            else
            {
                Time.timeScale = 1f;
                AudioListener.pause = false;
            }
        }

        #endregion

        public GameObject TryRaycastIntoMinigame(LayerMask layerMask)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            RaycastResult uiRaycast = UIRaycast(ScreenPosToPointerData(mousePos));

            // Debug.Log(uiRaycast.gameObject.name);
            
            if (uiRaycast.gameObject == null)
                return null;

            var minigameScreenSpacePos = Camera.main.WorldToScreenPoint(_minigameRect.position);
            minigameScreenSpacePos.z = 0;
            var sizeDelta = _minigameRect.sizeDelta;
            var localScale = _parentCanvasRect.localScale;
            float realWidth = sizeDelta.x * localScale.x;
            float realHeight = sizeDelta.y * localScale.y;

            Bounds minigameScreenBounds = new Bounds();
            minigameScreenBounds.center = minigameScreenSpacePos;
            minigameScreenBounds.size =
                new Vector3(realWidth, realHeight);

            // Debug.Log(_minigameRect.position);
            // Debug.Log(minigameScreenSpacePos);
            // Debug.Log(uiRaycast.screenPosition);
            // Debug.Log(minigameScreenBounds.Contains(uiRaycast.screenPosition));
            
            if (!minigameScreenBounds.Contains(uiRaycast.screenPosition))
                return null;
            
            Vector2 minigameScreenToMouseDelta = mousePos - minigameScreenSpacePos;
            
            Vector2 minigameScreenCoord = new Vector2(
                (minigameScreenToMouseDelta.x + realWidth/2f) / realWidth,
                (minigameScreenToMouseDelta.y + realHeight/2f) / realHeight);
            
            ray = _minigameCamera.Value.ViewportPointToRay(new Vector3(minigameScreenCoord.x, minigameScreenCoord.y));
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null)
            {
                GameObject hitObj = hit.collider.gameObject;
                // Debug.Log($"Hit object in additive scene: {hitObj.name}");
                if (hitObj.IsInLayerMask(layerMask))
                {
                    // Debug.Log("Hit object belongs to hit layermask");
                    return hitObj;
                }
            }

            return null;
        }

        public static RaycastResult UIRaycast (PointerEventData pointerData)
        {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            return results.Count < 1 ? new RaycastResult() : results[0];
        }

        public static PointerEventData ScreenPosToPointerData(Vector2 screenPos)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = screenPos;
            return pointerEventData;
        }
    }
}