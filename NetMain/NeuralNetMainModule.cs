
using JetBrains.Annotations;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;

public class NeuralNetMainModule : MonoBehaviour
{
    //-------------------------------Meta Data------------------------//

    bool WasAlteredAfterSaving;
    public void SetWasAlteredAfterSaving(bool value)
    {
        WasAlteredAfterSaving= value;
    }
    public bool GetWasAlteredAfterSaving()
    {
        return WasAlteredAfterSaving;
    }

    string ModelName;
    public string GetModelName() { return ModelName; }
    public void SetModelName(string s)
    {
        ModelName = s;
    }

    float Loss; 
    public float GetLoss() 
    { return Loss; }
    public void SetLoss(float loss)
    {
        Loss = loss;
    }

    int Cycles; 
    public int GetCycles()
    { return Cycles; }


    string NameID; 
    public string GetNameID()
    { return NameID; }
    public void SetNameID(string nameID)
    {
        NameID = nameID;
    }

    bool IsLoaded = false;
    public bool GetIsLoaded() { return IsLoaded; } 


    float TrainingResults;
    public void AddResultToTrainingResults(float result)
    {
        TrainingResults += result;
    }
    public float GetTrainingResult()
    {
        return TrainingResults;
    }

    public DataManager.LayerData[] GetLayerData()
    {
        DataManager.LayerData[] Data = new DataManager.LayerData[NeuralNet.Length];
        float[] BiasArray = null;
        float[,] WeightArray = null;
        for (int i = 0; i < NeuralNet.Length; i++)
        {
            Debug.Log("Layer " + i);
            BiasArray = new float[NeuralNet[i].Length];
            if(i < NeuralNet.Length-1)
            {
                WeightArray = new float[NeuralNet[i].Length, NeuralNet[i + 1].Length];
            }         
            for (int j = 0; j < NeuralNet[i].Length; j++)
            {
                BiasArray[j] = NeuralNet[i][j].GetBias();
                Debug.Log("BiasArray[" + j + "] = " + BiasArray[j]);
                if (i < NeuralNet.Length - 1)
                {
                    for (int k = 0; k < NeuralNet[i][j].GetWeightsList().Length; k++)
                    {
                        WeightArray[j, k] = NeuralNet[i][j].GetWeightsList()[k];
                    }
                }
                else
                {
                    WeightArray = null;
                }
            }
            Data[i] = new DataManager.LayerData(BiasArray, WeightArray);
        }
        return Data;
    }

    public void GetAutoencoderData(out int InputOutputLayerSize, out int FeatureLayerSize)
    {
        if(MyTrainingType == TrainingModel.NetworkType.Unsupervised)
        {
            Debug.Log("Unsupervised");
            InputOutputLayerSize = NumberOfInputNeurons;
            FeatureLayerSize = FeatureLayerNeurons;
        }
        else
        {
            Debug.Log("Supervised");
            InputOutputLayerSize = 0;
            FeatureLayerSize = 0;
        }
    }
    

    public int GetIncDec()
    {
        return EncoderDecoderDecrementIncrement;
    }

    //-------------------------------SetUp And Constructors------------------------//

