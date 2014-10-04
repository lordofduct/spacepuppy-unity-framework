using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{

    [AddComponentMenu("SpacePuppy/Spawn/Spawn Point")]
    public class SpawnPoint : AbstractSpawnPoint
    {

        public const string TRIGGERARG_RANDOM = "RANDOM";

        #region Fields

        [SerializeField()]
        [Tooltip("Objects available for spawning. When spawn is called with no arguments a prefab is selected at random, unless a ISpawnPointSelectionModifier is available on the SpawnPoint.")]
        private GameObject[] _prefabs = new GameObject[] { };

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();
        }

        #endregion

        #region Properties

        public GameObject[] Prefabs
        {
            get { return _prefabs; }
            set { _prefabs = value; }
        }

        public int PrefabCount
        {
            get { return (_prefabs != null) ? _prefabs.Length : 0; }
        }

        #endregion

        #region AbstractSpawnPoint Abstract Interface

        protected override GameObject[] GetAvailablePrefabs()
        {
            return _prefabs;
        }

        #endregion

        #region Methods

        public GameObject Spawn(int index)
        {
            if (!this.enabled) return null;
            if (this.Prefabs == null || index < 0 || index >= this.Prefabs.Length) return null;
            return this.Spawn(this.Prefabs[index]);
        }

        public GameObject Spawn(string name)
        {
            if (!this.enabled) return null;
            if (this.Prefabs == null) return null;
            for (int i = 0; i < this.Prefabs.Length; i++)
            {
                if (this.Prefabs[i].name == name) return this.Spawn(this.Prefabs[i]);
            }
            return null;
        }

        #endregion

        #region ITriggerable Interface

        public override bool CanTrigger
        {
            get { return base.CanTrigger && _prefabs != null && _prefabs.Length > 0; }
        }

        public override object Trigger(object arg)
        {
            if (!this.CanTrigger) return null;

            if (arg is string)
            {
                switch ((arg as string).ToUpper())
                {
                    case TRIGGERARG_RANDOM:
                        return this.Spawn(Random.Range(0, this.PrefabCount));
                    default:
                        return this.Spawn(arg as string);
                }
            }
            else if (ConvertUtil.ValueIsNumericType(arg))
            {
                return this.Spawn(ConvertUtil.ToInt(arg));
            }
            else
            {
                return this.Spawn();
            }
        }

        #endregion

    }
}