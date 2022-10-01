﻿using System;
using System.Collections.Generic;
using BML.ScriptableObjectCore.Scripts.SceneReferences;
using BML.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace BML.Scripts
{
    public class RaycastRenderTexture : MonoBehaviour
    {
        [SerializeField] private LayerMask _hitMask;
        [SerializeField] private RectTransform _minigameRect;
        [SerializeField] private RectTransform _parentCanvasRect;
        [SerializeField] private CameraSceneReference _minigameCamera;

        private Ray ray;

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(ray.origin, ray.direction * 15f);
        }

        public void OnClick()
        {
            TryRaycastIntoMinigame();
        }

        private void TryRaycastIntoMinigame()
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            RaycastResult uiRaycast = UIRaycast(ScreenPosToPointerData(mousePos));

            if (uiRaycast.gameObject == null)
                return;
            
            float realWidth = _minigameRect.sizeDelta.x * _parentCanvasRect.localScale.x;
            float realHeight = _minigameRect.sizeDelta.y * _parentCanvasRect.localScale.y;

            Bounds minigameScreenBounds = new Bounds();
            minigameScreenBounds.center = _minigameRect.position;
            minigameScreenBounds.size =
                new Vector3(realWidth, realHeight);

            if (!minigameScreenBounds.Contains(mousePos))
                return;
            
            Vector2 minigameScreenToMouseDelta = mousePos - _minigameRect.position;
            
            Vector2 minigameScreenCoord = new Vector2(
                (minigameScreenToMouseDelta.x + realWidth/2f) / realWidth,
                (minigameScreenToMouseDelta.y + realHeight/2f) / realHeight);
            
            ray = _minigameCamera.Value.ViewportPointToRay(new Vector3(minigameScreenCoord.x, minigameScreenCoord.y));
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null)
            {
                GameObject hitObj = hit.collider.gameObject;
                Debug.Log($"Hit object in additive scene: {hitObj.name}");
                if (hitObj.IsInLayerMask(_hitMask))
                {
                    Debug.Log("Hit object belongs to hit layermask");
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