    public void SetUpAsAutoEncoder(DataManager.LayerData[] LayerData, ActivationFunction Func, string modelName, int NetID)
    {

        IsLoaded = true;
    }
    public void SetUpAsAutoEncoder(int numberOfLayers,int InputOutputNeurons,int NeuronDecrementIncrement,int featureLayerNeurons, string modelName, int NetID, ActivationFunction Func)
    {
        Debug.Log("««««««««««««««««««SetUpAsAutoEncoder»»»»»»»»»»»»»»»»»»");
        TrainingResults = 0;
        NumberOfLayers = numberOfLayers; // has to be an even number | Encoder Layer = Decoder Layers | N layers = Encoder+Decoder+FeatureLayer

        NeuronsPerLayer_Min = InputOutputNeurons;
        NeuronsPerLayer_Max = InputOutputNeurons;

        NumberOfInputNeurons = InputOutputNeurons;
        NumberOfOutputNeurons = InputOutputNeurons;

        FeatureLayerNeurons = featureLayerNeurons;

        IsLoaded = true;

        MyActivationFunction = ActivationFunction.HyperbolicTangent;

        MyTrainingType = TrainingModel.NetworkType.Unsupervised;

        EncoderDecoderDecrementIncrement = NeuronDecrementIncrement;

        GenLayersAsAutoEncoder();

        int NumberOfGenes = 0;
        for (int i = 0; i < NeuralNet.Length; i++)
        {
            NumberOfGenes += NeuralNet[i].Length;
        }
        GeneScr[] Genes = new GeneScr[NumberOfGenes];

        int Counter = 0;
        for (int i = 0; i < NeuralNet.Length; i++)
        {
            //Debug.Log("------------------ Neurons @ Layer " + i + " = " + NeuralNet[i].Length + " ------------");
            for (int j = 0; j < NeuralNet[i].Length; j++)
            {
                //if(i != NeuralNet.Length - 1)
                //Debug.Log("---- Neuron "+ j + " Bias : " + NeuralNet[i][j].GetBias()+" Weights Length = " + NeuralNet[i][j].GetWeightsList().Length);
                ////for (int k = 0; k < NeuralNet[i][j].GetWeightsList().Length; k++)
                //{
                //    Debug.Log(" Weights : " + NeuralNet[i][j].GetWeightsList()[k]);
                //}
                bool IsFirstNeuronOfLayer = false;
                if (j == 0)
                {
                    IsFirstNeuronOfLayer = true;
                }
                if (i == NeuralNet.Length - 1)
                {
                    Genes[Counter] = new GeneScr(NeuralNet[i][j].GetBias(), null, IsFirstNeuronOfLayer);
                    Counter++;
                }
                else
                {
                    Genes[Counter] = new GeneScr(NeuralNet[i][j].GetBias(), NeuralNet[i][j].GetWeightsList(), IsFirstNeuronOfLayer);
                    Counter++;
                }

            }
        }
        Genome = new ChromScr(Genes);
        WasAlteredAfterSaving = true;


    }
    public void SetUpFromExcelData(DataManager.LayerData[] LayerData, ActivationFunction Func, string modelName, int NetID, bool isAutoEnc, int FtL, int IncDec)
    {
        MyActivationFunction = Func;
        NameID = "Net_" + NetID.ToString();
        TrainingResults = 0;
        NumberOfLayers = LayerData.Length;
        NumberOfInputNeurons = LayerData[0].Biases.Length;
        NumberOfOutputNeurons = LayerData[NumberOfLayers - 1].Biases.Length;

        if(isAutoEnc)
        {
            MyTrainingType = TrainingModel.NetworkType.Unsupervised;
            FeatureLayerNeurons = FtL;
            EncoderDecoderDecrementIncrement = IncDec;
        }
        else
        {
            MyTrainingType = TrainingModel.NetworkType.Supervised;
        }

        IsLoaded = true;

        NeuralNet = new NeuronScr[NumberOfLayers][];

        for (int i = 0; i < NumberOfLayers; i++)
        {
            NeuralNet[i] = new NeuronScr[LayerData[i].Biases.Length];
        }

        for (int j = 0; j < NumberOfLayers; j++)
        {
            for (int k = 0; k < NeuralNet[j].Length; k++)
            {
                float[] WeightsArray;
                if (j < NumberOfLayers - 1)
                {
                 WeightsArray = new float[LayerData[j].Weights.GetLength(1)];
                for (int l = 0; l < WeightsArray.Length; l++)
                {
                    //Set Weights
                    WeightsArray[l] = LayerData[j].Weights[k, l];
                }
                }
                else
                {
                    WeightsArray = null;
                }
                if (j == 0)
                {
                    NeuralNet[j][k] = new NeuronScr(0, WeightsArray, LayerData[j].Biases[k], MyActivationFunction, NeuronType.Input);
                }
                else if(j == NumberOfLayers-1)
                {
                    NeuralNet[j][k] = new NeuronScr(NeuralNet[j-1].Length, null, LayerData[j].Biases[k], MyActivationFunction, NeuronType.Output);
                }
                else
                {
                    NeuralNet[j][k] = new NeuronScr(NeuralNet[j - 1].Length, WeightsArray, LayerData[j].Biases[k], MyActivationFunction, NeuronType.Compute);
                }
            }
        }

        int NumberOfGenes = 0;
        for (int i = 0; i < NeuralNet.Length; i++)
        {
            NumberOfGenes += NeuralNet[i].Length;
        }
        GeneScr[] Genes = new GeneScr[NumberOfGenes];

        int Counter = 0;
        for (int i = 0; i < NeuralNet.Length; i++)
        {
            for (int j = 0; j < NeuralNet[i].Length; j++)
            {
                bool IsFirstNeuronOfLayer = false;
                if (j == 0)
                {
                    IsFirstNeuronOfLayer = true;
                }
                if (i == NeuralNet.Length - 1)
                {
                    Genes[Counter] = new GeneScr(NeuralNet[i][j].GetBias(), null, IsFirstNeuronOfLayer);
                    Counter++;
                }
                else
                {
                    Genes[Counter] = new GeneScr(NeuralNet[i][j].GetBias(), NeuralNet[i][j].GetWeightsList(), IsFirstNeuronOfLayer);
                    Counter++;
                }

            }
        }
        Genome = new ChromScr(Genes);

        WasAlteredAfterSaving = false;
    }
    public void SetUp(int numberOfLayers, int neuronPerLayer_Min, int neuronPerLayer_Max, int numberOfInputNeurons, int numberOfOutputNeurons)
    {
        MyActivationFunction = ActivationFunction.HyperbolicTangent;

        TrainingResults = 0;
        NumberOfLayers = numberOfLayers;
        NeuronsPerLayer_Min = neuronPerLayer_Min;
        NeuronsPerLayer_Max = neuronPerLayer_Max;
        NumberOfInputNeurons = numberOfInputNeurons;
        NumberOfOutputNeurons = numberOfOutputNeurons;
        IsLoaded = true;

        GenLayers();

        int NumberOfGenes = 0;
        for (int i = 0; i < NeuralNet.Length; i++)
        {
            NumberOfGenes += NeuralNet[i].Length;
        }
        GeneScr[] Genes = new GeneScr[NumberOfGenes];

        int Counter = 0;
        for (int i = 0; i < NeuralNet.Length; i++)
        {
            for (int j = 0; j < NeuralNet[i].Length; j++)
            {
                bool IsFirstNeuronOfLayer = false;
                if (j == 0)
                {
                    IsFirstNeuronOfLayer = true;
                }
                if (i == NeuralNet.Length - 1)
                {
                    Genes[Counter] = new GeneScr(NeuralNet[i][j].GetBias(), null, IsFirstNeuronOfLayer);
                    Counter++;
                }
                else
                {
                    Genes[Counter] = new GeneScr(NeuralNet[i][j].GetBias(), NeuralNet[i][j].GetWeightsList(), IsFirstNeuronOfLayer);
                    Counter++;
                }

            }
        }
        Genome = new ChromScr(Genes);
        WasAlteredAfterSaving = true;
    }
  
