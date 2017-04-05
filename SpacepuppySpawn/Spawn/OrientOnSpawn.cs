using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{

    [RequireComponent(typeof(i_Spawner))]
    public class OrientOnSpawn : SPComponent, ISpawnerModifier
    {

        public enum TransformInheritance
        {
            None = 0,
            Relative = 2,
            Local = 3,
            Global = 4,
            Additive = 5
        }

        #region Fields

        public int order;
        [Infobox("'Relative' sets target transform relative to this in world space.\n'Local' sets target transform directly in its local space.\n'Global' sets target transform directly in world space.\n'Additive' adds to the target transform in world space.\n'None' does not effect the target transform.")]
        public TransformInheritance TranslationInheritance = TransformInheritance.Relative;
        public Vector3 Translation = Vector3.zero;
        public TransformInheritance RotationInheritance = TransformInheritance.Relative;
        [EulerRotationInspector()]
        public Quaternion Rotation = Quaternion.identity;
        public TransformInheritance ScaleInheritance = TransformInheritance.None;
        public Vector3 Scale = Vector3.zero;

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region ISpawnerModifier Interface

        int ISpawnerModifier.order
        {
            get { return this.order; }
        }

        void ISpawnerModifier.OnBeforeSpawnNotification(SpawnPointBeforeSpawnNotification n)
        {

        }

        void ISpawnerModifier.OnSpawnedNotification(SpawnPointTriggeredNotification n)
        {
            var go = n.SpawnedObject;
            switch (this.TranslationInheritance)
            {
                case TransformInheritance.Relative:
                    go.transform.position = this.transform.TransformPoint(this.Translation);
                    break;
                case TransformInheritance.Local:
                    go.transform.localPosition = this.Translation;
                    break;
                case TransformInheritance.Global:
                    go.transform.position = this.Translation;
                    break;
                case TransformInheritance.Additive:
                    go.transform.position += this.Translation;
                    break;
            }

            switch (this.RotationInheritance)
            {
                case TransformInheritance.Relative:
                    go.transform.rotation = this.transform.TransformRotation(this.Rotation);
                    break;
                case TransformInheritance.Local:
                    go.transform.localRotation = this.Rotation;
                    break;
                case TransformInheritance.Global:
                    go.transform.rotation = this.Rotation;
                    break;
                case TransformInheritance.Additive:
                    go.transform.rotation *= this.Rotation;
                    break;
            }

            switch (this.ScaleInheritance)
            {
                case TransformInheritance.Relative:
                    go.transform.localScale = this.Scale;
                    break;
                case TransformInheritance.Local:
                    go.transform.localScale = this.Scale;
                    break;
                case TransformInheritance.Global:
                    go.transform.localScale = this.Scale;
                    break;
                case TransformInheritance.Additive:
                    go.transform.localScale += this.Scale;
                    break;
            }
        }

        #endregion

    }
}
