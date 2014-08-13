using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppy
{

    public class RadicalAsyncOperation : IRadicalYieldInstruction
    {

        #region Fields

        private AsyncOperation _op;

        #endregion

        #region CONSTRUCTOR

        public RadicalAsyncOperation(AsyncOperation op)
        {
            if (op == null) throw new System.ArgumentNullException("op");
            _op = op;
        }

        #endregion

        #region Properties

        public AsyncOperation AsyncOperation { get { return _op; } }

        public virtual bool Complete { get { return _op.isDone; } }

        public float Progress { get { return _op.progress; } }

        #endregion

        #region Methods

        protected virtual object Update(RadicalCoroutine routine)
        {
            return null;
        }

        #endregion

        #region IRadicalYieldInstruction Interface

        private object _yieldObject;

        object IRadicalYieldInstruction.CurrentYieldObject
        {
            get { return _yieldObject; }
        }

        bool IRadicalYieldInstruction.ContinueBlocking(RadicalCoroutine routine)
        {
            _yieldObject = this.Update(routine);
            return !this.Complete;
        }

        #endregion


    }

    public class SceneLoadAsyncOperation : RadicalAsyncOperation
    {

        #region Fields

        private string _name;
        private GameObjectCollection _objects;

        #endregion

        #region CONSTRUCTOR

        public SceneLoadAsyncOperation(string name, AsyncOperation op)
            : base(op)
        {
            _name = name;
        }

        #endregion

        #region Properties

        public string SceneName { get { return _name; } }

        public GameObjectCollection GameObjects { get { return _objects; } }

        public override bool Complete
        {
            get
            {
                return _objects != null && this.AsyncOperation.isDone;
            }
        }

        #endregion

        #region Methods

        public void SetGameObjectCollection(GameObjectCollection coll)
        {
            _objects = coll;
        }

        #endregion

    }

}
