using System.Threading;
using AE.Core.Interfaces;
using AE.Interfaces;
using UnityEngine;

namespace AE.Core.Generics
{

    /// <summary>
    /// Class that implements custom initialization pipeline
    /// </summary>
    public abstract class InGameMonoBehaviour : MonoBehaviour, IInitializable
    {
        public bool IsInitialized { get; private set; }
        
        protected CancellationToken _OnDestoryCancellationToken;

        private void Start()
        {
            Initialize();
        }


        public void Initialize()
        {
            if (IsInitialized) return;

            if (this is IAttachListeners attachListeners)
                attachListeners.AttachListeners();
            
            if (this is IWithSetUp setup)
                setup.SetUp();
            if(this is IHasSetBaseState setBase)
                setBase.UseCaller();

            _OnDestoryCancellationToken = this.destroyCancellationToken;
            
            IsInitialized = true;
        }

        protected virtual void CleanUp()
        {
        }

        private void OnDestroy()
        {
            CleanUp();

            if (this is IAttachListeners attachListeners)
                attachListeners.DetachListeners();

            if (this is IWithSetUp setup)
                setup.TearDown();

        }


    }
}
