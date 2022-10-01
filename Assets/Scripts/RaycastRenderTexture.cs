using System;
using System.Collections.Generic;
using BML.ScriptableObjectCore.Scripts.SceneReferences;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace BML.Scripts
{
    public class RaycastRenderTexture : MonoBehaviour
    {
        [SerializeField] private string _hitTag = "";
        [SerializeField] private RectTransform _minigameScreenRect;
        [SerializeField] private RectTransform _parentCanvasRect;
        [SerializeField] private CameraSceneReference _mainCamera;
        [SerializeField] private CameraSceneReference _minigameCamera;

        private GameObject _minigameScreenObj;
        private Ray ray;

        private void OnEnable()
        {
            _minigameScreenObj = _minigameScreenRect.gameObject;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(ray.origin, ray.direction * 15f);
        }

        public void OnClick()
        {
            Debug.Log("Clicked");

            Vector3 mousePos = Mouse.current.position.ReadValue();
            RaycastResult uiRaycast = UIRaycast(ScreenPosToPointerData(mousePos));
            if (uiRaycast.gameObject != null)
            {
                float minigameScreenRealWidth = _minigameScreenRect.sizeDelta.x * _parentCanvasRect.localScale.x;
                float minigameScreenRealHeight = _minigameScreenRect.sizeDelta.y * _parentCanvasRect.localScale.y;
                
                Debug.Log($"hitUI: {uiRaycast.gameObject.name} | hitPos: {uiRaycast.screenPosition} | mousePos: {mousePos}");
                Debug.Log($"Rect screen pos: {_minigameScreenRect.position} | Center: {_minigameScreenRect.rect.center} | Width: {minigameScreenRealWidth} | Height: {minigameScreenRealHeight}");

                Bounds minigameScreenBounds = new Bounds();
                minigameScreenBounds.center = _minigameScreenRect.position;
                minigameScreenBounds.size =
                    new Vector3(minigameScreenRealWidth, minigameScreenRealHeight);

                Debug.Log($"Contains Mouse: {minigameScreenBounds.Contains(mousePos)}");

                Vector2 minigameScreenToMouseDelta = mousePos - _minigameScreenRect.position;
                Debug.Log($"Delta {minigameScreenToMouseDelta}");
                
                Vector2 minigameScreenCoord = new Vector2(
                    (minigameScreenToMouseDelta.x + minigameScreenRealWidth/2f) / minigameScreenRealWidth,
                    (minigameScreenToMouseDelta.y + minigameScreenRealHeight/2f) / minigameScreenRealHeight);
                
                Debug.Log($"Coord {minigameScreenCoord}");

                if (minigameScreenBounds.Contains(mousePos))
                {
                    ray = _minigameCamera.Value.ViewportPointToRay(new Vector3(minigameScreenCoord.x, minigameScreenCoord.y));
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                    if (hit.collider != null)
                    {
                        Debug.Log($"Hit object in additive scene: {hit.collider.gameObject.name}");
                    }
                }
            }
        }

        static RaycastResult UIRaycast (PointerEventData pointerData)
        {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            return results.Count < 1 ? new RaycastResult() : results[0];
        }

        static PointerEventData ScreenPosToPointerData(Vector2 screenPos)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = screenPos;
            return pointerEventData;
        }
    }
}