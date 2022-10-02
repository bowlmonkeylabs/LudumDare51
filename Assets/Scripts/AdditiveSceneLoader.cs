using UnityEngine;
using UnityEngine.SceneManagement;
using BML.ScriptableObjectCore.Scripts.Events;
using Sirenix.OdinInspector;
using EmailInbox;

namespace BML.Scripts
{
    public class AdditiveSceneLoader : MonoBehaviour
    {
        [SerializeField, ReadOnly] private EmailItem _emailData;
        
        [SerializeField] private DynamicGameEvent _onOpenEmail;

        #region Unity lifecycle

        private void Awake()
        {
            _emailData = null;
            _onOpenEmail.Subscribe(OnOpenEmailDynamic);
        }

        private void OnDestroy()
        {
            _onOpenEmail.Unsubscribe(OnOpenEmailDynamic);
        }

        #endregion

        private void OnOpenEmailDynamic(object prevValue, object currentValue)
        {
            var curr = currentValue as EmailItem;
            OnOpenEmail(curr);
        }

        private void OnOpenEmail(EmailItem emailItem)
        {
            _emailData = emailItem;
        }

        public void LoadScene()
        {
            if(_emailData == null) {
                return;
            }

            SceneManager.LoadScene(_emailData.MinigameScene.name, LoadSceneMode.Additive);
        }

        public void UnloadScene()
        {
            if(_emailData == null) {
                return;
            }

            SceneManager.UnloadSceneAsync(_emailData.MinigameScene.name);
        }
    }
}