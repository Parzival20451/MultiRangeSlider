using System;
using UnityEngine;

namespace MultiRangeSlider
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class MultiRangeAttribute : PropertyAttribute
	{
		public float leftLimit;
		public float rightLimit;

		public MultiRangeAttribute(float leftLimit, float rightLimit)
		{
			this.leftLimit = leftLimit;
			this.rightLimit = rightLimit;
		}

		public bool IsInverted => leftLimit > rightLimit;
	}
}