    public NeuralNetMainModule(GeneScr[] CopyGenome,ActivationFunction AC)
    {
        TrainingResults = 0;

        Genome = new ChromScr(CopyGenome);



        GenLayers(CopyGenome,AC);
    }
    public NeuralNetMainModule(GeneScr[] ParentGenome1, GeneScr[] ParentGenome2,ActivationFunction AC)
    {
        TrainingResults = 0;
        Genome = new ChromScr(ParentGenome1, ParentGenome2, ChromScr.RecombinationType.NullRecomb, true, NetTrainingSystem.GlobalMutationChance, NetTrainingSystem.GlobalMaxMutationMagnitue);


        GenLayers(Genome.GetGenes(),AC);

    }
    public NeuralNetMainModule(int numberOfLayers, int neuronPerLayer_Min, int neuronPerLayer_Max, int numberOfInputNeurons, int numberOfOutputNeurons)
    {
        TrainingResults = 0;
        NumberOfLayers = numberOfLayers;
        NeuronsPerLayer_Min = neuronPerLayer_Min;
        NeuronsPerLayer_Max = neuronPerLayer_Max;
        NumberOfInputNeurons = numberOfInputNeurons;
        NumberOfOutputNeurons = numberOfOutputNeurons;
        IsLoaded = true;
        GenLayers();

        int NumberOfGenes = 0;
        for (int i = 0; i < NeuralNet.Length; i++)
        {
            NumberOfGenes += NeuralNet[i].Length;
        }

        GeneScr[] Genes = new GeneScr[NumberOfGenes];
        int Counter = 0;

        for (int i = 0; i < NeuralNet.Length; i++)
        {
            for (int j = 0; j < NeuralNet[i].Length; j++)
            {
                bool IsFirstNeuronOfLayer = false;
                if(j == 0)
                {
                    IsFirstNeuronOfLayer = true;
                }
                if(i == NeuralNet.Length - 1)
                {
                    Genes[Counter] = new GeneScr(NeuralNet[i][j].GetBias(), null, IsFirstNeuronOfLayer);
                    Counter++;
                }
                else
                {
                    Genes[Counter] = new GeneScr(NeuralNet[i][j].GetBias(), NeuralNet[i][j].GetWeightsList(), IsFirstNeuronOfLayer);
                    Counter++;
                }
              
            }
        }


        Genome = new ChromScr(Genes);



    }

