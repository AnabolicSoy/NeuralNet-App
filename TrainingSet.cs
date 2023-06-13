using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingSet  
{
    private readonly float[] Inputs = null;
    private readonly float[] Outputs = null;
    public TrainingSet(float[] inputs, float[] outputs)
    {
        Inputs = inputs;
        Outputs = outputs;

    
    }
    public float[] GetOutputs()
    {
        return Outputs;
    }
    public float[] GetInputs()
    {
       
        return Inputs;
    }
}
