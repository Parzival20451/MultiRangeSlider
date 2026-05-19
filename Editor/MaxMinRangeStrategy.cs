using UnityEditor;
using UnityEngine;
using MultiRangeSlider;

namespace MultiRangeSlider.Editor
{
	public class MaxMinRangeStrategy : MultiRangeStrategyBase
	{
		private const bool IS_INVERTED = true;

		public MaxMinRangeStrategy(SerializedProperty property, MultiRangeAttribute attribute) : base(property, attribute) { }

		protected override void ReadVector2Values(SerializedPropertyType propertyType, out string relativeMin, out string relativeMax, out RangeValue rangeValue)
		{
			if (IS_INVERTED)
			{
				relativeMax = "x";
				relativeMin = "y";

				if (propertyType == SerializedPropertyType.Vector2)
				{
					Vector2 value = property.vector2Value;
					rangeValue = new RangeValue(value.y, value.x);
				}
				else
				{
					Vector2Int value = property.vector2IntValue;
					rangeValue = new RangeValue(value.y, value.x);
				}
			}
			else
			{
				relativeMin = "x";
				relativeMax = "y";

				if (propertyType == SerializedPropertyType.Vector2)
				{
					Vector2 value = property.vector2Value;
					rangeValue = new RangeValue(value.x, value.y);
				}
				else
				{
					Vector2Int value = property.vector2IntValue;
					rangeValue = new RangeValue(value.x, value.y);
				}
			}
		}

		public override bool ClampValues(ref PropertyInfo propertyInfo)
		{
			if (propertyInfo.rangeValue.min < attribute.rightLimit)
			{
				propertyInfo.rangeValue.min = attribute.rightLimit;
				return true;
			}
			else if (propertyInfo.rangeValue.min > attribute.leftLimit)
			{
				propertyInfo.rangeValue.min = attribute.leftLimit;
				return true;
			}

			if (propertyInfo.rangeValue.max < attribute.rightLimit)
			{
				propertyInfo.rangeValue.max = attribute.rightLimit;
				return true;
			}
			else if (propertyInfo.rangeValue.max > attribute.leftLimit)
			{
				propertyInfo.rangeValue.max = attribute.leftLimit;
				return true;
			}

			return false;
		}

		public override void CalculateSliderLeftAndRight(PropertyInfo propertyInfo, out RangeValue sliderRangeValue, out RangeValue sliderLimitRangeValue)
		{
			//converting property max/min to slider min/max, because MinMaxSlider works only from min to max
			sliderRangeValue.min = attribute.leftLimit + (attribute.rightLimit - propertyInfo.rangeValue.max);
			sliderRangeValue.max = attribute.rightLimit - (propertyInfo.rangeValue.min - attribute.leftLimit);

			sliderRangeValue.min = Mathf.Clamp(sliderRangeValue.min, attribute.rightLimit, sliderRangeValue.max);
			sliderRangeValue.max = Mathf.Clamp(sliderRangeValue.max, sliderRangeValue.min, attribute.leftLimit);

			sliderLimitRangeValue.min = attribute.rightLimit;
			sliderLimitRangeValue.max = attribute.leftLimit;
		}

		public override void ReadSliderValues(ref RangeValue updatedRangeValue, RangeValue sliderRangeValue)
		{
			//converting slider min/max back to property max/min
			updatedRangeValue.max = attribute.rightLimit - (sliderRangeValue.min - attribute.leftLimit);
			updatedRangeValue.min = attribute.leftLimit + (attribute.rightLimit - sliderRangeValue.max);
		}
	}
}