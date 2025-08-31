using System;
using UnityEngine;

namespace EasyTweens
{
    [Serializable]
    public class TweenLocalRotation : Vector3Tween<Transform>
    {
        [ExposeInEditor]
        public bool skipX;
        [ExposeInEditor]
        public bool skipY;
        [ExposeInEditor]
        public bool skipZ;

        protected override Vector3 Property
        {
            get => target.localRotation.eulerAngles;
            set
            {
                Vector3 newAngles = value;

                if (skipX) newAngles.x = target.localRotation.x;
                if (skipY) newAngles.y = target.localRotation.y;
                if (skipZ) newAngles.z = target.localRotation.z;

                target.localRotation = Quaternion.Euler(newAngles);
            }
        }
    }
}
