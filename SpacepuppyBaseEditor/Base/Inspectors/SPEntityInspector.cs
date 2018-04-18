using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;

namespace com.spacepuppyeditor.Base
{
    [CustomEditor(typeof(SPEntity), true)]
    public class SPEntityInspector : SPEditor
    {

        protected override void OnSPInspectorGUI()
        {
            this.EnsureHasRootTag();

            this.DrawDefaultInspector();
        }

        public void EnsureHasRootTag()
        {
            var ent = this.target as SPEntity;
            if (!ent.HasTag(SPConstants.TAG_ROOT)) ent.AddTag(SPConstants.TAG_ROOT);
        }

    }
}