    private void GenLayers()
    {
        // Debug.Log("Generating Network Array");
        NeuralNet = new NeuronScr[NumberOfLayers][];

        NeuralNet[0] = new NeuronScr[NumberOfInputNeurons];
        NeuralNet[NumberOfLayers - 1] = new NeuronScr[NumberOfOutputNeurons];
        int[] Temp_NeuronsPerLayerArray = new int[NumberOfLayers];

        for (int i = 1; i < NumberOfLayers - 1; i++)
        {
            Temp_NeuronsPerLayerArray[i] = Random.Range(NeuronsPerLayer_Min, NeuronsPerLayer_Max + 1);
            NeuralNet[i] = new NeuronScr[Temp_NeuronsPerLayerArray[i]];
        }//Set number of neurons

        //  Debug.Log("Setting Up Input Neurons");
        for (int i = 0; i < NeuralNet[0].Length; i++)
        {
            float[] axonWeights = new float[NeuralNet[1].Length];
            for (int k = 0; k < axonWeights.Length; k++)
            {
                axonWeights[k] = Random.Range(1f, 4f);
                //  Debug.Log("SettingUpWeight " + k + " of Input Layer Neuron " + i + "| value = " + axonWeights[k]);
            }
            NeuralNet[0][i] = new NeuronScr(0, axonWeights, 0, MyActivationFunction, NeuronType.Input);
        }//SetInput

        for (int i = 0; i < NeuralNet[NeuralNet.Length - 1].Length; i++)
        {
            // Debug.Log("SettingUpWeight Output Layer Neuron " + i );
            NeuralNet[NeuralNet.Length - 1][i] = new NeuronScr(NeuralNet[NeuralNet.Length - 2].Length, null, 0, MyActivationFunction, NeuronType.Output);
        }//SetOutput

        for (int i = 1; i < NeuralNet.Length - 1; i++)
        {
            for (int j = 0; j < NeuralNet[i].Length; j++)
            {
                float[] axonWeights = new float[NeuralNet[i + 1].Length];
                for (int k = 0; k < axonWeights.Length; k++)
                {
                    axonWeights[k] = Random.Range(-1f, 2f);
                    //  Debug.Log("SettingUpWeight " + k + " of Compute Layer " + i + " Neuron " + j + "| value = " + axonWeights[k]);
                }
                NeuralNet[i][j] = new NeuronScr(NeuralNet[i - 1].Length, axonWeights, 0, MyActivationFunction, NeuronType.Compute);
            }
        }//SetComputeNeurons


    }
    private void GenLayersAsAutoEncoder()
    {
        Debug.Log("GenLayersAsAutoEncoder");
        Debug.Log("NumberOfLayers " + NumberOfLayers);
        Debug.Log("NumberOfInputNeurons " + NumberOfInputNeurons);
        Debug.Log("IncDec " + EncoderDecoderDecrementIncrement);
        Debug.Log("Ftl Size " + FeatureLayerNeurons);
        NeuralNet = new NeuronScr[NumberOfLayers][];
        int[] Temp_NeuronsPerLayerArray = new int[NumberOfLayers];
        NeuralNet[0] = new NeuronScr[NumberOfInputNeurons];

        NeuralNet[(NumberOfLayers - 1 )/ 2] = new NeuronScr[FeatureLayerNeurons];
        Temp_NeuronsPerLayerArray[(NumberOfLayers - 1) / 2] = FeatureLayerNeurons;

        NeuralNet[NumberOfLayers - 1] = new NeuronScr[NumberOfOutputNeurons];
       
        int counter = 1;
        for (int i = 1; i < (NumberOfLayers-1)/2 ; i++)
        {
            Temp_NeuronsPerLayerArray[i] = NumberOfInputNeurons - (counter * EncoderDecoderDecrementIncrement);
            NeuralNet[i] = new NeuronScr[Temp_NeuronsPerLayerArray[i]];
            counter++;
        }//Set number of neurons at encoder
        counter--;
        for (int i = 1+ (NumberOfLayers - 1 )/ 2; i < NumberOfLayers; i++)
        {
            Temp_NeuronsPerLayerArray[i] = NumberOfInputNeurons - (counter * EncoderDecoderDecrementIncrement);
            NeuralNet[i] = new NeuronScr[Temp_NeuronsPerLayerArray[i]]; 
            counter--;
        }//Set number of neurons at decoder

        //  Debug.Log("Setting Up Input Neurons");
        for (int i = 0; i < NeuralNet[0].Length; i++)
        {
            float[] axonWeights = new float[NeuralNet[1].Length];
            for (int k = 0; k < axonWeights.Length; k++)
            {
                int a = (Random.Range(0, 2) == 0)? -1 : 1;
                axonWeights[k] = 1/(float)Temp_NeuronsPerLayerArray[1] + (Mathf.Pow(Random.Range(0, 1f),2) * a);
                //  Debug.Log("SettingUpWeight " + k + " of Input Layer Neuron " + i + "| value = " + axonWeights[k]);
            }
            NeuralNet[0][i] = new NeuronScr(0, axonWeights, 0, MyActivationFunction, NeuronType.Input);
        }//SetInput

        for (int i = 0; i < NeuralNet[NeuralNet.Length - 1].Length; i++)
        {
            // Debug.Log("SettingUpWeight Output Layer Neuron " + i );
            NeuralNet[NeuralNet.Length - 1][i] = new NeuronScr(NeuralNet[NeuralNet.Length - 2].Length, null, 0, MyActivationFunction, NeuronType.Output);
        }//SetOutput

        for (int i = 1; i < NeuralNet.Length - 1; i++)
        {
            for (int j = 0; j < NeuralNet[i].Length; j++)
            {
                float[] axonWeights = new float[NeuralNet[i + 1].Length];
                for (int k = 0; k < axonWeights.Length; k++)
                {
                    int a = (Random.Range(0, 2) == 0) ? -1 : 1;
                    axonWeights[k] =( 1 / (float)Temp_NeuronsPerLayerArray[i+1]) + (Mathf.Pow(Random.Range(0, 1f), 2) * a);
                      //Debug.Log("SettingUpWeight " + k + " of Compute Layer " + i + " Neuron " + j + "| value = " + axonWeights[k]);
                    if(float.IsInfinity(axonWeights[k]))
                    {

                        Debug.LogError((float)Temp_NeuronsPerLayerArray[i + 1]);
                        Debug.LogError((Mathf.Pow(Random.Range(0, 1f), 2) * a));
                    }
                }
                NeuralNet[i][j] = new NeuronScr(NeuralNet[i - 1].Length, axonWeights, 0, MyActivationFunction, NeuronType.Compute);
            }
        }//SetComputeNeurons
    }
    private void GenLayers(GeneScr[] Genes , ActivationFunction Ac)
    {
        int LayerCount = 0;
        int NeuronCount = 0;
        int GeneCount = 0;

        //  Debug.Log("================================= Gen Layers From Genes Array ===============================");

        for (int i = 0; i < Genes.Length; i++)
        {
            if (Genes[i].GetGeneData()[0] > 0)
            {
                LayerCount++;
            }
        } // Find Number Of Layers
        NeuralNet = new NeuronScr[LayerCount][];

        //   Debug.Log("Total Number Of Genes = " + Genes.Length);
        for (int i = 0; i < NeuralNet.Length; i++)
        {
            Reset:;

            if (Genes.Length > GeneCount + 1)
            {
                NeuronCount++;
                GeneCount++;
                if (Genes[GeneCount].GetGeneData()[0] > 0)
                {
                    NeuralNet[i] = new NeuronScr[NeuronCount];
                    NeuronCount = 0;
                }//Is It a starting Neuron ? = true
                else
                {
                    goto Reset;
                }//Is It a starting Neuron ? = false
            }
            else
            {
                NeuronCount++;
                NeuralNet[i] = new NeuronScr[NeuronCount];
                goto ExitCycle;
            }

        } //Find Number Of Neurons Per Layer
        ExitCycle:;

        GeneCount = 0;

        for (int i = 0; i < NeuralNet.Length; i++)
        {
            if (i == 0)
            {
                for (int j = 0; j < NeuralNet[i].Length; j++)
                {
                    // Debug.Log("Setting Input Layer | Neuron " + j);
                    NeuralNet[i][j] = new NeuronScr(0, Genes[GeneCount].GetWeightGeneData(), Genes[GeneCount].GetGeneData()[1], ActivationFunction.HyperbolicTangent, NeuronType.Input);
                    GeneCount++;
                }
            }
            else if (i == NeuralNet.Length - 1)
            {
                for (int j = 0; j < NeuralNet[i].Length; j++)
                {
                    //  Debug.Log("Setting Output Layer | Neuron " + j);
                    NeuralNet[i][j] = new NeuronScr(NeuralNet[i - 1].Length, null, Genes[GeneCount].GetGeneData()[1], ActivationFunction.HyperbolicTangent, NeuronType.Output);
                    GeneCount++;
                }
            }
            else
            {
                for (int j = 0; j < NeuralNet[i].Length; j++)
                {
                    //   Debug.Log("Setting Compute Layer " + i + " | Neuron " + j);
                    NeuralNet[i][j] = new NeuronScr(NeuralNet[i - 1].Length, Genes[GeneCount].GetWeightGeneData(), Genes[GeneCount].GetGeneData()[1], ActivationFunction.HyperbolicTangent, NeuronType.Compute);
                    GeneCount++;
                }
            }

        } // Set Neurons in Net 

        NumberOfOutputNeurons = NeuralNet[NeuralNet.Length - 1].Length;
        NumberOfInputNeurons = NeuralNet[0].Length;
        for (int i = 0; i < NeuralNet.Length; i++)
        {

            // Debug.Log("Setting Layer " + i + " | Number Of Neurons " + NeuralNet[i].Length);

        }


    }

