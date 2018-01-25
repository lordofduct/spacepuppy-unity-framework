using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.SPInput
{

    public class ComboInputSignature : BaseInputSignature, IInputSignature
    {

        #region Fields

        #endregion

        #region CONSTRUCTOR

        public ComboInputSignature(string id)
            :base(id)
        {
            this.Precedence = float.PositiveInfinity;
        }
        
        #endregion

        #region Methods

        #endregion

        #region IInputSignature Interface

        public override void Update()
        {



        }

        #endregion

    }

}
