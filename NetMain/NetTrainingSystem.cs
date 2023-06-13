using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditorInternal;
using UnityEngine.Networking.Types;
using JetBrains.Annotations;
using Unity.Jobs.LowLevel.Unsafe;

public class NetTrainingSystem : MonoBehaviour
{

    public static NetTrainingSystem NTS;

    float[,] TrainingData;  //X : Set  Y : input Neuron Value  // X = number of sets Y = number of InputNeurons

    float[,]  TrainingData_IdealOutputs; // X : Set    Y : Output Ideal Value 

    private int GetTrainingDataSize()
    {
        return TrainingData.GetLength(0);
    }

    private float[] GetTrainingSetByID(int ID)
    {
        ID = Mathf.Clamp(ID, 0, TrainingData.GetLength(0));
        float[] Set = new float[TrainingData.GetLength(1)];
        for (int i = 0; i < TrainingData.GetLength(1); i++)
        {
            Set[i] = TrainingData[ID, i];
        }
        return Set;
    }

    private float[] GetRandomTrainingSet(out int SetId)
    {
        float[] Set = new float[TrainingData.GetLength(1)];
        SetId = Random.Range(0, TrainingData.GetLength(0));
        for (int i = 0; i < Set.Length; i++)
        {
            Set[i] = TrainingData[SetId, i];
        }
        return Set;
    }

    private float[] GetTrainingSetOutputByID(int SetId)
    {
        float[] Set = new float[TrainingData.GetLength(1)];
        
        for (int i = 0; i < Set.Length; i++)
        {
            Set[i] = TrainingData[SetId, i];
        }

        return Set;
    }

    [SerializeField]
    float NetTrainingCycleInterval;

    float CycleTimer;

    //private void Update()
    //{
    //    
    //    if(TrainNet)
    //    {
    //        CycleTimer -= Time.deltaTime;
    //        if(CycleTimer < 0)
    //        {
    //            CycleTimer = NetTrainingCycleInterval;
    //            if(NumberOfAutoCycleGenerations > 0)
    //            {
    //                //RunTrainingCycle();
    //                //CreateGeneticElitePool();
    //                //RunBackPropagationTrainingCycle();
    //                NumberOfAutoCycleGenerations--;
    //            }
    //        }
    //    }
    //}

    //public void RunNetOnce()
    //{
    //  //  Debug.Log("Running Network");

    //    CycleTimer = NetTrainingCycleInterval;
    //    float[] TrainingSet = GetRandomTrainingSet(out int SetID);
    //    NetMain.RunNetworkCycle(TrainingSet, out float[] NetOutput);

    //    //Compare NetOutput to IdealOutputs
    //    float[] IdealOutput = GetTrainingSetOutputByID(SetID); 
    //    float[] Cost = new float[TrainingSet.Length];
    //    float TotalCost = 0;
    //   /* for (int i = 0; i < NetOutput.Length; i++)
    //    {
    //     //   Debug.Log("Output " + NetOutput[i]);
    //       // Debug.Log("Expected Output " + IdealOutput[i]);
    //    }*/

    //    for (int i = 0; i < Cost.Length; i++)
    //    {
    //        Cost[i] = (NetOutput[i] - IdealOutput[i]) * (NetOutput[i] - IdealOutput[i]);
    //     //   Debug.Log("LossFunctionValue @ " + i + " = " + Cost[i]);
    //        TotalCost += Cost[i];
    //    }
    //  //  Debug.Log("Total Cost = " + TotalCost);
    //}

    bool TrainingCycleIsRun;

    private float CalculateLoss(float[] OptimalOutput , float[] CalculatedOutput)
    {
      //  Debug.Log("Running Network");
        float[] Cost = new float[CalculatedOutput.Length];

        float TotalCost = 0;
        /*for (int i = 0; i < CalculatedOutput.Length; i++)
        {
        //    Debug.Log("Output " + CalculatedOutput[i]);
         //   Debug.Log("Expected Output " + OptimalOutput[i]);
        }*/

        for (int i = 0; i < Cost.Length; i++)
        {
            Cost[i] = (CalculatedOutput[i] - OptimalOutput[i]) * (CalculatedOutput[i] - OptimalOutput[i]);
        //    Debug.Log("LossFunctionValue @ " + i + " = " + Cost[i]);
            TotalCost += Cost[i];
        }
      //  Debug.Log("Total Cost = " + TotalCost);
        return TotalCost;
       
    }

    NeuralNetMainModule NetMain;
    [SerializeField]
    bool TrainNet;

    public void CreateNewNet()
    {
        NetMain = new NeuralNetMainModule(4,4,4,4,4);
    }

    public void SetTrainNet(bool value)
    {
        TrainNet = value;
    }

    [SerializeField]
    string directory;

    private void Awake()
    {
        NTS = GetComponent<NetTrainingSystem>();
        NetMain = new NeuralNetMainModule(4,4,4,4,4);
        GlobalMaxMutationMagnitue = MaxMutationMagnitue;
        GlobalMutationChance= MutationChance;
    }

    private void Start()
    {
        TrainingCycleIsRun = false;
        if (!File.Exists(directory))
        {
            Debug.Log("Error");
            File.Create(directory).Close();
        }
        else
        {
            Debug.Log("FileFound");
        }
        ReadTrainingData();
        CreateTrainingPool();

       // UIManager.UIM.CreateNewNetPool(10);

    }

    private void ReadTrainingData()
    {
        StreamReader SR = new StreamReader(directory);
        bool DataFound = false;
        string Data = null;
        string[] SplitData = null;
        int LinesRead = 0;
        while (!DataFound && LinesRead < 50)
        {
            Data = SR.ReadLine();
            if (Data.StartsWith("I"))
            {
                SplitData = Data.Split(';');

                int NumberOsSets = int.Parse(SplitData[1]);
                int ValuesPerSet = int.Parse(SplitData[2]);

                //  Debug.Log("Number Of Sets :" + NumberOsSets);
                //  Debug.Log("Values Per Set :" + ValuesPerSet);

                TrainingData = new float[NumberOsSets, ValuesPerSet];

                // Debug.Log("SettingInputData");

                for (int i = 3; i < SplitData.Length; i++)
                {
                    //  Debug.Log("Reading Set : " + (i - 3));
                    string[] SplitSubData = SplitData[i].Split(',');
                    //Debug.Log("SplitSubData.length : " + SplitSubData.Length);
                    for (int j = 0; j < SplitSubData.Length; j++)
                    {
                        //  Debug.Log("SplitSubData " + SplitSubData[j]);
                        //  Debug.Log("Set " + (i - 3) + "/" + NumberOsSets);
                        //   Debug.Log("Value " + j + "/" + SplitSubData.Length);
                        TrainingData[i - 3, j] = float.Parse(SplitSubData[j]);
                        //   Debug.Log("-----------");
                    }
                }
            }
            else if (Data.StartsWith("O"))
            {
                SplitData = Data.Split(';');

                int NumberOsSets = int.Parse(SplitData[1]);
                int ValuesPerSet = int.Parse(SplitData[2]);

                TrainingData_IdealOutputs = new float[NumberOsSets, ValuesPerSet];

               // Debug.Log("SettingOutputData");

                for (int i = 3; i < SplitData.Length; i++)
                {

                    string[] SplitSubData = SplitData[i].Split(',');
                    for (int j = 0; j < SplitSubData.Length; j++)
                    {
                        //     Debug.Log("SplitSubData " + SplitSubData[j]);
                        TrainingData_IdealOutputs[i - 3, j] = float.Parse(SplitSubData[j]);
                    }

                }



                DataFound = true;
            }
            LinesRead++;
        }
        if (!DataFound)
        {
            Debug.LogError("TrainingDataMisformat");
        }

        //--------------DEBUG DATA------------//
        /*
        File.Create("Assets/Scripts/TrainingData_1.txt").Close();
        StreamWriter SW = new StreamWriter( "Assets/Scripts/TrainingData_1.txt");
        for (int i = 0; i < TrainingData.GetLength(0); i++)
        {
            for (int j = 0; j < TrainingData.GetLength(1); j++)
            {
                SW.Write(TrainingData[i, j] + ",");
            }
            SW.Write(";");
        }
        SW.Close();
        */
        //--------------------------------------//

    }

    // ========================================= GENE POOL ============================================ //

    [SerializeField]
    int NumberOfTrainingCyclesPerGeneration;
    NeuralNetMainModule[] TrainingPool;

