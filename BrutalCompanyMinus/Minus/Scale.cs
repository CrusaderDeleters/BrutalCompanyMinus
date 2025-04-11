using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BrutalCompanyMinus.Minus
{
    public struct Scale
    {
        public float Base, Increment, MinCap, MaxCap;

        public Scale(float Base, float Increment, float MinCap, float MaxCap)
        {
            this.Base = Base;
            this.Increment = Increment;
            this.MinCap = MinCap;
            this.MaxCap = MaxCap;
        }

        public static float Compute(Scale scale, MEvent.EventType Type = MEvent.EventType.Neutral)
        {
            float increment = scale.Increment;

            if (Type == MEvent.EventType.VeryBad || Type == MEvent.EventType.Bad) increment = scale.Increment * Configuration.badEventIncrementMultiplier.Value;
            if (Type == MEvent.EventType.VeryGood || Type == MEvent.EventType.Good) increment = scale.Increment * Configuration.goodEventIncrementMultiplier.Value;

            return Mathf.Clamp(scale.Base + (increment * Manager.difficulty), scale.MinCap, Configuration.ignoreMaxCap.Value ? 99999999999.0f : scale.MaxCap);
        }

        public float Computef(MEvent.EventType Type) => Compute(this, Type);

        public int Compute(MEvent.EventType Type) => (int)Compute(this, Type);
    }
}
