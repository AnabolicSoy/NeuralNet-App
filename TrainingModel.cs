using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor.UIElements;
using UnityEngine;


public class TrainingModel : MonoBehaviour
{
    //----------------------------Custom Input ID Mapping--------------------------//

    int[] CustomInputMapping;
    int[] CustomOutputMapping;
    
    public void SetCustomInputMapAtID(int ID, int NewValue)
    { CustomInputMapping[ID] = NewValue;}
    public void SetCustomOutputMapAtID(int ID, int NewValue)
    { CustomOutputMapping[ID] = NewValue;}

    public int[] GetCustomInputMapping()
    { return CustomInputMapping;}
    public int[] GetCustomOutputMapping()
    { return CustomOutputMapping;}
    public void SetCustomInputMapping(int[] array) {// Debug.Log("Setting CIM");
                                                     CustomInputMapping = array; }
    public void SetCustomOutputMapping(int[] array) { //Debug.Log("Setting COM");
                                                      CustomOutputMapping = array; }

    //-----------------------------------Default InputOutput ID Mapping-----------------------------//

    int[] OutputIDMapping;
    int[] InputIDMapping;

    public int GetNumberOfInputs()
    {
        if(InputIDMapping == null)
        {
            Debug.Log("TrainingSets.Length " + TrainingSets.Length);
            if(TrainingSets.Length > 0)
            {
                return TrainingSets[0].GetInputs().Length;
            }
            else
            {
                return NumberOfInputs;
            }

        }
        else
        {
            return InputIDMapping.Length;
        }
       
    }
    public int GetNumberOfOutputs()
    {
        if (OutputIDMapping == null)
        {
            Debug.Log("TrainingSets.Length " + TrainingSets.Length);
            if (TrainingSets.Length > 0)
            {
                return TrainingSets[0].GetInputs().Length;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            return OutputIDMapping.Length;
        }
    }

    //----------------Default Input Accepted Value Range For Network Model---------//

    float[] InputRangeMin;
    float[] InputRangeMax;

    public float[] GetInputRangeMin() 
    { 
        if (InputRangeMin == null)
        {

            float[] a = new float[TrainingSets[0].GetInputs().Length];

            for (int i = 0; i < a.Length; i++)
            {
                a[i] = -1;
            }
            return a;
        }
        else
        {
            return InputRangeMin;
        }
    }
    public float[] GetInputRangeMax()
    {
        if (InputRangeMax == null)
        {

            float[] a = new float[TrainingSets[0].GetInputs().Length];

            for (int i = 0; i < a.Length; i++)
            {
                a[i] = 1;
            }
            return a;
        }
        else
        {
            return InputRangeMax;
        }
    } 
    public void SetInputRange()
    {
        float[] InputMin = new float[InputIDMapping.Length]; 
        float[] InputMax = new float[InputIDMapping.Length];

        for (int i = 0; i < InputIDMapping.Length; i++)
        {
            switch (InputIDMapping[i])
            {
                default:
                    InputMin[i] = -1f;
                    InputMax[i] = 1f;
                    break;
                case 12://Delta 2
                    InputMin[i] = -1f;
                    InputMax[i] = 1f;
                    break;
                case 13://DELTA 1.5
                    InputMin[i] = -1f;
                    InputMax[i] = 0.5f;
                    break;
                case 14://DELTA 1.5
                    InputMin[i] = -0.5f;
                    InputMax[i] = 1f;
                    break;
                case 15://DELTA 1
                    InputMin[i] = -1f;
                    InputMax[i] = 0;
                    break;
                case 16://DELTA 1
                    InputMin[i] = -0.5f;
                    InputMax[i] = 0.5f;
                    break;
                case 17:// DELTA 1
                    InputMin[i] = 0f;
                    InputMax[i] = 1f;
                    break;
                case 18: // DELTA 0.5
                    InputMin[i] = -1f;
                    InputMax[i] = -0.5f;
                    break;
                case 19://Delta 05
                    InputMin[i] = -0.5f;
                    InputMax[i] = 0f;
                    break;
                case 20://Delta 05
                    InputMin[i] = 0;
                    InputMax[i] = 0.5f;
                    break;
                case 21:// Delta 05
                    InputMin[i] = 0.5f;
                    InputMax[i] = 1f;
                    break;


            }
        }
        InputRangeMin = InputMin;
        InputRangeMax = InputMax;
    }

    public static float[,] IDToInputOutputRange(int[] MapArray)
    {
        float[,] ReturnArray = new float[MapArray.Length,2];
        for (int i = 0; i < MapArray.Length; i++)
        {
            switch (MapArray[i])
            {
                default:
                    ReturnArray[i,0] = -1f;
                    ReturnArray[i, 1] = 1f;
                    break;
                case 12://Delta 2
                    ReturnArray[i, 0] = -1f;
                    ReturnArray[i, 1] = 1f;
                    break;
                case 13://DELTA 1.5
                    ReturnArray[i, 0] = -1f;
                    ReturnArray[i, 1] = 0.5f;
                    break;
                case 14://DELTA 1.5
                    ReturnArray[i, 0] = -0.5f;
                    ReturnArray[i, 1] = 1f;
                    break;
                case 15://DELTA 1
                    ReturnArray[i, 0] = -1f;
                    ReturnArray[i, 1] = 0;
                    break;
                case 16://DELTA 1
                    ReturnArray[i, 0] = -0.5f;
                    ReturnArray[i, 1] = 0.5f;
                    break;
                case 17:// DELTA 1
                    ReturnArray[i, 0] = 0f;
                    ReturnArray[i, 1] = 1f;
                    break;
                case 18: // DELTA 0.5
                    ReturnArray[i, 0] = -1f;
                    ReturnArray[i, 1] = -0.5f;
                    break;
                case 19://Delta 05
                    ReturnArray[i, 0] = -0.5f;
                    ReturnArray[i, 1] = 0f;
                    break;
                case 20://Delta 05
                    ReturnArray[i, 0] = 0;
                    ReturnArray[i, 1] = 0.5f;
                    break;
                case 21:// Delta 05
                    ReturnArray[i, 0] = 0.5f;
                    ReturnArray[i, 1] = 1f;
                    break;
            }
        }
        return ReturnArray;
    }

    //--------------------------------Model Meta Data------------------------------//

    int NumberOfInputs;
    int FeatureLayerSize;
    int DecInc;

    public void SetAutoEncoderData(int decInc,int FtlSize, int NumInOut)
    {
        NumberOfInputs = NumInOut;
        FeatureLayerSize = FtlSize;
        DecInc = decInc;
    }

    public enum NetworkType
    {
        Supervised,
        Unsupervised,
    }

    public NetworkType MyNetworkType;

    public string GetMetaData()
    {
        return "inputs " + InputIDMapping.Length + " outputs " + OutputIDMapping.Length + " layers " + NumberOfLayers  ; 
    }

    string ModelName;
    public string GetModelName()
    {
       // Debug.Log("name " + ModelName);
        return ModelName;
    }

    int NumberOfLayers; 
    public int GetNumberOfLayers()
    { return NumberOfLayers; } 

    TrainingSet[] TrainingSets; // <<<<<<<<<<<<<<<<<<<< TRAINING SETS
    public int GetNumberOfTrainingSets()
    {
        return TrainingSets.Length;
    }

    int MyID;
    public int GetID() { return MyID; }
    public void SetID(int Value) { MyID = Value; }

    private float[] InputMinValuesArray;
    private float[] InputMaxValuesArray;


    public float[] GetInputMaxArray()
    { return InputMaxValuesArray; }

    public float[] GetInputMinArray()
    { return InputMinValuesArray; }
  
    private void SetVarianceMaxArrays()
    {
        int IL = TrainingSets[0].GetInputs().Length;

        InputMaxValuesArray = new float[IL];
        InputMinValuesArray = new float[IL];
        for (int i = 0; i < InputMinValuesArray.Length; i++)
        {
            InputMinValuesArray[i] = 10000;
        }


        Debug.Log("-----------------SetMax-----------------  ");
        for (int i = 0; i < TrainingSets.Length; i++)
        {
            for (int j = 0; j < IL; j++)
            {
                Debug.Log("Set  " + i+ " = " + TrainingSets[i].GetInputs()[j]);
                if ( TrainingSets[i].GetInputs()[j] > InputMaxValuesArray[j])
                {
                    InputMaxValuesArray[j] = TrainingSets[i].GetInputs()[j];
                  //  Debug.LogError("TrainingSets " + i + " @ input " +j + " = "+ InputMaxValuesArray[j]);
                }
            }
            Debug.Log("----------------------------------  ");
        }
        Debug.Log("----------------SetMin------------------  ");
        for (int i = 0; i < TrainingSets.Length; i++)
        {
            for (int j = 0; j < IL; j++)
            {
                Debug.Log("Set  " + i + " = " + TrainingSets[i].GetInputs()[j]);
                if (TrainingSets[i].GetInputs()[j] < InputMinValuesArray[j])
                {
                    InputMinValuesArray[j] = TrainingSets[i].GetInputs()[j];
                    //  Debug.LogError("TrainingSets " + i + " @ input " +j + " = "+ InputMaxValuesArray[j]);
                }
            }
            Debug.Log("----------------------------------  ");
        }

      
    }

    //-------------------------------Networks---------------------------//

    [SerializeField]
    NeuralNetMainModule[] Networks; 

    public void AddNetwork(DataManager.LayerData[] Data, NeuralNetMainModule.ActivationFunction ActFunc,bool isAutoEnc,int FtL,int IncDec)
    {
        int n = GetNumberOfNetworks();
       if(n < Networks.Length && !Networks[n].GetIsLoaded())
        {
            Networks[n].SetUpFromExcelData(Data, ActFunc,ModelName,n,  isAutoEnc,  FtL,  IncDec);
        }
     
    }

    public void CreateNewNetwork()
    {
        Debug.Log("CreateNewNetwork() of Type" + MyNetworkType);
        for (int i = 0; i < Networks.Length; i++)
        {
            if (!Networks[i].GetIsLoaded())
            {
                Debug.Log("!Networks[i].GetIsLoaded() ");
                int NumberOfNeuronsPerLayer = 0;
                if (MyNetworkType == NetworkType.Supervised)
                {
                    NumberOfNeuronsPerLayer = InputIDMapping.Length;
                }
                else if(TrainingSets.Length > 0) 
                {
                    NumberOfNeuronsPerLayer = TrainingSets[0].GetInputs().Length;
                }
                else
                {
                    NumberOfNeuronsPerLayer = 0;
                    Debug.LogError("Number Of Neurons Can Not Equal 0");
                }

                if(MyNetworkType == NetworkType.Supervised && OutputIDMapping.Length > InputIDMapping.Length)
                {
                    NumberOfNeuronsPerLayer = OutputIDMapping.Length;
                }
                if (MyNetworkType == NetworkType.Supervised)
                {
                    Networks[i].SetUp(NumberOfLayers, NumberOfNeuronsPerLayer, NumberOfNeuronsPerLayer, InputIDMapping.Length, OutputIDMapping.Length);
                }
                else
                {
                    Networks[i].SetUpAsAutoEncoder(NumberOfLayers , NumberOfInputs, DecInc, FeatureLayerSize, ModelName, i,NeuralNetMainModule.ActivationFunction.HyperbolicTangent);
                }
                Networks[i].SetNameID("Net " + i.ToString());
                break;
            }
        }
      
    }

    public int GetNumberOfNetworks()
    {
       // Debug.Log("Networks.Length; " + Networks.Length);
        int count = 0;
        for (int i = 0; i < 10; i++)
        {
            if (Networks[i].GetIsLoaded())
            {
                count++;
            }
        }
       // Debug.Log("Active Net Count " + count);
        return count;
    }

    public NeuralNetMainModule GetNetworkByID(int ID)
    {
        return Networks[ID];
    }

    //-----------------------------------SetUp Check--------------------------------//

    bool IsSetUp = false;

    public bool GetIsSetUp()
    {
        return IsSetUp;
    }

    //-----------------------------Constructors And SetUp Functions-----------------------------//

    public void SetUpTrainingModel(int Id,int numberOfLayers, int[] outputIDMapping, int[] inputIDMapping, string name, TrainingSet[] trainingSets, NetworkType myType)
    {
        MyNetworkType = myType;
        NumberOfLayers = numberOfLayers;
        ModelName = name;
        OutputIDMapping = outputIDMapping;
        InputIDMapping = inputIDMapping;
        TrainingSets = trainingSets;
        IsSetUp = true;
         
        Debug.Log("Loading Model");
        Debug.Log("Number Of Training Sets " + TrainingSets.Length);
        MyID = Id;
        Networks = new NeuralNetMainModule[10];

        Transform[] ChildObjTransforms = gameObject.GetComponentsInChildren<Transform>();
       // Debug.Log("ChildObjTransforms" + ChildObjTransforms.Length);
        for (int i = 1; i < ChildObjTransforms.Length; i++)
        {
            Networks[i-1] = ChildObjTransforms[i].GetComponent<NeuralNetMainModule>();
        }
        if(myType == NetworkType.Supervised)
        {
            SetInputRange();
            int[] DefaultInputMap = new int[12];
            int[] DefaultOutputMap = new int[12];
            for (int i = 0; i < 12; i++)
            {
                DefaultInputMap[i] = i;
                DefaultOutputMap[i] = i;
            }
            SetCustomInputMapping(DefaultInputMap);
            SetCustomOutputMapping(DefaultOutputMap);
        }
        else
        {
            NumberOfInputs = trainingSets[0].GetInputs().Length;
        }

        if(myType == NetworkType.Unsupervised)
        {
            Debug.LogError("SetVarianceMaxArrays()");
            SetVarianceMaxArrays();
        }
      
    }

    public TrainingSet GetTrainingSetByID(int ID)
    {
        return TrainingSets[ID];
    }

    //-----------------------------Debug Functions-----------------------------//

    public void PrintRandomTrainingSet()
    {
        int Set = Random.Range(0, TrainingSets.Length);
        for (int i = 0; i < TrainingSets[i].GetInputs().Length; i++)
        {
            Debug.Log("Input " + i + " = " + TrainingSets[i].GetInputs()[i]);
        }
        for (int i = 0; i < TrainingSets[i].GetOutputs().Length; i++)
        {
            Debug.Log("Output " + i + " = " + TrainingSets[i].GetOutputs()[i]);
        }
    }

    public void PrintTrainingData()
    {
        for (int i = 0; i < TrainingSets.Length; i++)
        {
            Debug.Log("Input 0 @Set " + i + " = " + TrainingSets[i].GetInputs()[0]);
        }
    }

    //-----------------------------Analysis System-----------------------------//



    

    public void SaveCodeLayerData(int NetID)
    {
        if(File.Exists(DataManager.DM.GetCodeLayerSavesFileLocation() + "/" + ModelName+"Net"+ NetID.ToString()))
        {
            //Overwrite
        }
        else 
        {
            //CreateNewFile
        
        }
    }

}
