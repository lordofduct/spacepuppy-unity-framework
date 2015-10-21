using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppy.Scenes
{
    public class CompositeScene : SceneContainer, ICollection<Scene>
    {

        #region Fields

        private UniqueList<Scene> _scenes = new UniqueList<Scene>() { MoveDuplicatesToTopOnAdd = true };

        #endregion
        
        #region CONSTRUCTOR

        public CompositeScene(string id)
            : base(id)
        {
        }

        #endregion

        #region Properties

        public Scene PrimaryScene
        {
            get { return _scenes.FirstOrDefault(); }
        }

        #endregion

        #region Methods

        public void SetPrimaryScene(Scene scene)
        {
            if (scene == null) throw new System.ArgumentNullException("scene");
            if (_scenes.Contains(scene)) _scenes.Remove(scene);

            _scenes.Insert(0, scene);
        }

        public Scene GetSceneById(string id)
        {
            for (int i = 0; i < _scenes.Count; i++)
            {
                if (_scenes[i].Id == id) return _scenes[i];
            }
            return null;
        }

        #endregion

        #region ICollection Interface

        public void Add(Scene scene)
        {
            if (scene == null) throw new System.ArgumentNullException("scene");
            if (this.Loaded) throw new ModifiedLoadedSceneContainerException();
            _scenes.Add(scene);
        }

        public void Clear()
        {
            if (this.Loaded) throw new ModifiedLoadedSceneContainerException();
            _scenes.Clear();
        }

        public bool Contains(Scene scene)
        {
            return _scenes.Contains(scene);
        }

        void ICollection<Scene>.CopyTo(Scene[] array, int arrayIndex)
        {
            _scenes.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _scenes.Count; }
        }

        bool ICollection<Scene>.IsReadOnly
        {
            get { return this.Loaded; }
        }

        public bool Remove(Scene scene)
        {
            if (this.Loaded) throw new ModifiedLoadedSceneContainerException();
            return _scenes.Remove(scene);
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return _scenes.GetEnumerator();
        }

        IEnumerator<Scene> IEnumerable<Scene>.GetEnumerator()
        {
            return _scenes.GetEnumerator();
        }

        #endregion

        #region SceneContainer Interface

        public override int ChildCount
        {
            get { return _scenes.Count; }
        }

        public override Scene GetChild(int index)
        {
            return _scenes[index];
        }
       
        #endregion

    }
}
