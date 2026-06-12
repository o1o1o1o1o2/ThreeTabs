using UnityEngine;

namespace Client.Libs.UI.Views
{
    public class UIScreenLockView : MonoBehaviour
    {
        [SerializeField] private GameObject _locker;

        public void SetLockerVisible(bool v)
        {
            if (_locker)
                _locker.SetActive(v);
        }
    }
}