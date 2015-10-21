using System.Collections.Generic;

namespace com.spacepuppy.Scenes
{
    public abstract class SceneContainer : Scene
    {

        #region Fields

        #endregion

        #region CONSTRUCTOR

        public SceneContainer(string id) : base(id)
        {
        }

        #endregion

        #region Scene Interface

        protected internal override void SetLoaded(bool loaded)
        {
            base.SetLoaded(loaded);

            for (int i = 0; i < this.ChildCount; i++)
            {
                this.GetChild(i).SetLoaded(loaded);
            }
        }

        protected internal override void DoLoad()
        {
            for (int i = 0; i < this.ChildCount; i++)
            {
                if (i == 0)
                    this.GetChild(i).DoLoad();
                else
                    this.GetChild(i).DoLoadAdditive();
            }
        }

        protected internal override IProgressingYieldInstruction DoLoadAsync()
        {
            var arr = new IProgressingYieldInstruction[this.ChildCount];
            for (int i = 0; i < this.ChildCount; i++)
            {
                if (i == 0)
                    arr[i] = this.GetChild(i).DoLoadAsync();
                else
                    arr[i] = this.GetChild(i).DoLoadAdditiveAsync();
            }
            return new ProgressingYieldInstructionQueue(arr);
        }

        protected internal override void DoLoadAdditive()
        {
            for (int i = 0; i < this.ChildCount; i++)
            {
                this.GetChild(i).DoLoadAdditive();
            }
        }

        protected internal override IProgressingYieldInstruction DoLoadAdditiveAsync()
        {
            var arr = new IProgressingYieldInstruction[this.ChildCount];
            for (int i = 0; i < this.ChildCount; i++)
            {
                arr[i] = this.GetChild(i).DoLoadAdditiveAsync();
            }
            return new ProgressingYieldInstructionQueue(arr);
        }

        #endregion

        #region SceneContainer Interface

        public abstract int ChildCount { get; }

        public abstract Scene GetChild(int index);

        public IEnumerable<Scene> Find(string id)
        {
            if (this.Id == id) yield return this;

            for(int i = 0; i < this.ChildCount; i++)
            {
                var scene = this.GetChild(i);
                if (scene.Id == id) yield return scene;
                if (scene is SceneContainer)
                {
                    foreach (var subscene in (scene as SceneContainer).Find(id)) yield return subscene;
                }
            }
        }

        public IEnumerable<Scene> GetAllChildren()
        {
            for (int i = 0; i < this.ChildCount; i++)
            {
                var scene = this.GetChild(i);
                yield return scene;
                if (scene is SceneContainer)
                {
                    foreach (var subscene in (scene as SceneContainer).GetAllChildren()) yield return subscene;
                }
            }
        }

        #endregion

    }
}
