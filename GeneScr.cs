using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneScr 
{
    public GeneScr(GeneScr Gene_A, GeneScr Gene_B)
    {
        GeneData = new float[Gene_A.GetGeneData().Length];

        float[] GeneA_Data = Gene_A.GetGeneData();
        float[] GeneB_Data = Gene_B.GetGeneData();

        if (GeneA_Data[0] != GeneB_Data[0])
        {
            Debug.LogError("A Non Start Neuron Is Being Crossed With A Starting Neuron ||" + " geneA[0] = " + GeneA_Data[0] + " geneB[0] = " + GeneB_Data[0]);
        }
        else
        {
            GeneData[0] = GeneA_Data[0];
        }
        for (int i = 1; i < GeneData.Length; i++)
        {
            GeneData[i] = Mathf.Lerp(GeneA_Data[i], GeneB_Data[i], 0.5f);
        }

    }
    public GeneScr(float Bias, float[] NeuronWeights, bool IsFirstNeuronOfLayer)
    {
        if(NeuronWeights == null)
        {
            GeneData = new float[2];
        }
        else
        {
            GeneData = new float[NeuronWeights.Length + 2];
        }
       
        if(IsFirstNeuronOfLayer)
        {
            GeneData[0] = 1;
        }
        else
        {
            GeneData[0] = -1;
        }
       
        GeneData[1] = Bias; 
        for (int i = 2; i < GeneData.Length; i++)
        {
            GeneData[i] = NeuronWeights[i - 2];
        }
    }

    public void Mutate(float Magnitude, float Chance)
    {
        for (int i = 1; i < GeneData.Length; i++)
        {
            if(Random.Range(0f,1f) < Chance)
            {
                //Debug.Log(" #################################### MutatingGene @ Data[" + i + "]");
                GeneData[i] += Random.Range(-Magnitude, Magnitude);
            }
        }
    }

    float[] GeneData;

    public float[] GetGeneData()
    {
        return GeneData;
    }

    public float[] GetWeightGeneData()
    {
        float[] GeneData2 = new float[GeneData.Length - 2];
        for (int i = 0; i < GeneData2.Length; i++)
        {
            GeneData2[i] = GeneData[i + 2];
        }
        return GeneData2;
    }
}
