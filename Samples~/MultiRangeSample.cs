using MultiRangeSlider;
using UnityEngine;

namespace MultiRangeSlider.Samples
{
	public class MultiRangeSample : MonoBehaviour
	{
		[Header("Min Max")]
		[SerializeField, MultiRange(0f, 180.5f)] private RangeValue rangeValueMinMax;
		[SerializeField, MultiRange(0, 180.5f)] private RangeValueInt rangeValueIntMinMax;
		[SerializeField, MultiRange(0f, 180f)] private Vector2 vector2MinMax;
		[SerializeField, MultiRange(0, 180.5f)] private Vector2Int vector2IntMinMax;

		[Header("Max Min")]
		[SerializeField, MultiRange(180f, 0f)] private RangeValue rangeValueMaxMin;
		[SerializeField, MultiRange(180.5f, 0f)] private RangeValueInt rangeValueIntMaxMin;
		[SerializeField, MultiRange(180f, 0f)] private Vector2 vector2MaxMin;
		[SerializeField, MultiRange(180.5f, 0f)] private Vector2Int vector2IntMaxMin;
	}
}