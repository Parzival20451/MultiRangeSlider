namespace MultiRangeSlider
{
	[System.Serializable]
	public struct RangeValue
	{
		public float min;
		public float max;

		public RangeValue(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		public static implicit operator RangeValue(RangeValueInt rangeValueInt)
		{
			return new RangeValue(rangeValueInt.min, rangeValueInt.max);
		}
	}

	[System.Serializable]
	public struct RangeValueInt
	{
		public int min;
		public int max;

		public RangeValueInt(int min, int max)
		{
			this.min = min;
			this.max = max;
		}
	}
}