    //------------------------------GenomeData------------------------//

    ChromScr Genome;
    public ChromScr GetGenome()
    { return Genome; }

    //-------------------------------Native Enums--------------------------//
    public enum ActivationFunction
    {
        Linear,
        BinaryStep,
        Sigmoid,
        HyperbolicTangent,
    }

    public enum NeuronType
    {
        Input,
        Compute,
        Output,
    }

    //------------------------------Network Functionality Data------------------------//

    ActivationFunction MyActivationFunction; 
    public void SetActivationFunction(ActivationFunction AF)
    { MyActivationFunction = AF; }
    public ActivationFunction GetActivationFunction()
    { return MyActivationFunction; }

    int NumberOfLayers;

    int NeuronsPerLayer_Min;

    int NeuronsPerLayer_Max;

    int NumberOfInputNeurons;

    int NumberOfOutputNeurons;

    int FeatureLayerNeurons;

    int EncoderDecoderDecrementIncrement;

    TrainingModel.NetworkType MyTrainingType;

    public TrainingModel.NetworkType GetMyTrainingType()
    {
        return MyTrainingType;
    }

    //---------------------------Neurons Array and Functions---------------------//

    NeuronScr[][] NeuralNet;
    public NeuronScr[][] GetNetwork()
    {
        return NeuralNet;
    }

