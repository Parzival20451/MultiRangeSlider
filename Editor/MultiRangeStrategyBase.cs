using UnityEditor;
using UnityEngine;

namespace MultiRangeSlider.Editor
{
	public enum PropertyNumericType
	{
		Float, Int
	}

	public struct PropertyInfo
	{
		public string relativeNameMin, relativeNameMax;
		public RangeValue rangeValue;
		public PropertyNumericType numericType;
	}

	public abstract class MultiRangeStrategyBase
	{
		//visualisation properties
		private const float FIELD_WIDTH = 50f;
		private const float FIELD_GAP = 5f;
		private const float SLIDER_MIN_WIDTH = 12f;
		private const bool IS_END_ANCHOR = false;

		protected readonly SerializedProperty property;
		protected readonly MultiRangeAttribute attribute;

		public MultiRangeStrategyBase(SerializedProperty property, MultiRangeAttribute attribute)
		{
			this.property = property;
			this.attribute = attribute;
		}

		public virtual bool TryGetPropertyInfo(out PropertyInfo propertyInfo)
		{
			SerializedPropertyType propertyType = property.propertyType;

			if (propertyType == SerializedPropertyType.Generic)
			{
				if (!TryReadRangeValue(out propertyInfo))
				{
					return false;
				}
			}
			else if (propertyType == SerializedPropertyType.Vector2 || propertyType == SerializedPropertyType.Vector2Int)
			{
				propertyInfo.numericType = propertyType == SerializedPropertyType.Vector2 ? PropertyNumericType.Float : PropertyNumericType.Int;

				ReadVector2Values(propertyType, out propertyInfo.relativeNameMin, out propertyInfo.relativeNameMax, out propertyInfo.rangeValue);
			}
			else
			{
				propertyInfo = default;
				return false;
			}

			return true;
		}

		private bool TryReadRangeValue(out PropertyInfo propertyInfo)
		{
			if (property.boxedValue is RangeValue rangeValue)
			{
				propertyInfo.numericType = PropertyNumericType.Float;
				propertyInfo.rangeValue = rangeValue;
			}
			else if (property.boxedValue is RangeValueInt rangeValueInt)
			{
				propertyInfo.numericType = PropertyNumericType.Int;
				propertyInfo.rangeValue = rangeValueInt;
			}
			else
			{
				propertyInfo = default;
				return false;
			}

			propertyInfo.relativeNameMin = "min";
			propertyInfo.relativeNameMax = "max";
			return true;
		}

		protected abstract void ReadVector2Values(SerializedPropertyType propertyType, out string relativeMin, out string relativeMax, out RangeValue rangeValue);
		
		public abstract void CalculateSliderLeftAndRight(PropertyInfo propertyInfo, out RangeValue sliderRangeValue, out RangeValue sliderLimitRangeValue);

		public abstract void ReadSliderValues(ref RangeValue updatedRangeValue, RangeValue sliderRangeValue);

		public abstract bool ClampValues(ref PropertyInfo updatedPropertyInfo);

		public virtual void CalculateRects(Rect position, GUIContent label, out Rect remainingRect, out Rect leftFieldRect, out Rect sliderRect, out Rect rightFieldRect)
		{
			remainingRect = EditorGUI.PrefixLabel(position, label);

			leftFieldRect = remainingRect;
			leftFieldRect.x = remainingRect.x;
			leftFieldRect.width = FIELD_WIDTH;

			sliderRect = remainingRect;
			sliderRect.x += FIELD_WIDTH + FIELD_GAP;
			sliderRect.width = Mathf.Max(remainingRect.width - ((FIELD_WIDTH + FIELD_GAP) * 2), SLIDER_MIN_WIDTH);

			rightFieldRect = remainingRect;
			//This block is for whether you want last field to be anchored to the right side of container or to the left
			if (IS_END_ANCHOR)
			{
				rightFieldRect.x = (remainingRect.x + remainingRect.width) - FIELD_WIDTH;
			}
			else
			{
				rightFieldRect.x += FIELD_WIDTH + FIELD_GAP + sliderRect.width + FIELD_GAP;
			}
			rightFieldRect.width = FIELD_WIDTH;
		}
	}
}