using Planetbase;
using Redirection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MoreManufacturingLimits
{
    public class GuiAmountSelectorImproved : GuiAmountSelector 
    {
		public GuiAmountSelectorImproved(int min, int max, RefInt value, GuiDefinitions.Callback changeCallback, int flags) 
            : base(min, max, 1, value, changeCallback, flags)
        {

        }


		[RedirectFrom(typeof(GuiAmountSelector))]
		public new void onPlus()
		{
			if (this.mCurrent.get() < this.mMax)
			{
				var shiftMultiplier = Event.current.shift ? 10 : 1;
				var ctrlMultiplier = Event.current.control ? 100 : 1;

				if (this.hasFlag(4)) // FlagProgressiveStep
				{
					this.mCurrent.set(this.mCurrent.get() + this.mStep * Math.Max(shiftMultiplier, ctrlMultiplier));
				}
				else
				{
					this.mCurrent.set(this.mCurrent.get() + this.mStep * shiftMultiplier);
				}

				// Check we're still below max

				if(this.mCurrent.get() > this.mMax)
                {
					this.mCurrent.set(this.mMax);
                }

				if (this.mChangeCallback != null)
				{
					this.mChangeCallback(this);
				}
			}			
			else if (this.mCurrent.get() == 2147483647 && this.hasFlag(8))  // FlagLoop
			{
				this.mCurrent.set(this.mMin);
			}
			else if (this.hasFlag(2)) // FlagHasInfinity
			{
				this.mCurrent.set(int.MaxValue);
				if (this.mChangeCallback != null)
				{
					this.mChangeCallback(this);
				}
			}
		}

		[RedirectFrom(typeof(GuiAmountSelector))]
		public new void onMinus()
		{
			if (this.mCurrent.get() == 2147483647)
			{
				this.mCurrent.set(this.mMax);
				if (this.mChangeCallback != null)
				{
					this.mChangeCallback(this);
				}
			}			
			else if (this.mCurrent.get() > this.mMin)
			{
				var shiftMultiplier = Event.current.shift ? 10 : 1;
				var ctrlMultiplier = Event.current.control ? 100 : 1;

				if (this.hasFlag(4)) // FlagProgressiveStep
				{
					this.mCurrent.set(this.mCurrent.get() - shiftMultiplier * ctrlMultiplier);
				}
				else
				{
					this.mCurrent.set(this.mCurrent.get() - this.mStep * shiftMultiplier);
				}

				if (this.mCurrent.get() < this.mMin)
				{
					this.mCurrent.set(this.mMin);
				}

				if (this.mChangeCallback != null)
				{
					this.mChangeCallback(this);
				}
			}			
			else if (this.mCurrent.get() == this.mMin && this.hasFlag(8)) // FlagLoop
			{
				this.mCurrent.set(int.MaxValue);
			}
		}		
	}
}
