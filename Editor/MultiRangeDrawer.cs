using UnityEditor;
using UnityEngine;

namespace MultiRangeSlider.Editor
{
	[CustomPropertyDrawer(typeof(MultiRangeAttribute))]
	public class MultiRangeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			MultiRangeAttribute multiRangeAttribute = (MultiRangeAttribute)attribute;		
			MultiRangeStrategyBase drawerExecutor = !multiRangeAttribute.IsInverted ? new MinMaxRangeStrategy(property, multiRangeAttribute) : new MaxMinRangeStrategy(property, multiRangeAttribute);

			drawerExecutor.CalculateRects(position, label, out Rect remainingRect, out Rect leftFieldRect, out Rect sliderRect, out Rect rightFieldRect);

			if(!drawerExecutor.TryGetPropertyInfo(out PropertyInfo propertyInfo))
			{
				EditorGUI.LabelField(remainingRect, "This type is not supported");
				EditorGUI.EndProperty();
				return;
			}

			if (propertyInfo.numericType == PropertyNumericType.Float)
			{
				if (drawerExecutor.ClampValues(ref propertyInfo))
				{
					property.FindPropertyRelative(propertyInfo.relativeNameMin).floatValue = propertyInfo.rangeValue.min;
					property.FindPropertyRelative(propertyInfo.relativeNameMax).floatValue = propertyInfo.rangeValue.max;
				}
			}
			else
			{
				multiRangeAttribute.leftLimit = Mathf.Floor(multiRangeAttribute.leftLimit);
				multiRangeAttribute.rightLimit = Mathf.Floor(multiRangeAttribute.rightLimit);

				if (drawerExecutor.ClampValues(ref propertyInfo))
				{
					property.FindPropertyRelative(propertyInfo.relativeNameMin).intValue = (int)propertyInfo.rangeValue.min;
					property.FindPropertyRelative(propertyInfo.relativeNameMax).intValue = (int)propertyInfo.rangeValue.max;
				}
			}

			if (CreateSlider(sliderRect, property, propertyInfo, drawerExecutor))
			{
				EditorGUI.EndProperty();
				return;
			}

			if (multiRangeAttribute.IsInverted)
			{
				CreateMaxField(leftFieldRect, property, multiRangeAttribute.leftLimit, propertyInfo);
				CreateMinField(rightFieldRect, property, multiRangeAttribute.rightLimit, propertyInfo);
			}
			else
			{
				CreateMinField(leftFieldRect, property, multiRangeAttribute.leftLimit, propertyInfo);
				CreateMaxField(rightFieldRect, property, multiRangeAttribute.rightLimit, propertyInfo);
			}

			EditorGUI.EndProperty();
		}


		private bool CreateSlider(Rect sliderRect, SerializedProperty property, PropertyInfo propertyInfo, MultiRangeStrategyBase drawerExecutor)
		{
			EditorGUI.BeginChangeCheck();
			RangeValue updatedRangeValue = propertyInfo.rangeValue;

			drawerExecutor.CalculateSliderLeftAndRight(propertyInfo, out RangeValue sliderRangeValue, out RangeValue sliderLimitRangeValue);

			EditorGUI.MinMaxSlider(sliderRect, ref sliderRangeValue.min, ref sliderRangeValue.max, sliderLimitRangeValue.min, sliderLimitRangeValue.max);
			if (EditorGUI.EndChangeCheck())
			{
				drawerExecutor.ReadSliderValues(ref updatedRangeValue, sliderRangeValue);

				//rounding slider output to two digits for floats (same logic like in Unity's RangeAttribute)
				bool roundToTwoDigits = propertyInfo.numericType == PropertyNumericType.Float;

				if (!Mathf.Approximately(propertyInfo.rangeValue.min, updatedRangeValue.min))
				{
					SetPropertyValue(property, propertyInfo.relativeNameMin, propertyInfo.numericType, updatedRangeValue.min, roundToTwoDigits);
				}

				if (!Mathf.Approximately(propertyInfo.rangeValue.max, updatedRangeValue.max))
				{
					SetPropertyValue(property, propertyInfo.relativeNameMax, propertyInfo.numericType, updatedRangeValue.max, roundToTwoDigits);
				}
			}

			return false;
		}
		
		private void CreateMinField(Rect fieldRect, SerializedProperty property, float multiRangeAttributeMin, PropertyInfo propertyInfo)
		{
			float updatedMin = propertyInfo.rangeValue.min;

			EditorGUI.BeginChangeCheck();
			updatedMin = CreateField(fieldRect, propertyInfo.rangeValue.min, propertyInfo.numericType);
			if (EditorGUI.EndChangeCheck())
			{
				if (propertyInfo.rangeValue.min != updatedMin)
				{
					SetPropertyValue(property, propertyInfo.relativeNameMin, propertyInfo.numericType, Mathf.Clamp(updatedMin, multiRangeAttributeMin, propertyInfo.rangeValue.max));
				}
			}
		}

		private void CreateMaxField(Rect fieldRect, SerializedProperty property, float multiRangeAttributeMax, PropertyInfo propertyInfo)
		{
			float updatedMax = propertyInfo.rangeValue.max;

			EditorGUI.BeginChangeCheck();
			updatedMax = CreateField(fieldRect, propertyInfo.rangeValue.max, propertyInfo.numericType);
			if (EditorGUI.EndChangeCheck())
			{
				if (propertyInfo.rangeValue.max != updatedMax)
				{
					SetPropertyValue(property, propertyInfo.relativeNameMax, propertyInfo.numericType, Mathf.Clamp(updatedMax, propertyInfo.rangeValue.min, multiRangeAttributeMax));
				}
			}
		}

		public void SetPropertyValue(SerializedProperty property, string name, PropertyNumericType propertyNumericType, float value, bool roundToTwoDigits = false)
		{
			SerializedProperty propertyRelative = property.FindPropertyRelative(name);

			if (propertyNumericType == PropertyNumericType.Float)
			{
				propertyRelative.floatValue = roundToTwoDigits ? System.MathF.Round(value, 2) : value;
			}
			else if (propertyNumericType == PropertyNumericType.Int)
			{
				propertyRelative.intValue = (int)value;
			}
		}

		private float CreateField(Rect fieldRect, float value, PropertyNumericType propertyNumericType)
		{
			GUIStyle guiStyle = new GUIStyle(EditorStyles.numberField);
			guiStyle.padding.left = 5;

			if (propertyNumericType == PropertyNumericType.Float)
			{
				return EditorGUI.DelayedFloatField(fieldRect, value, guiStyle);
			}
			else
			{
				return EditorGUI.DelayedIntField(fieldRect, (int)value, guiStyle);
			}
		}
	}
}