using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Project;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Collections;

namespace com.spacepuppyeditor.Project
{

    [CustomPropertyDrawer(typeof(ResourceLinkTable), true)]
    public class ResourceLinkTablePropertyDrawer : DictionaryPropertyDrawer
    {

        private ResourceLinkPropertyDrawer _resourcePropertyDrawer = new ResourceLinkPropertyDrawer();

        protected override void DrawValue(Rect area, SerializedProperty valueProp)
        {
            var attrib = this.fieldInfo.GetCustomAttributes(typeof(ResourceLink.ConfigAttribute), false).FirstOrDefault() as ResourceLink.ConfigAttribute;
            _resourcePropertyDrawer.ManualConfig = true;
            _resourcePropertyDrawer.ResourceType = attrib != null ? attrib.resourceType : null;

            _resourcePropertyDrawer.OnGUI(area, valueProp, GUIContent.none);
        }

    }

}
