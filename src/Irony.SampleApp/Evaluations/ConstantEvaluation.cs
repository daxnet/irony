using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Irony.SampleApp.Evaluations
{
    internal sealed class ConstantEvaluation : Evaluation
    {
        private readonly object value;

        public ConstantEvaluation(object value)
        {
            this.value = value;
        }

        public override object Value => value;
    }
}
