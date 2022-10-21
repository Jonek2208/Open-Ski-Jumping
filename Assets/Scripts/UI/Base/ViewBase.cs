using System;
using UnityEngine;

namespace OpenSkiJumping.UI.Base
{
    public interface IViewBase
    {
        event Action OnDataSave;
        event Action OnDataReload;
    }
 public abstract class ViewBase : MonoBehaviour, IViewBase
    {
        public event Action OnDataSave;
        public event Action OnDataReload;

        private bool _initialized;
        protected abstract void Initialize();

        private void OnEnable()
        {
            if (!_initialized) return;
            OnDataReload?.Invoke();
        }

        private void OnDisable()
        {
            OnDataSave?.Invoke();
        }

        private void Start()
        {
            Initialize();
            _initialized = true;
            OnDataReload?.Invoke();
        }

    }
}