    public int GetNumberOfLayers()
    {
        return NeuralNet.Length;
    }

    public int GetNumberOfNeurons()
    {
        return Genome.GetNumberOfGenes();
    }
    public int GetNumberOfNeuronsAtLayer(int Layer)
    {
        return NeuralNet[Layer].Length;
    }
    public NeuronScr GetNeuron(int Layer, int NeuronID)
    {
        return NeuralNet[Layer][NeuronID];
    }

    //---------------------RUN NET---------------//

    public void RunNetworkCycle (float[] Inputs, out float[] Outputs)
    {
  
        Outputs = new float[NumberOfOutputNeurons];
       // Debug.Log("Number of Output Neurons "+ NumberOfOutputNeurons);
        for (int i = 0; i < NeuralNet[0].Length; i++)
        {
            NeuralNet[0][i].SetActivationValue(Inputs[i]);
            //Debug.Log(">>> Processing Input Layer @ Neuron " + i + " = " + NeuralNet[0][i].GetActivationValue());
            for (int k = 0; k < NeuralNet[1].Length; k++)
            {
                NeuralNet[1][k].AddInput(NeuralNet[0][i].GetWeightedActivationValue(k));
                
            }
        }
        for (int i = 1; i < NeuralNet.Length; i++)
        {
            for (int j = 0; j < NeuralNet[i].Length; j++)
            {                
                if (i < NeuralNet.Length - 1)
                { 
                    NeuralNet[i][j].CalculateActivationValue();
                    //Debug.Log(">>> Processing Compute Layer " + i + " @ Neuron" + j + " = " + NeuralNet[i][j].GetActivationValue());
                    for (int k = 0; k < NeuralNet[i+ 1].Length; k++)
                    {
                        float value = NeuralNet[i][j].GetWeightedActivationValue(k);
                        NeuralNet[i+1][k].AddInput(value);
                      
                    }
                }
                else
                {
                    Outputs[j] = NeuralNet[i][j].CalculateActivationValue();
                    //Debug.Log(">>> Processing Output Layer " + i + " @ Neuron" + j + " Outputs[j] = " + Outputs[j]);
                }
            }
        }
    }

