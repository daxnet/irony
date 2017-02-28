using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Irony.SampleApp.Evaluations
{
    internal abstract class Evaluation
    {
        public abstract object Value { get; }

        public override string ToString() => Value?.ToString();
    }
}
