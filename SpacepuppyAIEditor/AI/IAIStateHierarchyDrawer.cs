using UnityEngine;
using UnityEditor;

using com.spacepuppy;
using com.spacepuppy.AI;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.AI.Components
{

    [CustomHierarchyDrawer(typeof(IAIState))]
    public class IAIStateHierarchyDrawer : HierarchyDrawer
    {


        private Color _activeColor = Color.blue.SetAlpha(0.1f);
        private Color _inactiveColor = Color.red.SetAlpha(0.1f);

        public override void OnHierarchyGUI(Rect selectionRect)
        {
            if (!Application.isPlaying) return;

            var targ = this.Target as IAIState;
            if (targ == null) return;

            if(targ.IsActive)
            {
                EditorGUI.DrawRect(selectionRect, _activeColor);
            }
            else
            {
                EditorGUI.DrawRect(selectionRect, _inactiveColor);
            }
        }

    }
}