    public void RunNetworkCyclePassDataToUI(float[] Inputs, out float[] Outputs)
    {
        //for (int i = 0; i < Inputs.Length; i++)
        //{
        //    Debug.Log(Inputs[i]);
        //}
        Outputs = new float[NumberOfOutputNeurons];
         //Debug.Log("Number of Output Neurons "+ NumberOfOutputNeurons);
        for (int i = 0; i < NeuralNet[0].Length; i++)
        {
              //Debug.Log(">>> Processing Input Layer @ Neuron " + i);
            NeuralNet[0][i].SetActivationValue(Inputs[i]);
            for (int k = 0; k < NeuralNet[1].Length; k++)
            {
                float value = NeuralNet[0][i].GetWeightedActivationValue(k);
                //Debug.Log(" Propagating Input  " + k + " = " + value);
                NeuralNet[1][k].AddInput(value);
                UIManager.UIM.SetWeightTensor(0, i, k, value);
            }
        }
        for (int i = 1; i < NeuralNet.Length; i++)
        {
            for (int j = 0; j < NeuralNet[i].Length; j++)
            {
                if (i < NeuralNet.Length - 1)
                {
                      //Debug.Log(">>> Processing Compute Layer " + i + " @ Neuron" + j);
                    
                   float a = NeuralNet[i][j].CalculateActivationValue(); 
                    //Debug.Log("Activation Value " + a);
                    //Debug.Log("Activation Bias " + NeuralNet[i][j].GetBias());
                    //Debug.Log("Activation Act " + NeuralNet[i][j].GetActivationFunction());
                    //Debug.Log("Activation Sigma " + NeuralNet[i][j].GetSigmaValue());
                    for (int k = 0; k < NeuralNet[i + 1].Length; k++)
                    {
                        float value = NeuralNet[i][j].GetWeightedActivationValue(k);
                        //Debug.Log(" Propagation W = "+ NeuralNet[i][j].GetWeightsList()[k]  + " @ " + k + " = " + value);
                        
                        NeuralNet[i + 1][k].AddInput(value);
                        UIManager.UIM.SetWeightTensor(i, j, k, value);
                    }
                }
                else
                {
                      //Debug.Log(">>> Processing Output Layer " + i + " @ Neuron" + j);
                 
                    Outputs[j] = NeuralNet[i][j].CalculateActivationValue();
                }
            }
        }

    }

