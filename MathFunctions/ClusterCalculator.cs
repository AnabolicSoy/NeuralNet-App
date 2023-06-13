using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;


    public class ClusterCalculator : MonoBehaviour
{
    [SerializeField]
    RectTransform[] HistogramSlots;

    int Clusters;

    int MyNeuronID;

    public void SetUp(int SlotCount)
    {
     
    }

    public void ReCalculate(int[] histogramSlots)
    {
        for (int i = 0; i < histogramSlots.Length; i++)
        {
            RectTransform R = HistogramSlots[i];
            Rect A = R.rect;
            A.height = histogramSlots[i];

            HistogramSlots[i] = R;

        }
    }

}
