using BML.ScriptableObjectCore.Scripts.Variables;
using UnityEngine;
using Sirenix.OdinInspector;

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
            Debug.Log("Win");
        }

        private void OnStrikeCountUpdate(int prevVal, int newVal) {
            if(newVal > _maxStrikeCount) {
                Debug.Log("Lose");
            }
        }

        private void OnUnreadEmailCountUpdate(int prevVal, int newVal) {
            if(newVal > _maxUnreadEmailCount) {
                Debug.Log("Lose");
            }
        }
    }
}
