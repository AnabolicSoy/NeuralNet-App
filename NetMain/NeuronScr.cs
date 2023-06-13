using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NeuronScr 
{
    //------------CONSTRUCTOR----------------//

    NeuralNetMainModule.NeuronType MyType;

    public NeuronScr(int NumberOfNeuronsInThePrevious, float[] axonWeights,float bias, NeuralNetMainModule.ActivationFunction ActFunc, NeuralNetMainModule.NeuronType neuronType )
    {
        activationFunction = ActFunc;
        InputValues = new float[NumberOfNeuronsInThePrevious];
        AxonWeight = axonWeights;
        MyType = neuronType;

        if(neuronType == NeuralNetMainModule.NeuronType.Compute)
        for (int i = 0; i < AxonWeight.Length; i++)
        {
            if (float.IsInfinity(AxonWeight[i]) || AxonWeight[i] == float.NaN)
            {
                Debug.LogError(AxonWeight[i]);
                Debug.Log(i);
            }

        }

        ActivationFunctionMod = 1;
        switch (neuronType)
        {
            case NeuralNetMainModule.NeuronType.Input:

                WeightChange = new float[axonWeights.Length];
                break;
            case NeuralNetMainModule.NeuronType.Compute:
                WeightChange = new float[axonWeights.Length];
                break;
            case NeuralNetMainModule.NeuronType.Output:
                break;
        }
        NumberOfInputsTaken = 0;
        Bias = bias;
    
    }

    //-----------Activation Functions--------//

    float ActivationFunctionMod;

    //------------INPUT AND WEIGHT SYSTEM----------------//

    public void SetActivationValue(float value)
    {
        if(MyType == NeuralNetMainModule.NeuronType.Input)
        {
            ActivationValue = value;
           // Debug.Log("Setting Activation Value To : " + value);
        }
        else
        {
            //Debug.LogError("Trying To Set Activation Value of a Non Input Neuron");
        }
    }

    public void AddInput(float value)
    {
        if(MyType != NeuralNetMainModule.NeuronType.Input)
        {
           // Debug.Log(MyType + "Neuron Adding Input " + NumberOfInputsTaken + " : " + value);
            InputValues[NumberOfInputsTaken] = value;
            NumberOfInputsTaken++;
        }
        else if(MyType == NeuralNetMainModule.NeuronType.Output)
        {
            //Debug.Log("==============================>>>>>>>>>>>>>>> Adding Input Entry To Output Neuron");
            InputValues[NumberOfInputsTaken] = value;
            NumberOfInputsTaken++;
        }
        else
        {
            Debug.LogError("CantAddInputToInputNeuron");
        }

    }

    float[] InputValues;
    int NumberOfInputsTaken;

    NeuralNetMainModule.ActivationFunction activationFunction;
    public NeuralNetMainModule.ActivationFunction GetActivationFunction()
    {
        return activationFunction;
    }
    public void ResetInputs()
    {
        for (int i = 0; i < InputValues.Length; i++)
        {
            InputValues[i] = 0;
        }
        NumberOfInputsTaken = 0;
    }

    //----------------BIAS----------------------//

    float Bias;
    public void SetBias(float Value)
    {
        Bias = Value;
    }
    public float GetBias()
    {
        return Bias;
    }

    //--------------OUTPUT SYSTEM--------------//

    float ActivationValue;
    float SigmaValue;

    public float GetWeightedActivationValue(int WeightID)
    {
        // Debug.Log("Axon ID : " + WeightID +" | AV : " + ActivationValue  + " | Weight : " + AxonWeight[WeightID]  + " | Weight * ActivationValue : " + (ActivationValue * AxonWeight[WeightID]));
        if (ActivationValue * AxonWeight[WeightID] != ActivationValue * AxonWeight[WeightID] ||  float.IsInfinity(ActivationValue * AxonWeight[WeightID] ) )
        {
            Debug.LogError(ActivationValue);
            Debug.LogError(AxonWeight[WeightID]);
          
        }
        return ActivationValue  * AxonWeight[WeightID];
    }
    public float GetActivationValue()
    {
       // Debug.Log("ActivationValue : "  + ActivationValue);
        return ActivationValue;
    }

    public float CalculateActivationValue()
    {

       // Debug.Log("Calculating Activation Value... " );
        float Output = 0;

        for (int i = 0; i < NumberOfInputsTaken; i++)
        {
            Output += InputValues[i];
        }
        Output += Bias;// Sum Weights + Bias

        SigmaValue = Output;
       // Debug.Log("Sum = " + Output);

        switch (activationFunction)
        {
            case NeuralNetMainModule.ActivationFunction.Linear:
                Output *= ActivationFunctionMod;
                break;
            case NeuralNetMainModule.ActivationFunction.BinaryStep:
                if(Output >= ActivationFunctionMod)
                {
                    Output = ActivationFunctionMod;
                }
                else
                {
                    Output = 0;
                }
                break;
            case NeuralNetMainModule.ActivationFunction.Sigmoid:

                Output = ActivationFunctionMod / (1 + Mathf.Exp(-Output));

                break;
            case NeuralNetMainModule.ActivationFunction.HyperbolicTangent:
                if(Output >= 4)
                {
                    Output = 0.9995f;
                }
                else if(Output <= -4)
                {
                    Output = -0.9995f;
                }
                else
                {
                    Output = (Mathf.Exp(2 * Output) - 1) / (Mathf.Exp(2 * Output) + 1);
                }
              

                break;
        }
        NumberOfInputsTaken = 0;

       // Debug.Log(activationFunction + "(Output) = " + Output );
        ActivationValue = Output;

        if (Output != Output)
        {
            Debug.LogError(activationFunction);

            Debug.LogError(Mathf.Exp(2 * SigmaValue) - 1);
            Debug.LogError(Mathf.Exp(2 * SigmaValue) + 1);

            Debug.LogError(SigmaValue);


            Debug.LogError((Mathf.Exp(2 * SigmaValue) - 1) / (Mathf.Exp(2 * SigmaValue) + 1));
        }
        if (float.IsInfinity(Output))
        {
            Debug.LogError(Output);
            Debug.LogError((Mathf.Exp(2 * Output) - 1));
            Debug.LogError((Mathf.Exp(2 * Output) + 1));
            Debug.LogError((Mathf.Exp(2 * Output) - 1) / (Mathf.Exp(2 * Output) + 1));
        }

      //  Debug.LogWarning(Output);

        return Output;
    }

    public float GetSigmaValue()
    {
        return SigmaValue; 
    }

    //--------------ForwardPropagation--------------//

    // int[] AxonConnections; 
    // ref to the next layers neuron ID 

    /* 
       public int[] GetConnectionList()
       {
          return AxonConnections;
       }
    */

    float[] AxonWeight;

    public float[] GetWeightsList()
    {
     
        return AxonWeight;
    }

    public void SetNewWeight(int ID, float value)
    {
        AxonWeight[ID] = value;
    }

    //--------------------------Backpropagation-----------------------//

    float DelC0;
    public float GetDelC0() { return DelC0; }   
    public void SetDelC0(float Value)
    {
      //  Debug.Log("DelC0  = " + Value);
        DelC0 = Value;
    }

    

    float[] WeightChange;
    float BiasChange;

    public void ComitChanges()
    {
      //  Debug.Log("---Changing Neuron---");

        if(MyType != NeuralNetMainModule.NeuronType.Output)
        {
            for (int i = 0; i < WeightChange.Length; i++)
            {
                //    Debug.Log("WeightChange["+i+"]:" + WeightChange[i]);
                AxonWeight[i] += WeightChange[i];
            }
            WeightChange = new float[WeightChange.Length];
        }

        Bias += BiasChange;
        BiasChange = 0;
        DelC0 = 0;

        //Debug.Log( MyType + " BiasChange:" + BiasChange);
    }

    public void AddBiasChangeEntry(float value)
    {
        BiasChange += value;
    }

    public void AddWeightChangeEntry(float value,int ConnectionID)
    {
        WeightChange[ConnectionID] = value;   
    }

}
