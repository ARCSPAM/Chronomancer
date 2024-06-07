using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Line class for UI bar graphs
/// </summary>
public class Line : MonoBehaviour
{
    public Image line;
    public RectTransform graphTransform;
    public float minValue;
    public float maxValue;

    /// <summary>
    /// Updates where the line on the bar graph should be
    /// </summary>
    /// <param name="value"></param>
    public void UpdateLinePosition(float value)
    {
        //finds percent value between min and max value
        float normalizedValue = Mathf.InverseLerp(minValue, maxValue, value);
        float graphWidth = graphTransform.rect.width;
        float halfLineWidth = line.rectTransform.rect.width / 2;
        //finds the correct position for the line
        float xPos = Mathf.Lerp(minValue - graphWidth / 2f + halfLineWidth, graphWidth / 2f - halfLineWidth, normalizedValue);
        //moves line to correct position
        Vector3 newPos = line.rectTransform.localPosition;
        newPos.x = xPos;
        line.rectTransform.localPosition = newPos;
    }
}
