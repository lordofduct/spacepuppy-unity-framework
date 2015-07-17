using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppyeditor
{
    public interface IArrayHandlingPropertyDrawer
    {

        PropertyDrawer InternalDrawer { get; set; }

    }
}
