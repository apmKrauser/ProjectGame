using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SimpleGraphicsLib
{
    [DataContract]
    [KnownType("GetDerivedTypes")]
    public class AnimationRigidBody :  GFXAnimation, IAnimationRigidBody
    {

        public event Action<IAnimationRigidBody> OnDispose;
        private IRigidBody _sprite;

        [DataMember]
        public string Name { get; set; }


        [XmlIgnore]
        public IRigidBody Sprite
        {
            get { return _sprite; }
            set
            {
                _sprite = value;
                IAnimationOnDispose ani = (value as IAnimationOnDispose);
                if (ani != null) OnDispose += ani.Animation_OnDispose;
            }
        }
       
        public override void Dispose() 
        {
            base.Dispose();
            if (OnDispose != null) OnDispose(this);
        }

        public AnimationRigidBody()
        {    }

        public AnimationRigidBody(IRigidBody _sprite)
        {
            Sprite = _sprite;
        }

        // tiggered by GFXContainer
        public AnimationRigidBody(IRigidBody _sprite, TimingSource.Sources _tsrc)
        {
            Sprite = _sprite;
            SetTimingSource(_tsrc);
        }

        public AnimationRigidBody(IRigidBody _sprite, GFXContainer _GFXContainer)
        {
            Sprite = _sprite;
            SetTimingSource(_GFXContainer);
        }

        public static IEnumerable<Type> GetDerivedTypes()
        {
            var types = from t in Assembly.GetExecutingAssembly().GetTypes()
                        where t.IsSubclassOf(typeof(AnimationRigidBody))
                        select t;
            return types;
        }

        [OnDeserializing]
        protected void OnDeserializing(StreamingContext context)
        {
            
        }
    }
}
