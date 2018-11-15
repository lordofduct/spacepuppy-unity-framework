using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor
{

    /*
     * TODO
     * 
    public static class ExecutionOrderHelper
    {
        
        private const string PB_TITLE = "Updating Execution Order";
        private const string ERR_MESSAGE = "Unable to locate and set execution order for {0}";

        [InitializeOnLoadMethod]
        private static void OnLoad()
        {
            if (EditorUtility.DisplayCancelableProgressBar(PB_TITLE, "Scanning for types", 0f)) return;

            var table = new Dictionary<System.Type, ExecutionOrderAttribute>();
            
            foreach(var tp in TypeUtil.GetTypesAssignableFrom(typeof(Component)))
            {
                var attrib = tp.GetCustomAttributes(typeof(ExecutionOrderAttribute), false).FirstOrDefault() as ExecutionOrderAttribute;
                if (attrib == null) continue;

                table[tp] = attrib;
            }
            
            if (EditorUtility.DisplayCancelableProgressBar(PB_TITLE, "Scanning for inherited types.", 0.05f)) return;

            foreach (var pair in table)
            {
                if(pair.Value.Inherited)
                {
                    foreach(var tp in TypeUtil.GetTypesAssignableFrom(pair.Key))
                    {
                        if (!table.ContainsKey(tp)) table[tp] = new ExecutionOrderAttribute(pair.Value.Order);
                    }
                }
            }

        }
        
    }

    */

}