    public bool RunEncoderLayers(float[] Inputs, out float[] Outputs)
    {
       // Debug.Log("Runing encoder Layers");
        Outputs = null;
        if (MyTrainingType == TrainingModel.NetworkType.Unsupervised)
        {
            Outputs = new float[NumberOfOutputNeurons];
            
          //  Debug.Log("NeuralNet[0].Length " + NeuralNet[0].Length);
            for (int i = 0; i < NeuralNet[0].Length; i++)
            {
             //   Debug.Log("Setting Input "+i+" to " + Inputs[i]);
                NeuralNet[0][i].SetActivationValue(Inputs[i]);
                
                for (int k = 0; k < NeuralNet[1].Length; k++)
                {
                   
                    //Debug.Log("K " + k);
                    NeuralNet[1][k].AddInput(NeuralNet[0][i].GetWeightedActivationValue(k));
                }
            }
            
            int EncoderLength = ((NumberOfLayers-1)/2)+1;
          //  Debug.Log("Fase 2 EncoderLength = " + EncoderLength);
            for (int i = 1; i < EncoderLength; i++)
            {
                for (int j = 0; j < NeuralNet[i].Length; j++)
                {
                    if (i < EncoderLength-1)
                    {
                        NeuralNet[i][j].CalculateActivationValue();
                    //    Debug.Log(">>> Processing Compute Layer " + i + " @ Neuron" + j + " = " + NeuralNet[i][j].GetActivationValue());
                        for (int k = 0; k < NeuralNet[i + 1].Length; k++)
                        {
                           // Debug.Log("K " + k);
                            float value = NeuralNet[i][j].GetWeightedActivationValue(k);
                            NeuralNet[i + 1][k].AddInput(value);
                        }
                    }
                    else
                    {
                        Outputs[j] = NeuralNet[i][j].CalculateActivationValue();
                     //   Debug.Log(">>> Processing Output Layer " + i + " @ Neuron" + j + " Outputs[j] = " + Outputs[j]);
                    }
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}
