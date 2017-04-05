using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.Components
{
    public class a_NullAction : AIActionComponent
    {

        [SerializeField()]
        private bool _fail;

        public bool FailOnTick
        {
            get { return _fail; }
            set { _fail = value; }
        }

        protected override ActionResult OnStart(IAIController ai)
        {
            return (_fail) ? ActionResult.Failed : ActionResult.Success;
        }

    }
}
