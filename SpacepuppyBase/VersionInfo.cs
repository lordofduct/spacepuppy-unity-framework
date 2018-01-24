using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace com.spacepuppy
{
    
    public struct VersionInfo : IComparable<VersionInfo>
    {

        #region Fields

        public int Major;
        public int Minor;
        public int Patch;
        public int Build;

        #endregion

        #region CONSTRUCTOR

        public VersionInfo(int major, int minor = 0, int patch = 0, int build = 0)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Build = build;
        }

        #endregion

        #region Methods

        public int CompareTo(VersionInfo other)
        {
            if (Major < other.Major) return -1;
            if (Major > other.Major) return +1;
            if (Minor < other.Minor) return -1;
            if (Minor > other.Minor) return +1;
            if (Patch < other.Patch) return -1;
            if (Patch > other.Patch) return +1;
            if (Build < other.Build) return -1;
            if (Build > other.Build) return +1;
            return 0;
        }

        public override string ToString()
        {
            if (this.Build == 0)
                return string.Format("{0}.{1}.{2}", Major, Minor, Patch);
            else
                return string.Format("{0}.{1}.{2} build {3}", Major, Minor, Patch, Build);
        }

        public override bool Equals(object other)
        {
            if (other is VersionInfo)
                return this == (VersionInfo)other;
            else
             return false;
        }
        
        public override int GetHashCode()
        {
            return Major.GetHashCode() ^ Minor.GetHashCode() ^ Patch.GetHashCode() ^ Build.GetHashCode();
        }

        #endregion

        #region Operators

        public static bool operator ==(VersionInfo a, VersionInfo b)
        {
            return a.CompareTo(b) == 0;
        }


        public static bool operator !=(VersionInfo a, VersionInfo b)
        {
            return a.CompareTo(b) != 0;
        }


        public static bool operator <=(VersionInfo a, VersionInfo b)
        {
            return a.CompareTo(b) <= 0;
        }


        public static bool operator >=(VersionInfo a, VersionInfo b)
        {
            return a.CompareTo(b) >= 0;
        }


        public static bool operator <(VersionInfo a, VersionInfo b)
        {
            return a.CompareTo(b) < 0;
        }


        public static bool operator >(VersionInfo a, VersionInfo b)
        {
            return a.CompareTo(b) > 0;
        }

        #endregion

        #region Static Accessors

        private static VersionInfo? _unityVersion;
        public static VersionInfo UnityVersion
        {
            get
            {
                if(_unityVersion == null || !_unityVersion.HasValue)
                {
                    var m = Regex.Match(Application.unityVersion, @"^(\d+)\.(\d+)\.(\d+)");
                    var build = 0;
                    _unityVersion = new VersionInfo()
                    {
                        Major = Convert.ToInt32(m.Groups[1].Value),
                        Minor = Convert.ToInt32(m.Groups[2].Value),
                        Patch = Convert.ToInt32(m.Groups[3].Value),
                        Build = build
                    };
                }
                return _unityVersion.Value;
            }
        }

        #endregion

    }
}
