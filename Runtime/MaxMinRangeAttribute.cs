using System;
using UnityEngine;

namespace MultiRangeSlider
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class MaxMinRangeAttribute : PropertyAttribute
	{
		public float max;
		public float min;

		public MaxMinRangeAttribute(float max, float min)
		{
			this.max = max;
			this.min = min;
		}
	}
}