using System.Threading;
using AE._Project.Scripts.Core.Interfaces;
using AE._Project.Scripts.Interfaces;
using UnityEngine;

namespace AE._Project.Scripts.Core.Generics
{
    /// <summary>
    ///     Class that implements custom initialization pipeline
    /// </summary>
    public abstract class InGameMonoBehaviour : MonoBehaviour, IInitializable
    {
        protected CancellationToken OnDestoryCancellationToken;

        private void Start()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            CleanUp();

            if (this is IAttachListeners attachListeners)
            {
                attachListeners.DetachListeners();
            }

            if (this is IWithSetUp setup)
            {
                setup.TearDown();
            }
        }

        public bool IsInitialized { get; private set; }


        public void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }

            if (this is IAttachListeners attachListeners)
            {
                attachListeners.AttachListeners();
            }

            if (this is IWithSetUp setup)
            {
                setup.SetUp();
            }

            if (this is IHasSetBaseState setBase)
            {
                setBase.UseCaller();
            }

            OnDestoryCancellationToken = destroyCancellationToken;

            IsInitialized = true;
        }

        protected virtual void CleanUp()
        {
        }
    }
}