using UnityEditor;
using UnityEngine;

namespace MultiRangeSlider.Editor
{
	public class MinMaxRangeStrategy : MultiRangeStrategyBase
	{
		public MinMaxRangeStrategy(SerializedProperty property, MultiRangeAttribute attribute) : base(property, attribute) { }

		protected override void ReadVector2Values(SerializedPropertyType propertyType, out string relativeMin, out string relativeMax, out RangeValue rangeValue)
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

		public override bool ClampValues(ref PropertyInfo propertyInfo)
		{
			if (propertyInfo.rangeValue.min < attribute.leftLimit)
			{
				propertyInfo.rangeValue.min = attribute.leftLimit;
				return true;
			}
			else if (propertyInfo.rangeValue.min > attribute.rightLimit)
			{
				propertyInfo.rangeValue.min = attribute.rightLimit;
				return true;
			}

			if (propertyInfo.rangeValue.max < attribute.leftLimit)
			{
				propertyInfo.rangeValue.max = attribute.leftLimit;
				return true;
			}
			else if (propertyInfo.rangeValue.max > attribute.rightLimit)
			{
				propertyInfo.rangeValue.max = attribute.rightLimit;
				return true;
			}

			return false;
		}

		public override void CalculateSliderLeftAndRight(PropertyInfo propertyInfo, out RangeValue sliderRangeValue, out RangeValue sliderLimitRangeValue)
		{
			sliderRangeValue.min = propertyInfo.rangeValue.min;
			sliderRangeValue.max = propertyInfo.rangeValue.max;

			sliderRangeValue.min = Mathf.Clamp(sliderRangeValue.min, attribute.leftLimit, sliderRangeValue.max);
			sliderRangeValue.max = Mathf.Clamp(sliderRangeValue.max, sliderRangeValue.min, attribute.rightLimit);

			sliderLimitRangeValue.min = attribute.leftLimit;
			sliderLimitRangeValue.max = attribute.rightLimit;
		}

		public override void ReadSliderValues(ref RangeValue updatedRangeValue, RangeValue sliderRangeValue)
		{
			updatedRangeValue.min = sliderRangeValue.min;
			updatedRangeValue.max = sliderRangeValue.max;
		}
	}
}