    public NeuralNetMainModule GetTrainingPoolNet(int ID)
    {
        return TrainingPool[ID];
    }


    [SerializeField]
    int TrainingPoolSize;

    private void CreateTrainingPool()
    {
        TrainingPool = new NeuralNetMainModule[TrainingPoolSize];
        for (int i = 0; i < TrainingPoolSize; i++)
        {
            TrainingPool[i] = new NeuralNetMainModule(4,4,4,4,4);
        }
    }
    
    [SerializeField]
    int NumberOfEliteSpeciesSelectedPerGeneration ; // Max value = 10
    NeuralNetMainModule[] GeneticElitePool;

    public void RunTrainingCycle()
    {
        Debug.Log(" Running Training Cycle ");
        TrainingCycleIsRun= true;

        float[][] inputs = new float[GetTrainingDataSize()][];
        int[] SetIDArray = new int[GetTrainingDataSize()];
        for (int i = 0; i < GetTrainingDataSize(); i++)
        {
            inputs[i] = GetTrainingSetByID(i);
           // Debug.Log("input " + i + " = " + inputs[i]);
            SetIDArray[i] = i;
        }
       
        for (int i = 0; i < TrainingPool.Length; i++)
        {
            for (int j = 0; j < GetTrainingDataSize(); j++)
            {
                //   Debug.Log(" Network " + i + " Training Cycle " + j);
               // if(UIManager.UIM.GetActiveNetworkID() != i)
                {
                    TrainingPool[i].RunNetworkCycle(inputs[j], out float[] Outputs);

                    TrainingPool[i].AddResultToTrainingResults(CalculateLoss(GetTrainingSetOutputByID(SetIDArray[j]), Outputs));
                }
                //else
                //{
                  //  TrainingPool[i].RunNetworkCyclePassDataToUI(inputs[j], out float[] Outputs);
                  //  TrainingPool[i].AddResultToTrainingResults(CalculateLoss(GetTrainingSetOutputByID(SetIDArray[j]), Outputs));
                //}
            }
        } // Train Each Net J number of times with the training Data
        Debug.Log(" ======================================= RESULTS ============================================ ");
        float[] Results = new float[TrainingPool.Length];
        float Sum = 0;
        for (int i = 0; i < TrainingPool.Length; i++)
        {

            Results[i] = TrainingPool[i].GetTrainingResult();
            Sum += Results[i];
            Debug.Log("Net " + i + " Result : " + Results[i]);
        }
        Debug.Log("Median Value : " + (Sum / TrainingPool.Length));

        UIManager.UIM.UpdateConnections();
        UIManager.UIM.UpdateNeuronValues();
    }
    public void RunSingleTrainingCycle()
    {
        Debug.Log(" Running Single Training Cycle ");
        TrainingCycleIsRun = true;

        float[][] inputs = new float[1][];
        int[] SetIDArray = new int[1];
        for (int i = 0; i < 1; i++)
        {
            inputs[i] = GetRandomTrainingSet(out int SetID);
            SetIDArray[i] = SetID;
        }

        for (int i = 0; i < TrainingPool.Length; i++)
        {
           
                //   Debug.Log(" Network " + i + " Training Cycle " + j);
                //if (UIManager.UIM.GetActiveNetworkID() != i)
                {

                    TrainingPool[i].RunNetworkCycle(inputs[0], out float[] Outputs);

                    TrainingPool[i].AddResultToTrainingResults(CalculateLoss(GetTrainingSetOutputByID(SetIDArray[0]), Outputs));
                }
                //else
                //{

                  //  TrainingPool[i].RunNetworkCyclePassDataToUI(inputs[0], out float[] Outputs);

                  //TrainingPool[i].AddResultToTrainingResults(CalculateLoss(GetTrainingSetOutputByID(SetIDArray[0]), Outputs));
                //}


            
        } // Train Each Net J number of times with the training Data
        Debug.Log(" ======================================= RESULTS ============================================ ");
        float[] Results = new float[TrainingPool.Length];
        float Sum = 0;
        for (int i = 0; i < TrainingPool.Length; i++)
        {

            Results[i] = TrainingPool[i].GetTrainingResult();
            Sum += Results[i];
            Debug.Log("Net " + i + " Result : " + Results[i]);
        }
        Debug.Log("Median Value : " + (Sum / TrainingPool.Length));

        UIManager.UIM.UpdateConnections();
        UIManager.UIM.UpdateNeuronValues();
    }
    public void CreateGeneticElitePool() 
    {
        if(TrainingCycleIsRun)
        {
        Debug.Log("Creating Genetic Elite Pool");

        List<NeuralNetMainModule> GeneticElitePool = new List<NeuralNetMainModule>();
        List<NeuralNetMainModule> TrainingPoolList = new List<NeuralNetMainModule>();

        for (int i = 0; i < TrainingPool.Length; i++)
        {
            TrainingPoolList.Add(TrainingPool[i]);
        }

        float LowestValue = 100;
       // Debug.Log("LowestValue " + LowestValue);
        int Counter = 0;
        int ValuesObtained = 0;
        int ID = 0;
       
        while (GeneticElitePool.Count < NumberOfEliteSpeciesSelectedPerGeneration)
        {
         //   Debug.Log("Cycle " + Counter);
            if(TrainingPoolList[Counter].GetTrainingResult() < LowestValue)
            {
                LowestValue = (TrainingPoolList[Counter].GetTrainingResult());    
                
              //  Debug.Log("LowestValue " + LowestValue);
             
                
                ID = Counter;
            }
            Counter++;
            if(Counter >= TrainingPoolList.Count)
            {
              //  Debug.Log("-----------Cycle Complete-----------");
           
                Counter = 0;
                GeneticElitePool.Add(TrainingPoolList[ID]);
              //  Debug.Log("Adding At ID " + ID);
                TrainingPoolList.RemoveAt(ID);
                LowestValue = 100;
            
                   
                

                ValuesObtained++;
            }
        }


        for (int i = 0; i < GeneticElitePool.Count; i++)
        {
          //  Debug.Log("GeneticElitePool : " + GeneticElitePool[i].GetTrainingResult());
        }

        // NeuralNetMainModule[] TempTrainingPool = new NeuralNetMainModule[GeneticElitePool.Count];
        //  Debug.Log("Creating TempArray");
        /* for (int i = 0; i < TempTrainingPool.Length; i++)
         {
            TempTrainingPool[i] = GeneticElitePool[i];
         } */
            Debug.Log("GeneticElitePoolSize" + GeneticElitePool.Count);
            Debug.Log("Creating New Training Pool");
            int Parent_1_ID = 0;
            int Parent_2_ID = 0;
            for (int i = 0; i < TrainingPool.Length; i++)
            {
                Parent_1_ID = Random.Range(0, GeneticElitePool.Count);
                Parent_2_ID = Random.Range(0, GeneticElitePool.Count);
               // Debug.Log("Parent_1_ID : " + Parent_1_ID);
               // Debug.Log("Parent_2_ID : " + Parent_2_ID);
                TrainingPool[i] = new NeuralNetMainModule(GeneticElitePool[Parent_1_ID].GetGenome().GetGenes() , GeneticElitePool[Parent_2_ID].GetGenome().GetGenes(), GeneticElitePool[Parent_1_ID].GetActivationFunction());
            }
            TrainingCycleIsRun = false;
        }

    }

    //---------------------------------------BACK PROP----------------------------------------------//

    [SerializeField]
    float TrainingRate;
    public float GetTrainingRate()
    {
        return TrainingRate;
    }
    public void ChangeLR(float delta)
    {
        TrainingRate += delta;
        if(TrainingRate > 1)
        {
            TrainingRate = 1;
        }
        else if(TrainingRate < 0)
        {
            TrainingRate = 0;
        }
        UIManager.UIM.UpdateLearningRate();
    }


    public void TrainAsAutoEncoder(int NetID, int ModelID)
    {

    }



