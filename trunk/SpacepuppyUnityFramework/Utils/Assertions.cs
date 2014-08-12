using UnityEngine;
using System.Linq;

namespace com.spacepuppy.Utils
{

    public static class Assertions
    {

        public static void Assert(string msg)
        {
            //throw new System.Exception(msg);
            Debug.LogWarning(msg);
        }

        /// <summary>
        /// Throws error message if obj is not null.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool AssertNull(object obj, string msg)
        {
            if (obj != null)
            {
                Assert(msg);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Throws error message if obj is null.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool AssertNotNull(object obj, string msg)
        {
            if (obj == null)
            {
                Assert(msg);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Throws error message if index is less than 0 or greater than len - 1.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="len"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool AssertInBounds(int index, int len, string msg = "index out of range.")
        {
            if (index < 0 || index >= len)
            {
                Assert(msg);
                return true;
            }

            return false;
        }

        #region HasLikeComponent

        public static bool AssertHasLikeComponent(GameObject go, System.Type tp)
        {
            if (!go.HasLikeComponent(tp))
            {
                Assert("GameObject requires a component of type " + tp.Name);
                return true;
            }

            return false;
        }

        public static bool AssertRequireLikeComponentAttrib(Component comp)
        {
            System.Type missingCompType;

            return AssertRequireLikeComponentAttrib(comp, out missingCompType);
        }

        public static bool AssertRequireLikeComponentAttrib(Component comp, out System.Type missingCompType)
        {
            if (comp == null) throw new System.ArgumentNullException("comp");
            missingCompType = null;

            var tp = comp.GetType();
            foreach (var obj in tp.GetCustomAttributes(typeof(RequireLikeComponentAttribute), true))
            {
                RequireLikeComponentAttribute attrib = obj as RequireLikeComponentAttribute;
                foreach (var reqType in attrib.Types)
                {
                    if (!comp.HasLikeComponent(reqType))
                    {
                        missingCompType = reqType;
                        Assert(System.String.Format("Component type {0} requires the gameobject to also have a component of type {1}.", tp.Name, reqType.Name));
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool AssertRequireComponentInEntityAttrib(Component comp)
        {
            System.Type missingCompType;

            return AssertRequireComponentInEntityAttrib(comp, out missingCompType);
        }

        public static bool AssertRequireComponentInEntityAttrib(Component comp, out System.Type missingCompType)
        {
            if (comp == null) throw new System.ArgumentNullException("comp");
            missingCompType = null;

            var tp = comp.GetType();
            foreach (var obj in tp.GetCustomAttributes(typeof(RequireComponentInEntityAttribute), true))
            {
                RequireComponentInEntityAttribute attrib = obj as RequireComponentInEntityAttribute;
                foreach (var reqType in attrib.Types)
                {
                    if (!comp.EntityHasComponent(reqType))
                    {
                        missingCompType = reqType;
                        Assert(System.String.Format("Component type {0} requires the entity to also have a component of type {1}.", tp.Name, reqType.Name));
                        return true;
                    }
                }
            }

            return false;
        }


        public static bool AssertUniqueToEntityAttrib(Component comp)
        {
            var tp = comp.GetType();
            var attrib = tp.GetCustomAttributes(typeof(UniqueToEntityAttribute), false).FirstOrDefault() as UniqueToEntityAttribute;

            if (attrib != null)
            {
                if (attrib.MustBeAttachedToRoot)
                {
                    if (!comp.HasTag(SPConstants.TAG_ROOT))
                    {
                        Assert(System.String.Format("Component type {0} must be attached to the root gameObject.", tp.Name));
                        return true;
                    }
                }
                else
                {
                    var root = comp.FindRoot();

                    foreach (var child in root.GetAllChildrenAndSelf())
                    {
                        if (child != comp.transform && child.HasComponent(tp))
                        {
                            Assert(System.String.Format("Only one component of type {0} must be attached to a root or any of its children.", tp.Name));
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        #endregion

    }

}