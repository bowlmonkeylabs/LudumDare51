using System;
using BML.ScriptableObjectCore.Scripts.Events;
using BML.ScriptableObjectCore.Scripts.Variables;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace EmailInbox
{
    public class UiEmailViewerController : MonoBehaviour
    {
        #region Inspector

        [SerializeField, ReadOnly] private EmailInstancePayload? _emailData;
        
        [SerializeField] private DynamicGameEvent _onOpenEmail;
        [SerializeField] private DynamicGameEvent _onCloseEmail;
        [SerializeField] private UnityEvent _onCloseEmailCallback;
        
        [Required, SerializeField] private TMP_Text _textFromAddress;
        [Required, SerializeField] private TMP_Text _textSubject;
        [Required, SerializeField] private TMP_Text _textBody;
        [Required, SerializeField] private Canvas _parentCanvas;

        [Required, SerializeField] private UnityEvent _onSuccessAcceptTask;
        [Required, SerializeField] private UnityEvent _onPenaltyRejectTask;
        [Required, SerializeField] private UnityEvent _onPenaltyAcceptSpam;
        [Required, SerializeField] private UnityEvent _onSuccessRejectSpam;

        [Required, SerializeField] private GameEvent _onMinigameSuccess;
        [SerializeField] private UnityEvent _onMinigameSuccessCallback;
        [Required, SerializeField] private GameEvent _onMinigameFail;
        [SerializeField] private UnityEvent _onMinigameFailedCallback;

        #endregion

        #region Unity lifecycle

        private void Awake()
        {
            _emailData = null;
            RenderEmailData();
            _onOpenEmail.Subscribe(OnOpenEmailDynamic);
            _onMinigameSuccess.Subscribe(OnMinigameSuccess);
            _onMinigameFail.Subscribe(OnMinigameFailed);
        }

        private void OnDestroy()
        {
            _onOpenEmail.Unsubscribe(OnOpenEmailDynamic);
            _onMinigameFail.Unsubscribe(OnMinigameFailed);
        }

        #endregion

        #region Emails

        private void OnOpenEmailDynamic(object prevValue, object currentValue)
        {
            var payload = currentValue as EmailInstancePayload?;
            if (payload == null) return;
            
            OnOpenEmail(payload.Value);
        }

        private void OnOpenEmail(EmailInstancePayload emailInstanceData)
        {
            if (_emailData != null)
            {
                CloseEmail(false, true);
            }
            
            _emailData = emailInstanceData;
            
            RenderEmailData();
        }
        
        public void CloseEmail(bool removeFromInbox = true, bool unloadScene = false)
        {
            if (unloadScene)
            {
                try
                {
                    SceneManager.UnloadSceneAsync(_emailData.Value.EmailData.MinigameScene.name);
                }
                catch (Exception e)
                {
                    
                }
            }
            if (removeFromInbox)
            {
                _onCloseEmail.Raise(_emailData);
            }
            
            _emailData = null;
            _onCloseEmailCallback?.Invoke();
            RenderEmailData();
        }

        private void RenderEmailData()
        {
            var showThisObject = (_emailData != null);
            _parentCanvas.enabled = showThisObject;
            
            if (!showThisObject) return;

            _textFromAddress.text = _emailData.Value.EmailData.FromAddress;
            _textSubject.text = _emailData.Value.EmailData.Subject;
            _textBody.text = _emailData.Value.EmailData.Body;
        }

        public void OnClickAccept()
        {
            if (_emailData == null)
            {
                return;
            }
            if (_emailData.Value.EmailData.IsSpam)
            {
                _onPenaltyAcceptSpam?.Invoke();
                CloseEmail();
                return;
            }
            
            SceneManager.LoadScene(_emailData.Value.EmailData.MinigameScene.name, LoadSceneMode.Additive);
            _onSuccessAcceptTask?.Invoke();
        }

        public void OnClickReject()
        {
            if (_emailData == null)
            {
                return;
            }
            if (!_emailData.Value.EmailData.IsSpam)
            {
                _onPenaltyRejectTask?.Invoke();
                CloseEmail();
                return;
            }
            
            _onSuccessRejectSpam?.Invoke();
            CloseEmail();
        }

        public void OnMinigameSuccess()
        {
            // Debug.Log($"UiEmailViewerController OnMinigameSuccess");
            
            _onMinigameSuccessCallback?.Invoke();
            
            CloseEmail(true, true);
        }

        public void OnMinigameFailed()
        {
            // Debug.Log($"UiEmailViewerController OnMinigameFailed");
            
            _onMinigameFailedCallback?.Invoke();
            
            CloseEmail();
        }

        #endregion
    }
}