    public void RunBackPropagationTrainingCycle()
    {
        Debug.Log(" Running BackPropagation Training Cycle ");

        float[][] inputs = new float[GetTrainingDataSize()][]; //Getting All The Training Data
        float[] Outputs = null;
        float[] IdealOutputs = null;
        int[] SetIDArray = new int[GetTrainingDataSize()];

        float[] LayerCostArray = null;

        for (int i = 0; i < GetTrainingDataSize(); i++)
        {
            inputs[i] = GetTrainingSetByID(i);
    
            SetIDArray[i] = i;
        }

        for (int i = 0; i < TrainingPool.Length; i++)
        {
              //   Debug.Log(">>>>>>>>>>>>>>>--------------------------BackPropagation Net[" + i + "]-----------------------<<<<<<<<<<<<<<<<");
            //cycle each net in the pool

            float Loss = 0;
            for (int j = 0; j < GetTrainingDataSize(); j++)
            {
                //for each net in the pool cycle the training sets 
                // Debug.Log("=============>>>>>>>>>>>>>>>>>>>> Training Cycle " + j + "<<<<<<<<<<<<<<<<<<<=================");
                NeuralNetMainModule.ActivationFunction AC = TrainingPool[i].GetActivationFunction();

                    TrainingPool[i].RunNetworkCycle(inputs[j], out Outputs);
        
                    IdealOutputs = GetTrainingSetOutputByID(SetIDArray[j]);

                    TrainingPool[i].AddResultToTrainingResults(CalculateLoss(IdealOutputs, Outputs));

                    LayerCostArray = new float[TrainingPool[i].GetNumberOfNeuronsAtLayer(TrainingPool[i].GetNumberOfLayers() - 1)];
                //LayerBValueChangeArray = new float[TrainingPool[i].GetNumberOfNeuronsAtLayer(TrainingPool[i].GetNumberOfLayers() - 2)];
                /* 
                 for (int CostIndex = 0; CostIndex < LayerCostArray.Length; CostIndex++)
                 {
                         //LayerCostArray[CostIndex] = Mathf.Pow((IdealOutputs[CostIndex] - Outputs[CostIndex]),2) ;

                         LayerCostArray[CostIndex] = 2*(IdealOutputs[CostIndex] - Outputs[CostIndex]);

                       Debug.Log("IdealOutputs[" + CostIndex + "] = " + IdealOutputs[CostIndex]);
                       Debug.Log("Outputs[" + CostIndex + "] = " + Outputs[CostIndex]);
                       Debug.Log("Cost For Output[" + CostIndex + "] = " + LayerCostArray[CostIndex]);
                 }//SetUp Base Output Layer Cost Values
                */
                /* for (int Layer = TrainingPool[i].GetNumberOfLayers() - 2; Layer >= 0; Layer--)
                 {
                    Debug.Log(" @Layer " + Layer);
                    for (int Neuron = 0; Neuron < TrainingPool[i].GetNumberOfNeuronsAtLayer(Layer); Neuron++)
                    {
                       Debug.Log(" @Neuron " + Neuron);
                       float BiasChange = 0;
                       for (int CostIndex = 0; CostIndex < LayerCostArray.Length; CostIndex++)
                       {
                         Debug.Log(" @Tracing Cost Neuron Id " + CostIndex);
                         if(TrainingPool[i].GetNeuron(Layer, Neuron).GetActivationValue() > 0)
                         {
                             TrainingPool[i].GetNeuron(Layer, Neuron).AddWeightChangeEntry((LayerCostArray[CostIndex] * TrainingRate) / Mathf.Clamp(TrainingPool[i].GetNeuron(Layer, Neuron).GetActivationValue(), 0.1f, 2), CostIndex);
                             Debug.Log(" > Setting Weight");
                         }
                         else if(TrainingPool[i].GetNeuron(Layer, Neuron).GetActivationValue() < 0)
                         {
                             TrainingPool[i].GetNeuron(Layer, Neuron).AddWeightChangeEntry((LayerCostArray[CostIndex] * TrainingRate) / -Mathf.Clamp(-TrainingPool[i].GetNeuron(Layer, Neuron).GetActivationValue(), 0.1f, 2), CostIndex);
                             Debug.Log(" > Setting Weight");
                         }
                         else 
                         {

                         }

                         BiasChange += LayerCostArray[CostIndex] * TrainingPool[i].GetNeuron(Layer, Neuron).GetWeightsList()[CostIndex];//Sigma 

                       }//cycle each Output with respect to Neuron(Neuron) and set the bias and weight

                     BiasChange *= TrainingRate; // bias * training weight
                     Debug.Log(" > Setting Bias");
                     // TrainingPool[i].GetNeuron(Layer, Neuron).AddBiasChangeEntry(BiasChange); //Set Bias
                    }

                    // LayerCostArray = new float[TrainingPool[i].GetNumberOfNeuronsAtLayer(Layer)];
                     float[] Temp_LayerCostArray = new float[TrainingPool[i].GetNumberOfNeuronsAtLayer(Layer)];

                    for (int CostIndex = 0; CostIndex < Temp_LayerCostArray.Length; CostIndex++)
                    {
                        Debug.Log("Propagating Error @ Neuron " + CostIndex);
                        float Sum = 0;
                        for (int N = 0; N < TrainingPool[i].GetNumberOfNeuronsAtLayer(Layer + 1); N++)
                        {
                         Sum += LayerCostArray[N] * TrainingPool[i].GetNeuron(Layer, CostIndex).GetWeightsList()[N];
                        } // Sum (Activation Value(M) * Weight(MN)
                        Temp_LayerCostArray[CostIndex] = Sum / TrainingPool[i].GetNumberOfNeuronsAtLayer(Layer + 1);
                        Debug.Log("LayerCostArray @ Neuron [" + CostIndex + "] = " + Temp_LayerCostArray[CostIndex]);
                    } //propagate error

                    LayerCostArray = Temp_LayerCostArray;

                 }*/
                NeuronScr[] PrevLayer = null;
                //   Debug.Log("========Setting C0 @ Output Layer =========");
                for (int Neuron = 0; Neuron < TrainingPool[i].GetNumberOfNeuronsAtLayer(TrainingPool[i].GetNumberOfLayers() - 1); Neuron++)
                {
                    //  Debug.Log("Setting C0 @ " + Neuron);
                    TrainingPool[i].GetNeuron
                      (TrainingPool[i].GetNumberOfLayers() - 1, Neuron)
                      .SetDelC0
                      (DelCost0DelAL
                      (IdealOutputs[Neuron],TrainingPool[i].GetNeuron
                      (TrainingPool[i].GetNumberOfLayers( ) - 1, Neuron)
                      .GetActivationValue()
                      ));

                    Loss += ((TrainingPool[i].GetNeuron(TrainingPool[i].GetNumberOfLayers() - 1, Neuron).GetActivationValue() - IdealOutputs[Neuron]) *
                        (TrainingPool[i].GetNeuron(TrainingPool[i].GetNumberOfLayers() - 1, Neuron).GetActivationValue() - IdealOutputs[Neuron]));

                } // setting layer L DelC0DelA

                for (int L = TrainingPool[i].GetNumberOfLayers()-2; L >= 0 ; L--)
                {//for each layer
                 //   Debug.Log("========Setting C0 @ Layer " + L + "=========");

                    PrevLayer = new NeuronScr[TrainingPool[i].GetNumberOfNeuronsAtLayer(L + 1)];
                    for (int Np = 0; Np < PrevLayer.Length; Np++)
                    {
                        PrevLayer[Np] = TrainingPool[i].GetNeuron(L +1 , Np);
                    }

                    for (int N = 0; N < TrainingPool[i].GetNumberOfNeuronsAtLayer(L); N++)
                    {//for each neuron
                     //    Debug.Log("Setting C0 @ Neuron " + N);
                        TrainingPool[i].GetNeuron(L,N).SetDelC0( DelCost0DelA( PrevLayer, TrainingPool[i].GetNeuron(L, N) , AC) );

                            for (int W = 0; W < TrainingPool[i].GetNeuron(L, N).GetWeightsList().Length; W++)
                            {                          
                               TrainingPool[i].GetNeuron(L, N).AddWeightChangeEntry(  - TrainingRate *DelADelW( PrevLayer[W], TrainingPool[i].GetNeuron(L, N), AC),W);
                            }
                    }
                }

                  for (int Layer =  0; Layer < TrainingPool[i].GetNumberOfLayers() - 1; Layer++)
                  {
                     for (int Neuron = 0; Neuron < TrainingPool[i].GetNumberOfNeuronsAtLayer(Layer); Neuron++)
                     {
                        //     Debug.Log("@Layer " + Layer + " @Neuron " + Neuron );
                        TrainingPool[i].GetNeuron(Layer, Neuron).ComitChanges();
                     }
                  }//Comit Changes

                
            }
            Debug.Log("                     ||||||||||============= Training Set Total Loss " + Loss + " ==========||||||||||");
        } 
    } //Deprecated

