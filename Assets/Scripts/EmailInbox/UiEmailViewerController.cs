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
        [SerializeField] private UnityEvent _onOpenEmailCallback;
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
        
        [Required, SerializeField] private GameEvent _onMinigameEnded;
        [SerializeField] private UnityEvent _onMinigameEndedCallback;
        [Required, SerializeField] private BoolReference _lastMinigameSuccess; 

        #endregion

        #region Unity lifecycle

        private void Awake()
        {
            _emailData = null;
            RenderEmailData();
            _onOpenEmail.Subscribe(OnOpenEmailDynamic);
            _onMinigameSuccess.Subscribe(OnMinigameSuccess);
            _onMinigameFail.Subscribe(OnMinigameFailed);
            _onMinigameEnded.Subscribe(OnMinigameEnded);
        }

        private void OnDestroy()
        {
            _onOpenEmail.Unsubscribe(OnOpenEmailDynamic);
            _onMinigameFail.Unsubscribe(OnMinigameFailed);
            _onMinigameEnded.Unsubscribe(OnMinigameEnded);
        }

        #endregion

        #region Emails

        private void OnOpenEmailDynamic(object prevValue, object currentValue)
        {
            var payload = currentValue as EmailInstancePayload;
            if (payload == null) return;
            
            OnOpenEmail(payload);
        }

        private void OnOpenEmail(EmailInstancePayload emailInstanceData)
        {
            Debug.Log($"OnOpenEmail Try open {emailInstanceData.InstanceId}");
            
            if (_emailData != null)
            {
                bool alreadyOpen = (_emailData.InstanceId == emailInstanceData.InstanceId);
                if (alreadyOpen)
                {
                    Debug.Log($"OnOpenEmail Already open {_emailData.InstanceId}");
                    return;
                }
                
                CloseEmail(_lastMinigameSuccess.Value, _lastMinigameSuccess.Value);
            }

            _lastMinigameSuccess.Value = false;
            _emailData = emailInstanceData;
            
            _onOpenEmailCallback?.Invoke();
            RenderEmailData();
        }
        
        public void CloseEmail(bool removeFromInbox, bool countAsFinishedItem)
        {
            CloseMinigame();
            
            if (removeFromInbox && _emailData != null)
            {
                var payload = new RemoveEmailInstancePayload
                {
                    EmailInstance = _emailData,
                    CountAsFinishedItem = countAsFinishedItem,
                };
                Debug.Log($"Requesting to close email ({_emailData.EmailData.Subject}) {_emailData.InstanceId}");
                _onCloseEmail.Raise(payload);
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

            _textFromAddress.text = _emailData.EmailData.FromAddress;
            _textSubject.text = _emailData.EmailData.Subject;
            _textBody.text = _emailData.EmailData.Body;
        }

        public void OnClickAccept()
        {
            if (_emailData == null)
            {
                return;
            }
            if (_emailData.EmailData.IsSpam)
            {
                _onPenaltyAcceptSpam?.Invoke();
                CloseEmail(true, false);
                return;
            }
            
            OpenMinigame();
            _onSuccessAcceptTask?.Invoke();
        }

        public void OnClickReject()
        {
            Debug.Log($"OnClickReject");
            
            if (_emailData == null)
            {
                return;
            }
            if (!_emailData.EmailData.IsSpam)
            {
                _onPenaltyRejectTask?.Invoke();
                CloseEmail(true, false);
                return;
            }
            
            _onSuccessRejectSpam?.Invoke();
            CloseEmail(true, true);
        }

        #endregion
        
        #region Minigame

        public void OpenMinigame()
        {
            SceneManager.LoadScene(_emailData.EmailData.MinigameSceneName, LoadSceneMode.Additive);
        }

        public void CloseMinigame()
        {
            try
            {
                SceneManager.UnloadSceneAsync(_emailData.EmailData.MinigameSceneName);
            }
            catch
            {
                // ignored
            }
        }
        
        public void OnMinigameSuccess()
        {
            // Debug.Log($"UiEmailViewerController OnMinigameSuccess");
            
            _onMinigameSuccessCallback?.Invoke();
            
            // CloseEmail(true);
        }

        public void OnMinigameFailed()
        {
            // Debug.Log($"UiEmailViewerController OnMinigameFailed");
            
            _onMinigameFailedCallback?.Invoke();
            
            // CloseEmail();
        }

        public void OnMinigameEnded()
        {
            _onMinigameEndedCallback?.Invoke();
            
            CloseEmail(_lastMinigameSuccess.Value, _lastMinigameSuccess.Value);
        }
        
        #endregion
    }
}