using System;
using BML.ScriptableObjectCore.Scripts.Events;
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

        [SerializeField, ReadOnly] private EmailItem _emailData;
        
        [SerializeField] private DynamicGameEvent _onOpenEmail;
        
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
        }

        private void OnDestroy()
        {
            _onOpenEmail.Unsubscribe(OnOpenEmailDynamic);
        }

        #endregion

        #region Emails

        private void OnOpenEmailDynamic(object prevValue, object currentValue)
        {
            var curr = currentValue as EmailItem;
            OnOpenEmail(curr);
        }

        private void OnOpenEmail(EmailItem emailItem)
        {
            _emailData = emailItem;
            RenderEmailData();
        }

        private void RenderEmailData()
        {
            var showThisObject = (_emailData != null);
            _parentCanvas.enabled = showThisObject;
            
            if (!showThisObject) return;

            _textFromAddress.text = _emailData.FromAddress;
            _textSubject.text = _emailData.Subject;
            _textBody.text = _emailData.Body;
        }

        public void CloseEmail()
        {
            _emailData = null;
            RenderEmailData();
        }

        public void OnClickAccept()
        {
            if (_emailData == null)
            {
                return;
            }
            if (_emailData.IsSpam)
            {
                _onPenaltyAcceptSpam?.Invoke();
                CloseEmail();
                return;
            }
            
            SceneManager.LoadScene(_emailData.MinigameScene.name, LoadSceneMode.Additive);
            _onSuccessAcceptTask?.Invoke();
        }

        public void OnClickReject()
        {
            if (_emailData == null)
            {
                return;
            }
            if (!_emailData.IsSpam)
            {
                _onPenaltyRejectTask?.Invoke();
                CloseEmail();
                return;
            }
            
            // TODO remove from inbox
            _onSuccessRejectSpam?.Invoke();
            CloseEmail();
        }

        public void OnMinigameSuccess()
        {
            _onMinigameSuccessCallback?.Invoke();
            
            // TODO remove from inbox
            
            CloseEmail();
        }

        public void OnMinigameFailed()
        {
            _onMinigameFailedCallback?.Invoke();
            
            // TODO remove from inbox
            
            CloseEmail();
        }

        #endregion
    }
}