    public void RunBackPropagationTrainingCycle(int NetID,int ModelID)
    {
        Debug.Log(" Running BackPropagation Training Cycle ");

        TrainingModel TM = null;

        switch (ModelID)
        {
            case 0:
                TM = DataManager.DM.GetModel1();
                break;
            case 1:
                TM = DataManager.DM.GetModel2();
                break;
            case 2:
                TM = DataManager.DM.GetModel3();
                break;
            case 3:
                TM = DataManager.DM.GetModel4();
                break;
            case 4:
                TM = DataManager.DM.GetModel5();
                break;
            case 5:
                TM = DataManager.DM.GetModel6();
                break;
        }
         
        float[][] inputs = new float[TM.GetNumberOfTrainingSets()][]; //Getting All The Training Data
        float[] Outputs = null;
        float[] IdealOutputs = null;

        float[] LayerCostArray = null;

        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = TM.GetTrainingSetByID(i).GetInputs();
        }

        //Swap Last Set 
        //int RNG = Random.Range(0, inputs.Length);
        //float[] Copy = inputs[inputs.Length - 1];
        //inputs[inputs.Length - 1] = inputs[RNG];
        //inputs[RNG] = Copy;
        //---------------------------------------//


        Debug.Log(" Number Of Inputs " + inputs.Length);
        NeuralNetMainModule NNMM = DataManager.DM.GetNetworkRef(ModelID,NetID);

        float Loss = 0;
        NeuralNetMainModule.ActivationFunction ActFnc = NNMM.GetActivationFunction();
        for (int j = 0; j < inputs.Length; j++)
        {
            
            //for each net in the pool cycle the training sets 
            // Debug.Log("=============>>>>>>>>>>>>>>>>>>>> Training Cycle " + j + "<<<<<<<<<<<<<<<<<<<=================");

                NNMM.RunNetworkCyclePassDataToUI(inputs[j], out Outputs);

                IdealOutputs = TM.GetTrainingSetByID(j).GetOutputs();

                NNMM.AddResultToTrainingResults(CalculateLoss(IdealOutputs, Outputs));

                LayerCostArray = new float[NNMM.GetNumberOfNeuronsAtLayer(NNMM.GetNumberOfLayers() - 1)];

                //LayerBValueChangeArray = new float[TrainingPool[i].GetNumberOfNeuronsAtLayer(TrainingPool[i].GetNumberOfLayers() - 2)];
                /* 
                 for (int CostIndex = 0; CostIndex < LayerCostArray.Length; CostIndex++)
                 {
                         //LayerCostArray[CostIndex] = Mathf.Pow((IdealOutputs[CostIndex] - Outputs[CostIndex]),2) ;

                         LayerCostArray[CostIndex] = 2*(IdealOutputs[CostIndex] - Outputs[CostIndex]);

                       Debug.Log("IdealOutputs[" + CostIndex + "] = " + IdealOutputs[CostIndex]);
                       Debug.Log("Outputs[" + CostIndex + "] = " + Outputs[CostIndex]);
                       Debug.Log("Cost For Output[" + CostIndex + "] = " + LayerCostArray[CostIndex]);
                 }//SetUp Base Output Layer Cost Values
                */
                /* for (int Layer = TrainingPool[i].GetNumberOfLayers() - 2; Layer >= 0; Layer--)
                 {
                    Debug.Log(" @Layer " + Layer);
                    for (int Neuron = 0; Neuron < TrainingPool[i].GetNumberOfNeuronsAtLayer(Layer); Neuron++)
                    {
                       Debug.Log(" @Neuron " + Neuron);
                       float BiasChange = 0;
                       for (int CostIndex = 0; CostIndex < LayerCostArray.Length; CostIndex++)
                       {
                         Debug.Log(" @Tracing Cost Neuron Id " + CostIndex);
                         if(TrainingPool[i].GetNeuron(Layer, Neuron).GetActivationValue() > 0)
                         {
                             TrainingPool[i].GetNeuron(Layer, Neuron).AddWeightChangeEntry((LayerCostArray[CostIndex] * TrainingRate) / Mathf.Clamp(TrainingPool[i].GetNeuron(Layer, Neuron).GetActivationValue(), 0.1f, 2), CostIndex);
                             Debug.Log(" > Setting Weight");
                         }
                         else if(TrainingPool[i].GetNeuron(Layer, Neuron).GetActivationValue() < 0)
                         {
                             TrainingPool[i].GetNeuron(Layer, Neuron).AddWeightChangeEntry((LayerCostArray[CostIndex] * TrainingRate) / -Mathf.Clamp(-TrainingPool[i].GetNeuron(Layer, Neuron).GetActivationValue(), 0.1f, 2), CostIndex);
                             Debug.Log(" > Setting Weight");
                         }
                         else 
                         {

                         }

                         BiasChange += LayerCostArray[CostIndex] * TrainingPool[i].GetNeuron(Layer, Neuron).GetWeightsList()[CostIndex];//Sigma 

                       }//cycle each Output with respect to Neuron(Neuron) and set the bias and weight

                     BiasChange *= TrainingRate; // bias * training weight
                     Debug.Log(" > Setting Bias");
                     // TrainingPool[i].GetNeuron(Layer, Neuron).AddBiasChangeEntry(BiasChange); //Set Bias
                    }

                    // LayerCostArray = new float[TrainingPool[i].GetNumberOfNeuronsAtLayer(Layer)];
                     float[] Temp_LayerCostArray = new float[TrainingPool[i].GetNumberOfNeuronsAtLayer(Layer)];

                    for (int CostIndex = 0; CostIndex < Temp_LayerCostArray.Length; CostIndex++)
                    {
                        Debug.Log("Propagating Error @ Neuron " + CostIndex);
                        float Sum = 0;
                        for (int N = 0; N < TrainingPool[i].GetNumberOfNeuronsAtLayer(Layer + 1); N++)
                        {
                         Sum += LayerCostArray[N] * TrainingPool[i].GetNeuron(Layer, CostIndex).GetWeightsList()[N];
                        } // Sum (Activation Value(M) * Weight(MN)
                        Temp_LayerCostArray[CostIndex] = Sum / TrainingPool[i].GetNumberOfNeuronsAtLayer(Layer + 1);
                        Debug.Log("LayerCostArray @ Neuron [" + CostIndex + "] = " + Temp_LayerCostArray[CostIndex]);
                    } //propagate error

                    LayerCostArray = Temp_LayerCostArray;

                 }*/

                NeuronScr[] PrevLayer = null;

                //   Debug.Log("========Setting C0 @ Output Layer =========");

                int LastLayerID = NNMM.GetNumberOfLayers() - 1;
                NeuronScr neuronScr ;
                for (int Neuron = 0; Neuron < NNMM.GetNumberOfNeuronsAtLayer(LastLayerID); Neuron++)
                {
                    neuronScr = NNMM.GetNeuron(LastLayerID, Neuron);
                //  Debug.Log("Setting C0 @ " + Neuron);

                    neuronScr.SetDelC0( DelCost0DelAL(IdealOutputs[Neuron], neuronScr.GetActivationValue()));
                    neuronScr.AddBiasChangeEntry(-TrainingRate * DelADelZ(neuronScr, ActFnc));

                     Loss += ((neuronScr.GetActivationValue() - IdealOutputs[Neuron]) *
                        (neuronScr.GetActivationValue() - IdealOutputs[Neuron]));


                } // setting layer L DelC0DelA

                for (int L = NNMM.GetNumberOfLayers() - 2; L >= 0; L--)
                {   
                    //for each layer
                    //Debug.Log("========Setting C0 @ Layer " + L + "=========");
                    PrevLayer = new NeuronScr[NNMM.GetNumberOfNeuronsAtLayer(L + 1)];
                    for (int Np = 0; Np < PrevLayer.Length; Np++)
                    {
                        PrevLayer[Np] = NNMM.GetNeuron(L + 1, Np);
                    }

                    for (int N = 0; N < NNMM.GetNumberOfNeuronsAtLayer(L); N++)
                    {
                        //for each neuron
                        //Debug.Log("Setting C0 @ Neuron " + N);
                        NNMM.GetNeuron(L, N).SetDelC0( DelCost0DelA(PrevLayer, NNMM.GetNeuron(L, N), ActFnc));
                        NNMM.GetNeuron(L, N).AddBiasChangeEntry(-TrainingRate * DelADelZ( NNMM.GetNeuron(L, N) , ActFnc)    );
                        for (int W = 0; W < NNMM.GetNeuron(L, N).GetWeightsList().Length; W++)
                        {
                           NNMM.GetNeuron(L, N).AddWeightChangeEntry
                           (-TrainingRate * DelADelW(PrevLayer[W], NNMM.GetNeuron(L, N), ActFnc), W);
                        }
                    }
                }

                for (int Layer = 0; Layer < NNMM.GetNumberOfLayers(); Layer++)
                {
                    for (int Neuron = 0; Neuron < NNMM.GetNumberOfNeuronsAtLayer(Layer); Neuron++)
                    {
                        //Debug.Log("@Layer " + Layer + " @Neuron " + Neuron );
                        NNMM.GetNeuron(Layer, Neuron).ComitChanges();
                    }
                }//Comit Changes

                Debug.Log("||||||||||============= Training Set Total Loss " + Loss + " ==========||||||||||");
        }

