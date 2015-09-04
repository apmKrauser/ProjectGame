using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGraphicsLib
{
    [DataContract]
    public class AnimationMotor
    {

        [OnDeserializing]
        protected void OnDeserializing(StreamingContext context)
        {        }
    }
}
