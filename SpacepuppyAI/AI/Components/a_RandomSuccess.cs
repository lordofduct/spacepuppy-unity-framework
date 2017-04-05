using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.AI.Components
{
    public class a_RandomSuccess : AIActionComponent
    {

        [SerializeField()]
        [Range(0f, 1f)]
        private float _oddsOfSuccess = 0.5f;

        public float Odds
        {
            get { return _oddsOfSuccess; }
            set { _oddsOfSuccess = Mathf.Clamp01(value); }
        }

        protected override ActionResult OnStart(IAIController ai)
        {
            return RandomUtil.Standard.Bool(_oddsOfSuccess) ? ActionResult.Success : ActionResult.Failed;
        }


    }
}