        NNMM.SetLoss(Loss);
        NNMM.SetWasAlteredAfterSaving(true);
    }

    float[][] inputs = null;
    public void RunBackPropagationTrainingCycleAsAutoencoder(int NetID, int ModelID, int BatchSize)
    {
        Debug.Log(" Running BackPropagation Training Cycle As Autoencoder");

        TrainingModel TM = null;

        switch (ModelID)
        {
            case 0:
                TM = DataManager.DM.GetModel1();
                break;
            case 1:
                TM = DataManager.DM.GetModel2();
                break;
            case 2:
                TM = DataManager.DM.GetModel3();
                break;
            case 3:
                TM = DataManager.DM.GetModel4();
                break;
            case 4:
                TM = DataManager.DM.GetModel5();
                break;
            case 5:
                TM = DataManager.DM.GetModel6();
                break;
        }
        int NumberOfTrainingSets = TM.GetNumberOfTrainingSets();
        BatchSize = TM.GetNumberOfTrainingSets() < BatchSize ? TM.GetNumberOfTrainingSets() : BatchSize;

        inputs = new float[BatchSize][]; //Getting All The Training Data
        float[] Outputs = null;
        float[] IdealOutputs = null;

        float[] LayerCostArray = null;
        float a = 0;
        float[] MaxArray = TM.GetInputMaxArray();
        float[] MinArray = TM.GetInputMinArray();
        for (int i = 0; i < inputs.Length; i++)
        {
           // Debug.Log("---------------- Batch Set[" + i + "] ----------------");
            int Count = 0;
            RESTART:;
            int Id = Random.Range(0, NumberOfTrainingSets);
            inputs[i] = new float [TM.GetTrainingSetByID(Id).GetInputs().Length];
            for (int l = 0; l < inputs[i].Length; l++)
            {
                inputs[i][l] = TM.GetTrainingSetByID(Id).GetInputs()[l]/MaxArray[l]; ;
            }
            //for (int j = 0; j < inputs[i].Length; j++)
            //{
               
            //   // a = inputs[i][j];
            //   //// Debug.Log("[" + j + "] = " + a);
                
            //   // if (a > MaxArray[j] || a < MinArray[j])
            //   // {
            //   //     Debug.LogError("Corrupted Area " + a + " id " + Id + " j " + j);
            //   //     Count++;
            //   //     if(Count < 20)
            //   //     goto RESTART;
                   
            //   // }
            //   // else
            //   // {
            //   //     //inputs[i][j] /= MaxArray[j]; ;
            //   // }


            //    //if(inputs[i][j] > 1)
            //    //{
            //    //    Debug.LogError(" i > 1 @ " + i + "/" + j);
            //    //    Debug.LogError("TM.GetInputMaxArray()[j] " + TM.GetInputMaxArray()[j]);
            //    //    Debug.LogError("inputs[i][j] " + a);
            //    //    Debug.LogError("Set ID " + Id);
            //    //    Debug.LogError("Set [0] " + TM.GetTrainingSetByID(Id).GetInputs()[0]);
            //    //    Debug.LogError("Set [1] " + TM.GetTrainingSetByID(Id).GetInputs()[1]);
            //    //    Debug.LogError("Set [2] " + TM.GetTrainingSetByID(Id).GetInputs()[2]);
            //    //    Debug.LogError("Set [3] " + TM.GetTrainingSetByID(Id).GetInputs()[3]);
            //    //    Debug.LogError("Set [4] " + TM.GetTrainingSetByID(Id).GetInputs()[4]);
            //    //    Debug.LogError("Set [5] " + TM.GetTrainingSetByID(Id).GetInputs()[5]);
            //    //    Debug.LogError("Set [6] " + TM.GetTrainingSetByID(Id).GetInputs()[6]);
            //    //    Debug.LogError("Set [7] " + TM.GetTrainingSetByID(Id).GetInputs()[7]);
            //    //    Debug.LogError("Set [8] " + TM.GetTrainingSetByID(Id).GetInputs()[8]);
            //    //    Debug.LogError("Set [9] " + TM.GetTrainingSetByID(Id).GetInputs()[9]);
            //    //}
            //}
        }

        //Swap Last Set 
        //int RNG = Random.Range(0, inputs.Length);
        //float[] Copy = inputs[inputs.Length - 1];
        //inputs[inputs.Length - 1] = inputs[RNG];
        //inputs[RNG] = Copy;
        //---------------------------------------//


      // Debug.Log(" Number Of Inputs " + inputs.Length);
        NeuralNetMainModule NNMM = DataManager.DM.GetNetworkRef(ModelID, NetID);

        float Loss = 0;
        NeuralNetMainModule.ActivationFunction ActFnc = NNMM.GetActivationFunction();
        for (int j = 0; j < inputs.Length; j++)
        {

            //for each net in the pool cycle the training sets 
            // Debug.Log("=============>>>>>>>>>>>>>>>>>>>> Training Cycle " + j + "<<<<<<<<<<<<<<<<<<<=================");

            NNMM.RunNetworkCyclePassDataToUI(inputs[j], out Outputs);

            IdealOutputs = inputs[j];

            NNMM.AddResultToTrainingResults(CalculateLoss(IdealOutputs, Outputs));

            LayerCostArray = new float[NNMM.GetNumberOfNeuronsAtLayer(NNMM.GetNumberOfLayers() - 1)];

            //LayerBValueChangeArray = new float[TrainingPool[i].GetNumberOfNeuronsAtLayer(TrainingPool[i].GetNumberOfLayers() - 2)];
            /* 
             for (int CostIndex = 0; CostIndex < LayerCostArray.Length; CostIndex++)
             {
                     //LayerCostArray[CostIndex] = Mathf.Pow((IdealOutputs[CostIndex] - Outputs[CostIndex]),2) ;

                     LayerCostArray[CostIndex] = 2*(IdealOutputs[CostIndex] - Outputs[CostIndex]);

                   Debug.Log("IdealOutputs[" + CostIndex + "] = " + IdealOutputs[CostIndex]);
                   Debug.Log("Outputs[" + CostIndex + "] = " + Outputs[CostIndex]);
                   Debug.Log("Cost For Output[" + CostIndex + "] = " + LayerCostArray[CostIndex]);
             }//SetUp Base Output Layer Cost Values
            */
            /* for (int Layer = TrainingPool[i].GetNumberOfLayers() - 2; Layer >= 0; Layer--)
             {
                Debug.Log(" @Layer " + Layer);
                for (int Neuron = 0; Neuron < TrainingPool[i].GetNumberOfNeuronsAtLayer(Layer); Neuron++)
                {
                   Debug.Log(" @Neuron " + Neuron);
                   float BiasChange = 0;
                   for (int CostIndex = 0; CostIndex < LayerCostArray.Length; CostIndex++)
                   {
                     Debug.Log(" @Tracing Cost Neuron Id " + CostIndex);
                     if(TrainingPool[i].GetNeuron(Layer, Neuron).GetActivationValue() > 0)
                     {
                         TrainingPool[i].GetNeuron(Layer, Neuron).AddWeightChangeEntry((LayerCostArray[CostIndex] * TrainingRate) / Mathf.Clamp(TrainingPool[i].GetNeuron(Layer, Neuron).GetActivationValue(), 0.1f, 2), CostIndex);
                         Debug.Log(" > Setting Weight");
                     }
                     else if(TrainingPool[i].GetNeuron(Layer, Neuron).GetActivationValue() < 0)
                     {
                         TrainingPool[i].GetNeuron(Layer, Neuron).AddWeightChangeEntry((LayerCostArray[CostIndex] * TrainingRate) / -Mathf.Clamp(-TrainingPool[i].GetNeuron(Layer, Neuron).GetActivationValue(), 0.1f, 2), CostIndex);
                         Debug.Log(" > Setting Weight");
                     }
                     else 
                     {

                     }

                     BiasChange += LayerCostArray[CostIndex] * TrainingPool[i].GetNeuron(Layer, Neuron).GetWeightsList()[CostIndex];//Sigma 

                   }//cycle each Output with respect to Neuron(Neuron) and set the bias and weight

                 BiasChange *= TrainingRate; // bias * training weight
                 Debug.Log(" > Setting Bias");
                 // TrainingPool[i].GetNeuron(Layer, Neuron).AddBiasChangeEntry(BiasChange); //Set Bias
                }

                // LayerCostArray = new float[TrainingPool[i].GetNumberOfNeuronsAtLayer(Layer)];
                 float[] Temp_LayerCostArray = new float[TrainingPool[i].GetNumberOfNeuronsAtLayer(Layer)];

                for (int CostIndex = 0; CostIndex < Temp_LayerCostArray.Length; CostIndex++)
                {
                    Debug.Log("Propagating Error @ Neuron " + CostIndex);
                    float Sum = 0;
                    for (int N = 0; N < TrainingPool[i].GetNumberOfNeuronsAtLayer(Layer + 1); N++)
                    {
                     Sum += LayerCostArray[N] * TrainingPool[i].GetNeuron(Layer, CostIndex).GetWeightsList()[N];
                    } // Sum (Activation Value(M) * Weight(MN)
                    Temp_LayerCostArray[CostIndex] = Sum / TrainingPool[i].GetNumberOfNeuronsAtLayer(Layer + 1);
                    Debug.Log("LayerCostArray @ Neuron [" + CostIndex + "] = " + Temp_LayerCostArray[CostIndex]);
                } //propagate error

                LayerCostArray = Temp_LayerCostArray;

             }*/

            NeuronScr[] PrevLayer = null;

            //   Debug.Log("========Setting C0 @ Output Layer =========");

            int LastLayerID = NNMM.GetNumberOfLayers() - 1;
            NeuronScr neuronScr;
            for (int Neuron = 0; Neuron < NNMM.GetNumberOfNeuronsAtLayer(LastLayerID); Neuron++)
            {
                neuronScr = NNMM.GetNeuron(LastLayerID, Neuron);
                //  Debug.Log("Setting C0 @ " + Neuron);

                neuronScr.SetDelC0(DelCost0DelAL(IdealOutputs[Neuron], neuronScr.GetActivationValue()));
                neuronScr.AddBiasChangeEntry(-TrainingRate * DelADelZ(neuronScr, ActFnc));

                Loss += ((neuronScr.GetActivationValue() - IdealOutputs[Neuron]) *
                   (neuronScr.GetActivationValue() - IdealOutputs[Neuron]));


            } // setting layer L DelC0DelA

            for (int L = NNMM.GetNumberOfLayers() - 2; L >= 0; L--)
            {
                //for each layer
                //Debug.Log("========Setting C0 @ Layer " + L + "=========");
                PrevLayer = new NeuronScr[NNMM.GetNumberOfNeuronsAtLayer(L + 1)];
                for (int Np = 0; Np < PrevLayer.Length; Np++)
                {
                    PrevLayer[Np] = NNMM.GetNeuron(L + 1, Np);
                }

                for (int N = 0; N < NNMM.GetNumberOfNeuronsAtLayer(L); N++)
                {
                    //for each neuron
                    //Debug.Log("Setting C0 @ Neuron " + N);
                    NNMM.GetNeuron(L, N).SetDelC0(DelCost0DelA(PrevLayer, NNMM.GetNeuron(L, N), ActFnc));
                    NNMM.GetNeuron(L, N).AddBiasChangeEntry(-TrainingRate * DelADelZ(NNMM.GetNeuron(L, N), ActFnc));
                    for (int W = 0; W < NNMM.GetNeuron(L, N).GetWeightsList().Length; W++)
                    {
                        NNMM.GetNeuron(L, N).AddWeightChangeEntry
                        (-TrainingRate * DelADelW(PrevLayer[W], NNMM.GetNeuron(L, N), ActFnc), W);
                    }
                }
            }

            for (int Layer = 0; Layer < NNMM.GetNumberOfLayers(); Layer++)
            {
                for (int Neuron = 0; Neuron < NNMM.GetNumberOfNeuronsAtLayer(Layer); Neuron++)
                {
                    //Debug.Log("@Layer " + Layer + " @Neuron " + Neuron );
                    NNMM.GetNeuron(Layer, Neuron).ComitChanges();
                }
            }//Comit Changes

            Debug.Log("||||||||||============= Training Set Total Loss " + Loss + " ==========||||||||||");
        }

        NNMM.SetLoss(Loss);
        NNMM.SetWasAlteredAfterSaving(true);
    }


    [SerializeField]
    int NumberOfAutoCycleGenerations;

    [SerializeField]
    float MutationChance,MaxMutationMagnitue;
    
    public static float GlobalMutationChance;
    public static float GlobalMaxMutationMagnitue;

    private float Sum(float[] Array , float sumA , float sumB , float sumC, bool multiply)
    {
        float sum = 0;
        if(multiply)
        {
            for (int i = 0; i < Array.Length; i++)
            {
                sum += Array[i] * sumA * sumB * sumC;
            }
        }
        else
        {
            for (int i = 0; i < Array.Length; i++)
            {
                sum += Array[i] + sumA + sumB + sumC;
            }
        }
    
        return sum;
    }
    private float Sum(float[] Array, float sumA, float sumB, bool multiply)
    {
        float sum = 0;
        if (multiply)
        {
            for (int i = 0; i < Array.Length; i++)
            {
                sum += Array[i] * sumA * sumB ;
            }
        }
        else
        {
            for (int i = 0; i < Array.Length; i++)
            {
                sum += Array[i] + sumA + sumB ;
            }
        }
        return sum;
    }
    private float Sum(float[] Array, float sumA, bool multiply)
    {
        float sum = 0;
        if (multiply)
        {
            for (int i = 0; i < Array.Length; i++)
            {
                sum += Array[i] * sumA;
            }
        }
        else
        {
            for (int i = 0; i < Array.Length; i++)
            {
                sum += Array[i] + sumA ;
            }
        }
        return sum;
    }


    private float DelCost0DelAL(float Y,float AL)
    {
        //Y = optimal value for neuron N of the output layer
        // AL = activation value of neuron N at output layer L
        float value = 2 * (AL - Y);
      //  Debug.Log("DelC0  = " + value);
        return value;

    } // Derivative of C0 with respect to the activation value of a neuron in the output layer
    private float DelCost0DelA(NeuronScr[] PrevLayer,NeuronScr A , NeuralNetMainModule.ActivationFunction AC)
    {
    
        float value = 0;
        for (int i = 0; i < PrevLayer.Length; i++)
        {
            switch (AC)
            {
                case NeuralNetMainModule.ActivationFunction.Linear:
                    Debug.LogError("Linear Activation Function Not Implemented");
                    break;
                case NeuralNetMainModule.ActivationFunction.BinaryStep:
                    Debug.LogError("BinaryStep Activation Function Not Implemented");
                    break;
                case NeuralNetMainModule.ActivationFunction.Sigmoid:
                    value +=

                PrevLayer[i].GetDelC0() *

                (1 / (1 + Mathf.Exp(PrevLayer[i].GetSigmaValue()))) * (1 - (1 / (1 + Mathf.Exp(PrevLayer[i].GetSigmaValue())))) *

                A.GetWeightsList()[i]
                ;
                    break;
                case NeuralNetMainModule.ActivationFunction.HyperbolicTangent:
                    value +=

                PrevLayer[i].GetDelC0() *

                (2 / (Mathf.Exp(PrevLayer[i].GetSigmaValue()) + Mathf.Exp(-PrevLayer[i].GetSigmaValue())  )) * (2 / (Mathf.Exp(PrevLayer[i].GetSigmaValue()) + Mathf.Exp(-PrevLayer[i].GetSigmaValue()))) *

                A.GetWeightsList()[i]
                ;
                    break;
                default:
                    Debug.LogError("BinaryStep Activation Function Not Implemented");
                    break;
            }
            

          //  Debug.Log("DelC0 (L+1) i = " + PrevLayer[i].GetDelC0());
            //Debug.Log("DelSigmoid = " + (1 / (1 + Mathf.Exp(PrevLayer[i].GetSigmaValue()))) * (1 - (1 / (1 + Mathf.Exp(PrevLayer[i].GetSigmaValue())))));
           // Debug.Log("Del Z Del W = " + A.GetWeightsList()[i]);
        }
        A.SetDelC0(value);
       // Debug.Log("DelC0DelA " + value);
        return value;
    } // Derivative of C0 with respect to the activation value of a neuron in layer X
    private float DelADelW(NeuronScr A, NeuronScr W, NeuralNetMainModule.ActivationFunction AC)
    {
        float

        value = 0;
        switch (AC)
        {
            case NeuralNetMainModule.ActivationFunction.Linear:
                Debug.LogError("Linear Activation Function Not Implemented");
                break;
            case NeuralNetMainModule.ActivationFunction.BinaryStep:
                Debug.LogError("Linear Activation Function Not Implemented");
                break;
            case NeuralNetMainModule.ActivationFunction.Sigmoid:
                value = W.GetActivationValue() *
                   (1 / (1 + Mathf.Exp(A.GetSigmaValue()))) *
                   (1 - (1 / (1 + Mathf.Exp(A.GetSigmaValue())))) *
                    A.GetDelC0();
                break;
            case NeuralNetMainModule.ActivationFunction.HyperbolicTangent:
                value = W.GetActivationValue() *
                     (2 / (Mathf.Exp(A.GetSigmaValue()) + Mathf.Exp(-A.GetSigmaValue()))) *
                     (2 / (Mathf.Exp(A.GetSigmaValue()) + Mathf.Exp(-A.GetSigmaValue()))) *
                     A.GetDelC0();

                break;
            default:
                break;
        }

        return value;
    }
    private float DelADelZ(NeuronScr AZ, NeuralNetMainModule.ActivationFunction AC)
    {
        float

        value = 0;
        switch (AC)
        {
            case NeuralNetMainModule.ActivationFunction.Linear:
                Debug.LogError("Linear Activation Function Not Implemented");
                break;
            case NeuralNetMainModule.ActivationFunction.BinaryStep:
                Debug.LogError("Linear Activation Function Not Implemented");
                break;
            case NeuralNetMainModule.ActivationFunction.Sigmoid:

                value =
                   (1 / (1 + Mathf.Exp(AZ.GetSigmaValue()))) *
                   (1 - (1 / (1 + Mathf.Exp(AZ.GetSigmaValue())))) *
                    AZ.GetDelC0();

                break;
            case NeuralNetMainModule.ActivationFunction.HyperbolicTangent:
                value = 
                     (2 / (Mathf.Exp(AZ.GetSigmaValue()) + Mathf.Exp(-AZ.GetSigmaValue()))) *
                     (2 / (Mathf.Exp(AZ.GetSigmaValue()) + Mathf.Exp(-AZ.GetSigmaValue()))) *
                     AZ.GetDelC0();

                break;
            default:
                break;
        }

        return value;
    }

    /* Testing

    float[] Layer1 = new float[4];
    float[] Layer0 = new float[4];

    float[] IdealOutputs = new float[4];
    float[] ErrorArray = new float[4];
    float[] ErrorMagnitude = new float[4];

    float[,] Weights = new float[4,4];

    float totalError = 0;

    bool isSet = false;

   
    public void TestFunc()
    {
       if(!isSet)
       {
            isSet = true;
            Debug.Log("SetUp@");
            Layer1[0] = 0; Layer1[1] = 0; Layer1[2] = 0; Layer1[3] = 0;

            Layer0[0] = 1; Layer0[1] = 1; Layer0[2] = 1; Layer0[3] = 1;

            IdealOutputs[0] = 1; IdealOutputs[1] = 0; IdealOutputs[2] = 0; IdealOutputs[3] = 0.5f;

            Weights[0, 0] = 0.5f; Weights[0, 1] = 0.5f; Weights[0, 2] = 0.5f; Weights[0, 3] = 0.5f;
            Weights[1, 0] = 0.5f; Weights[1, 1] = 0.5f; Weights[1, 2] = 0.5f; Weights[1, 3] = 0.5f;
            Weights[2, 0] = 0.5f; Weights[2, 1] = 0.5f; Weights[2, 2] = 0.5f; Weights[2, 3] = 0.5f;
            Weights[3, 0] = 0.5f; Weights[3, 1] = 0.5f; Weights[3, 2] = 0.5f; Weights[3, 3] = 0.5f;
        }
       else
       {
            for (int i = 0; i < 4; i++)
            {
                Layer1[i] = 0;
                for (int j = 0; j < 4; j++)
                {
                    
                    Layer1[i] += Layer0[j] * Weights[j,i]/4;
                }
                ErrorArray[i] = 2 * ( IdealOutputs[i] - Layer1[i]);
                ErrorMagnitude[i] = (IdealOutputs[i] - Layer1[i]) * (IdealOutputs[i] - Layer1[i]);
        
                //Debug.Log("Layer1@" + i + " = " + Layer1[i]);
                //Debug.Log("ErrorMagnitude@" + i + " = " + ErrorMagnitude[i]);
                //Debug.Log("Error@" + i + " = " + ErrorArray[i]);
                Debug.Log("ErrorCalc@" + i + " = " + ErrorMagnitude[i] * ErrorArray[i]);
                totalError += ErrorMagnitude[i];
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    float Correction = ((ErrorArray[i] * ErrorMagnitude[i]) * 0.05f) / Layer0[i];
                    Debug.Log("Correction @ " + i + "," + j + " : " + Correction);
                    Weights[i, j] += Correction;
                }

            }

            Debug.Log("Total Error " + totalError);
            Debug.Log("I[0] " + IdealOutputs[0] + " I[1] " + IdealOutputs[1] + " I[2] " + IdealOutputs[2] + " I[3] " + IdealOutputs[3]);
            Debug.Log("O[0] " + Layer1[0] + " O[1] " + Layer1[1] + " O[2] " + Layer1[2] + " O[3] " + Layer1[3]);
        }
    }

    */

    //----------------------------------SimSpaceTraining-----------------------//

    int Cycles;

    bool IsDoingTrainingCycle = false;

    bool PlayPause;

    public int GetCycleCount()
    {
        return Cycles;
    }

    float CycleRate = 0.5f;

    public void ChangeCycleRate(float delta)
    {
        CycleRate += delta;
 
        if (CycleRate > 2 )
        {
            CycleRate = 2;
        }
        else if(CycleRate < 0.1f)
        {
            CycleRate = 0.1f;
        }
        UIManager.UIM.UpdateCycleRate();
    }

    public void PauseCycles()
    {
        PlayPause = !PlayPause;
        UIManager.UIM.CyclePlayPauseBTN(PlayPause);
    }

    public float GetCycleRate()
    {
        return CycleRate;
    }

    public void RunTrainingSetsFromData()
    {
        UIManager.UIM.SetUpCustomInputs(false);
        if (!IsDoingTrainingCycle)
        {
            StopTraining();
            Cycles = 100;
            PlayPause = true;
           
            if(DataManager.DM.GetNetworkRef(UIManager.UIM.GetSelectedModelID(), UIManager.UIM.GetSelectedNetworkID()).GetMyTrainingType() == TrainingModel.NetworkType.Supervised)
            {

                RunBackPropagationTrainingCycle(UIManager.UIM.GetSelectedNetworkID(), UIManager.UIM.GetSelectedModelID());
            }
            else
            {
                RunBackPropagationTrainingCycleAsAutoencoder(UIManager.UIM.GetSelectedNetworkID(), UIManager.UIM.GetSelectedModelID(), 20);
            }
            UIManager.UIM.UpdateNeuronValues();
            UIManager.UIM.UpdateNetLossInfoData();
            UIManager.UIM.UpdateCycleCount();
            UIManager.UIM.CyclePlayPauseBTN(PlayPause);
            StartCoroutine(CycleRateRoutine());

        }
    }

    public void RunRandomTrainingTests()
    {
       UIManager.UIM. SetUpCustomInputs(false);
        Cycles = 10;
        StopTraining();
        float[] InputArray = null;
        float[] Min = null;
        float[] Max = null;

        switch (UIManager.UIM.GetSelectedModelID())
        {
            case 0:
                InputArray = new float[DataManager.DM.GetModel1().GetNumberOfInputs()];
                Min = DataManager.DM.GetModel1().GetInputRangeMin();
                Max = DataManager.DM.GetModel1().GetInputRangeMax();
                break;
            case 1:
                InputArray = new float[DataManager.DM.GetModel2().GetNumberOfInputs()];
                Min = DataManager.DM.GetModel2().GetInputRangeMin();
                Max = DataManager.DM.GetModel2().GetInputRangeMax();
                break;
            case 2:
                InputArray = new float[DataManager.DM.GetModel3().GetNumberOfInputs()];
                Min = DataManager.DM.GetModel3().GetInputRangeMin();
                Max = DataManager.DM.GetModel3().GetInputRangeMax();
                break;
            case 3:
                InputArray = new float[DataManager.DM.GetModel4().GetNumberOfInputs()];
                Min = DataManager.DM.GetModel4().GetInputRangeMin();
                Max = DataManager.DM.GetModel4().GetInputRangeMax();
                break;
            case 4:
                InputArray = new float[DataManager.DM.GetModel5().GetNumberOfInputs()];
                Min = DataManager.DM.GetModel5().GetInputRangeMin();
                Max = DataManager.DM.GetModel5().GetInputRangeMax();
                break;
            case 5:
                InputArray = new float[DataManager.DM.GetModel6().GetNumberOfInputs()];
                Min = DataManager.DM.GetModel6().GetInputRangeMin();
                Max = DataManager.DM.GetModel6().GetInputRangeMax();
                break;
        }

        for (int i = 0; i < InputArray.Length; i++)
        {
            InputArray[i] = Random.Range(Min[i], Max[i]);
            Debug.Log("RandomInput @ " + i + " = " + InputArray[i]);
        }

         DataManager.DM.GetNetworkRef(UIManager.UIM.GetSelectedModelID(), UIManager.UIM.GetSelectedNetworkID())
            .RunNetworkCyclePassDataToUI(InputArray,out float[] Outputs);
        UIManager.UIM.UpdateNeuronValues();
        UIManager.UIM.UpdateNetLossInfoData();
        UIManager.UIM.UpdateCycleCount();
        UIManager.UIM.CyclePlayPauseBTN(PlayPause);
        PlayPause = true;
        StartCoroutine(CycleRandomInputRoutine());

    }// this function does not train the network Only use for testing purpose
 
    public void RunCustomInputs()
    {
        StopTraining();
        float[] InputArray = null;
        UIManager.UIM.GetCustomInputValues();
        switch (UIManager.UIM.GetSelectedModelID())
        {
            case 0:
                InputArray = new float[DataManager.DM.GetModel1().GetNumberOfInputs()];
                break;
            case 1:
                InputArray = new float[DataManager.DM.GetModel2().GetNumberOfInputs()];            
                break;
            case 2:
                InputArray = new float[DataManager.DM.GetModel3().GetNumberOfInputs()];
                break;
            case 3:
                InputArray = new float[DataManager.DM.GetModel4().GetNumberOfInputs()];
                break;
            case 4:
                InputArray = new float[DataManager.DM.GetModel5().GetNumberOfInputs()];
                break;
            case 5:
                InputArray = new float[DataManager.DM.GetModel6().GetNumberOfInputs()];
                break;
        }
        float[] array = UIManager.UIM.GetCustomInputValues();
        for (int i = 0; i < InputArray.Length; i++)
        {
            InputArray[i] = array[i];
        }

        DataManager.DM.GetNetworkRef(UIManager.UIM.GetSelectedModelID(), UIManager.UIM.GetSelectedNetworkID())
         .RunNetworkCyclePassDataToUI(InputArray, out float[] Outputs);

        UIManager.UIM.UpdateNeuronValues();
        UIManager.UIM.UpdateNetLossInfoData();

    }// this function does not train the network only use for testing purpose 

    public void StopTraining()
    {
        StopAllCoroutines();
        IsDoingTrainingCycle = false;
        PlayPause = false;
    }

    IEnumerator CycleRateRoutine()
    {
        if(PlayPause)
        {
            Reset:;
           
           float timer = CycleRate;
           Debug.Log("CycleRateRoutine");
           while(timer > 0)
           {
              timer -= Time.deltaTime;
              yield return null;
           }
           if(!PlayPause && Cycles > 0)
           {
                goto Reset;
           }
           if (PlayPause && Cycles > 0)
           {
              Cycles--;
                if (DataManager.DM.GetNetworkRef(UIManager.UIM.GetSelectedModelID(), UIManager.UIM.GetSelectedNetworkID()).GetMyTrainingType() == TrainingModel.NetworkType.Supervised)
                {

                    RunBackPropagationTrainingCycle(UIManager.UIM.GetSelectedNetworkID(), UIManager.UIM.GetSelectedModelID());
                }
                else
                {
                    RunBackPropagationTrainingCycleAsAutoencoder(UIManager.UIM.GetSelectedNetworkID(), UIManager.UIM.GetSelectedModelID(), 20);
                }
               
              UIManager.UIM.UpdateNeuronValues();
              UIManager.UIM.UpdateNetLossInfoData();
              UIManager.UIM.UpdateCycleCount();
              StartCoroutine(CycleRateRoutine());
           }
        }
    }

    IEnumerator CycleRandomInputRoutine()
    {
        if (PlayPause)
        {
            Debug.Log("CycleRandomInputRoutine");

            Reset:;

            float timer = CycleRate;
            while(timer > 0)
            {
               timer -= Time.deltaTime;
               yield return null;
            }
            if(!PlayPause && Cycles > 0 )
            {
                goto Reset;
            }
            else if(PlayPause && Cycles > 0)
            {
                Cycles--;
                float[] InputArray = null; float[] Min = null; float[] Max = null;

                switch (UIManager.UIM.GetSelectedModelID())
                {
                    case 0:
                        InputArray = new float[DataManager.DM.GetModel1().GetNumberOfInputs()];
                        Min = DataManager.DM.GetModel1().GetInputRangeMin();
                        Max = DataManager.DM.GetModel1().GetInputRangeMax();
                        break;
                    case 1:
                        InputArray = new float[DataManager.DM.GetModel2().GetNumberOfInputs()];
                        Min = DataManager.DM.GetModel2().GetInputRangeMin();
                        Max = DataManager.DM.GetModel2().GetInputRangeMax();
                        break;
                    case 2:
                        InputArray = new float[DataManager.DM.GetModel3().GetNumberOfInputs()];
                        Min = DataManager.DM.GetModel3().GetInputRangeMin();
                        Max = DataManager.DM.GetModel3().GetInputRangeMax();
                        break;
                    case 3:
                        InputArray = new float[DataManager.DM.GetModel4().GetNumberOfInputs()];
                        Min = DataManager.DM.GetModel4().GetInputRangeMin();
                        Max = DataManager.DM.GetModel4().GetInputRangeMax();
                        break;
                    case 4:
                        InputArray = new float[DataManager.DM.GetModel5().GetNumberOfInputs()];
                        Min = DataManager.DM.GetModel5().GetInputRangeMin();
                        Max = DataManager.DM.GetModel5().GetInputRangeMax();
                        break;
                    case 5:
                        InputArray = new float[DataManager.DM.GetModel6().GetNumberOfInputs()];
                        Min = DataManager.DM.GetModel6().GetInputRangeMin();
                        Max = DataManager.DM.GetModel6().GetInputRangeMax();
                        break;
                }

                for (int i = 0; i < InputArray.Length; i++)
                {
                    InputArray[i] = Random.Range(Min[i], Max[i]);
                    Debug.Log("RandomInput @ " + i + " = " + InputArray[i]);
                }

                DataManager.DM.GetNetworkRef(UIManager.UIM.GetSelectedModelID()
                  ,UIManager.UIM.GetSelectedNetworkID()).RunNetworkCyclePassDataToUI(InputArray, out float[] Outputs);
                UIManager.UIM.UpdateNeuronValues();
                UIManager.UIM.UpdateNetLossInfoData();
                UIManager.UIM.UpdateCycleCount();
                StartCoroutine(CycleRandomInputRoutine());
            }
        }
    }






}