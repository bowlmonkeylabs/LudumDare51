using BML.ScriptableObjectCore.Scripts.Variables;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace BML.Scripts {
    public class WorldManager : MonoBehaviour
    {
        [TitleGroup("Max Counts")]
        [SerializeField] private int _maxStrikeCount = 3;
        [SerializeField] private int _maxUnreadEmailCount = 10;

        [TitleGroup("Scriptable Object Vars")]
        [SerializeField] private TimerVariable _gameTimer;
        [SerializeField] private IntVariable _strikeCount;
        [SerializeField] private IntVariable _unreadEmailCount;

        [TitleGroup("Win Lose Events")]
        [SerializeField] private UnityEvent _onWin;
        [SerializeField] private UnityEvent _onLoseStrikes;
        [SerializeField] private UnityEvent _onLoseInbox;

        void Update() {
            _gameTimer.UpdateTime();
        }

        void OnEnable() {
            _strikeCount.Subscribe(OnStrikeCountUpdate);
            _unreadEmailCount.Subscribe(OnUnreadEmailCountUpdate);
            _gameTimer.SubscribeFinished(OnGameTimeEnd);

            _gameTimer.StartTimer();
        }

        void OnDisable() {
            _strikeCount.Unsubscribe(OnStrikeCountUpdate);
            _unreadEmailCount.Unsubscribe(OnUnreadEmailCountUpdate);
            _gameTimer.UnsubscribeFinished(OnGameTimeEnd);

            _gameTimer.ResetTimer();
        }

        private void OnGameTimeEnd() {
            _onWin.Invoke();
        }

        private void OnStrikeCountUpdate(int prevVal, int newVal) {
            if(newVal >= _maxStrikeCount) {
                _onLoseStrikes.Invoke();
            }
        }

        private void OnUnreadEmailCountUpdate(int prevVal, int newVal) {
            if(newVal >= _maxUnreadEmailCount) {
                _onLoseInbox.Invoke();
            }
        }
    }
}
