using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using com.spacepuppy.Utils;

namespace com.spacepuppyeditor
{
    public static class AssetHelper
    {

        public static string GetRelativeResourcePath(string spath)
        {
            if (string.IsNullOrEmpty(spath)) return string.Empty;

            int i = spath.IndexOf("Resources") + 9;
            if (i >= spath.Length) return string.Empty;

            spath = spath.Substring(i);
            spath = Path.Combine(Path.GetDirectoryName(spath), Path.GetFileNameWithoutExtension(spath)).Replace(@"\", "/");
            return spath.EnsureNotStartWith("/");
        }

    }
}
