using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    [SerializeField]
    string PersistentDataFilePath;

    private void OnApplicationQuit()
    {
        SavePersistentData();
    }

    private void SavePersistentData()
    {
        //---Create String With Data---//
        string Data = DirectoryPath;

        Debug.Log(Data);

        //-----------------------------//

        byte[] EncodedData = Encoding.ASCII.GetBytes(Data);

        string A = Encoding.ASCII.GetString(EncodedData);

        StreamWriter SW = new StreamWriter(PersistentDataFilePath);   

        string TxtData = string.Empty;

        for (int i = 0; i < EncodedData.Length; i++)
        {
            TxtData += EncodedData[i] + ".";
        }

        SW.WriteLine(TxtData);
   
        SW.Close();
    }
    private void LoadPersistentData()
    {
        StreamReader SR = new StreamReader(PersistentDataFilePath);

        string Data = SR.ReadToEnd();

        string[] SplitData = Data.Split('.');

        byte[] textbytes = new byte[SplitData.Length - 1];

        for (int i = 0; i < SplitData.Length - 1; i++)
        {
            textbytes[i] = Convert.ToByte(SplitData[i]);
        }

        Data = Encoding.ASCII.GetString(textbytes);

        DirectoryPath = Data;

        SR.Close();
    }


    public void ResetDirectoryToDefault()
    {
        DirectoryPath = DefaultDirectoryPath;
        IField.text = null; InputFieldText.text = "";
    }

    public static DataManager DM;

    public int GetNumberOfTrainingModels()
    {
        int value = 0;

        if (Model_1.GetIsSetUp())
        {
            value++;
        }
        if (Model_2.GetIsSetUp())
        {
            value++;
        }
        if (Model_3.GetIsSetUp())
        {
            value++;
        }
        if (Model_4.GetIsSetUp())
        {
            value++;

        }
        if (Model_5.GetIsSetUp())
        {
            value++;
        }
        if (Model_6.GetIsSetUp())
        {
            value++;
        }

        return value;
    }

    public TrainingModel[] GetTrainingModelsArray()
    {

        TrainingModel[] Models = new TrainingModel[GetNumberOfTrainingModels()];
        int count = 0;
        for (int i = 0; i < Models.Length; i++)
        {
            reset:;
            switch (count)
            {
                case 0:
                    if (Model_1.GetIsSetUp())
                    {
                        Models[i] = Model_1; count++;
                    }
                    else
                    {
                        count++; goto reset;
                    }
                    break;
                case 1:
                    if (Model_2.GetIsSetUp())
                    {
                        Models[i] = Model_2; count++;
                    }
                    else
                    {
                        count++; goto reset;
                    }
                    break;
                case 2:
                    if (Model_3.GetIsSetUp())
                    {
                        Models[i] = Model_3; count++;
                    }
                    else
                    {
                        count++; goto reset;
                    }
                    break;
                case 3:
                    if (Model_4.GetIsSetUp())
                    {
                        Models[i] = Model_4; count++;
                    }
                    else
                    {
                        count++; goto reset;
                    }
                    break;
                case 4:
                    if (Model_5.GetIsSetUp())
                    {
                        Models[i] = Model_5; count++;
                    }
                    else
                    {
                        count++; goto reset;
                    }
                    break;
                case 5:
                    if (Model_6.GetIsSetUp())
                    {
                        Models[i] = Model_6; count++;
                    }
                    else
                    {
                        count++; goto reset;
                    }
                    break;
            }
        }



        return Models;
    }

    public TrainingModel GetModelByID(int ID)
    {
        switch (ID)
        {
            default: return null;

            case 0: return Model_1;

            case 1: return Model_2;

            case 2: return Model_3;

            case 3: return Model_4;

            case 4: return Model_5;

            case 5: return Model_6;


        }
    }

    [SerializeField]
    string DefaultDirectoryPath;

    string DirectoryPath;

    [SerializeField]
    Text InputFieldDefaultText;

    [SerializeField]
    Text InputFieldText;

    [SerializeField]
    InputField IField;

    public void UpdateInputField()
    {
        Debug.Log("Value Change");

        if (System.IO.Directory.Exists(IField.text))
        {
            Debug.Log("Exists");
            DirectoryPath = InputFieldText.text;
        }
        else
        {
            IField.text = "";
            Debug.Log("Set To Null");
            InputFieldText.text = "";
        }
    }

    [SerializeField]
    TrainingModel Model_1; public TrainingModel GetModel1() { return Model_1; }
    [SerializeField]
    TrainingModel Model_2; public TrainingModel GetModel2() { return Model_2; }
    [SerializeField]
    TrainingModel Model_3; public TrainingModel GetModel3() { return Model_3; }
    [SerializeField]
    TrainingModel Model_4; public TrainingModel GetModel4() { return Model_4; }
    [SerializeField]
    TrainingModel Model_5; public TrainingModel GetModel5() { return Model_5; }
    [SerializeField]
    TrainingModel Model_6; public TrainingModel GetModel6() { return Model_6; }

    public NeuralNetMainModule GetNetworkRef(int ModelID, int NetID)
    {
        switch (ModelID)
        {
            default:
                Debug.LogError("No Model ID with values greater than 6 or smaller than 0 ");
                return null;
            case 0:
                return Model_1.GetNetworkByID(NetID);
            case 1:
                return Model_2.GetNetworkByID(NetID);
            case 2:
                return Model_3.GetNetworkByID(NetID);
            case 3:
                return Model_4.GetNetworkByID(NetID);
            case 4:
                return Model_5.GetNetworkByID(NetID);
            case 5:
                return Model_6.GetNetworkByID(NetID);
        }
    }


    string[] Data;
    private void Awake()
    {
       


        LoadPersistentData();

        if(!System.IO.Directory.Exists(DirectoryPath))
        {
            DirectoryPath = DefaultDirectoryPath;

        }
        else
        {
            IField.text = DirectoryPath; InputFieldText.text = DirectoryPath;
        }
        


        InputFieldDefaultText.text = System.IO.Directory.GetCurrentDirectory() + "/" + DefaultDirectoryPath;
       
        SpawnedNPCs = new List<GameObject>();
        DM = GetComponent<DataManager>();



    }
    private void Start()
    {
        UpdateAllModels();
        ReadAllNetworks();

        UIManager.UIM.OpenCloseModelView(true);
        UIManager.UIM.OpenCloseNetViewMode(false);


        //float[] Coef = { 3f, 4f, -5f, 6f, -2f};
        //MathFunctionClass.PolinomialRoots(Coef, 5f, out float[] Roots);
        //for (int i = 0; i < Roots.Length; i++)
        //{
        //    Debug.Log("Root " + i + " = " + Roots[i]);
        //}

        ////float[,] Matrix =
        ////{
        ////    { -3,4,5,6,1,4 },
        ////    { 6,9,1,-1,1,8 },
        ////    { 1,3,6,3,8,-2 },
        ////    { -9,1,1,1,1,2 },
        ////    { 9,1,-1,-7,4,3},
        ////    { 3,5,-7,-3,2,8},
        ////};

        //float[,] Matrix =
        //{
        //  { 2,2,6 ,2},
        //  { 2,3,1 ,1},
        //  { 6,1,4 ,4},
        //  { 2,1,4 ,5},
        //};

        ////Debug.Log("Determinant = " + MathFunctionClass.CalculateMatrixDeterminant(Matrix));

        //MathFunctionClass.CalculateAproxEigenVectors(Matrix,50);


    }
 

    [SerializeField]
    string TemporaryyFileCopy;

    public void UpdateAllModels()
    {
       // DirectoryPath = System.IO.Directory.GetCurrentDirectory() + "/" + DirectoryPath;

        Data = System.IO.Directory.GetFiles(DirectoryPath, "*.csv");
        Debug.Log("Number Of CVS Files" + Data.Length);

        for (int i = 0; i < Data.Length ; i++)
        {
            //AutoEncoderData
            int FtlSize = 0;
            int IncDec = 0;

            //
            string ModelName = null;
            TrainingSet[] Sets = null;
            int NumberOfInputs = 0; int NumberOfOutputs = 0;
            int Size = 0;
            float[][] InputSets = null;
            float[][] OutputSets = null;
            int[] InputIDs = null;
            int[] OutputIDs = null;
            int NumberOfSets = 0;
            StreamReader SR = new StreamReader(Data[i]);
            bool Jump = false;
            Debug.Log("Loading " + Data[i]);
            string Line1 = SR.ReadLine();
            if (Line1 == null)
            {
                Debug.LogError("File Has No Data ");
                SR.Close();
                goto EXIT;
            }

            string[] DataLine = Line1.Split(';');

            int SetCount = 0;
            if (DataLine[0] == "MF2")
            { 
                SR.Close(); 
                if(TryProcessType2ModelTemplate(Data[i], out  SetCount, out Sets))
                {
                    ModelName = DataLine[2]; Debug.Log("ModelName " + ModelName);
                    NumberOfInputs = int.Parse(DataLine[4]); Debug.Log("NumberOfInputs " + NumberOfInputs);
                    NumberOfOutputs = int.Parse(DataLine[6]); Debug.Log("NumberOfOutputs " + NumberOfOutputs);
                    Size = int.Parse(DataLine[8]); Debug.Log("Size " + Size);
                    FtlSize = int.Parse(DataLine[10]); Debug.Log("FtleSize " + FtlSize);
                    IncDec = int.Parse(DataLine[12]); Debug.Log("IncDec " + IncDec);

                    Jump = true;
                    NumberOfSets = SetCount;
                    
                    goto EXIT;
                }  
            }

            ModelName = DataLine[1]; Debug.Log("ModelName " + ModelName);
            NumberOfInputs = int.Parse(DataLine[3]); Debug.Log("NumberOfInputs " + NumberOfInputs);
            NumberOfOutputs = int.Parse(DataLine[5]); Debug.Log("NumberOfOutputs " + NumberOfOutputs);
            Size = int.Parse(DataLine[7]); Debug.Log("Size " + Size);
            NumberOfSets = int.Parse(DataLine[9]); Debug.Log("NumberOfSets " + NumberOfSets);
            InputSets = new float[NumberOfSets][];
            for (int k = 0; k < NumberOfSets; k++)
            {
                InputSets[k] = new float[NumberOfInputs];
            }
            OutputSets = new float[NumberOfSets][];
            for (int k = 0; k < NumberOfSets; k++)
            {
                OutputSets[k] = new float[NumberOfOutputs];
            }

            SR.ReadLine();//empty line
            InputIDs = new int[NumberOfInputs];
            DataLine = SR.ReadLine().Split(';');

            for (int j = 0; j < NumberOfInputs; j++)
            {
                InputIDs[j] = int.Parse(DataLine[j]);
            }

            SR.ReadLine();//empty line
            OutputIDs = new int[NumberOfOutputs];
            DataLine = SR.ReadLine().Split(';');

            for (int j = 0; j < NumberOfOutputs; j++)
            {
                OutputIDs[j] = int.Parse(DataLine[j]);
            }

            for (int j = 0; j < NumberOfSets; j++)
            {
                SR.ReadLine();//empty line
                SR.ReadLine();//empty line

                DataLine = SR.ReadLine().Split(';');
                for (int k = 0; k < NumberOfInputs; k++)
                {
                    InputSets[j][k] = float.Parse(DataLine[k + 2]);

                }
                SR.ReadLine();//empty line

                DataLine = SR.ReadLine().Split(';');
                for (int k = 0; k < NumberOfOutputs; k++)
                {
                   // Debug.Log("[" + k + "]");
                   // Debug.Log("Data " + DataLine[k + 2]);

                    OutputSets[j][k] = float.Parse(DataLine[k + 2]);
                }
            }

            SR.Close();
            Sets = new TrainingSet[NumberOfSets];

            for (int k = 0; k < NumberOfSets; k++)
            {
                Sets[k] = new TrainingSet(InputSets[k], OutputSets[k]);
            }

            EXIT:;
            TrainingModel.NetworkType NetType = TrainingModel.NetworkType.Supervised;
            if(Jump)
            {
                Debug.LogError("TYPE 2 MODEL " );
                NetType = TrainingModel.NetworkType.Unsupervised;
                
            }
            switch (i)
            {
                case 0:
                    if (Jump)
                        Model_1.SetAutoEncoderData(IncDec, FtlSize, NumberOfInputs);
                    Model_1.SetUpTrainingModel(0, Size, OutputIDs, InputIDs, ModelName, Sets, NetType);
                    Model_1.SetID(0);
                   
                    Debug.Log("Setting Model 1");
                    //Debug.Log("Size " + Size);
                    //Debug.Log("ModelName " + ModelName);
                    break;
                case 1:
                    if (Jump)
                        Model_2.SetAutoEncoderData(IncDec, FtlSize, NumberOfInputs);
                    Model_2.SetUpTrainingModel(1, Size, OutputIDs, InputIDs, ModelName, Sets, NetType);
                    Model_2.SetID(1);
                    
                    Debug.Log("Setting Model 2");
                    //Debug.Log("Size " + Size);
                    //Debug.Log("ModelName " + ModelName);
                    break;
                case 2:
                    if (Jump)
                        Model_3.SetAutoEncoderData(IncDec, FtlSize, NumberOfInputs);
                    Model_3.SetUpTrainingModel(2, Size, OutputIDs, InputIDs, ModelName, Sets, NetType);
                    Model_3.SetID(2);
                   
                    Debug.Log("Setting Model 3");
                    break;
                case 3:
                    if (Jump)
                        Model_4.SetAutoEncoderData(IncDec, FtlSize, NumberOfInputs);
                    Model_4.SetUpTrainingModel(3, Size, OutputIDs, InputIDs, ModelName, Sets, NetType);
                    Model_4.SetID(3);
                    
                    Debug.Log("Setting Model 4");
                    break;
                case 4:
                    if (Jump)
                        Model_5.SetAutoEncoderData(IncDec, FtlSize, NumberOfInputs);
                    Model_5.SetUpTrainingModel(4, Size, OutputIDs, InputIDs, ModelName, Sets, NetType);
                    Model_5.SetID(4);
                  
                    Debug.Log("Setting Model 5");
                    break;
                case 5:
                    if (Jump)
                        Model_6.SetAutoEncoderData(IncDec, FtlSize, NumberOfInputs);
                    Model_6.SetUpTrainingModel(5, Size, OutputIDs, InputIDs, ModelName, Sets, NetType);
                    Model_6.SetID(5);
                 
                    Debug.Log("Setting Model 6");
                    break;

            }
            
        }

        for (int l = 0; l < 6; l++)
        {
            switch (l)
            {
                case 0:
                    if (!Model_1.GetIsSetUp())
                    {
                        Debug.LogError("DataNotSet @ Model " + (l + 1));
                    }
                    else
                    {
                        Debug.Log("DataSet @ Model " + (l + 1));
                    }
                    break;
                case 1:
                    if (!Model_2.GetIsSetUp())
                    {
                        Debug.LogError("DataNotSet @ Model " + (l + 1));
                    }
                    else
                    {
                        Debug.Log("DataSet @ Model " + (l + 1));
                    }
                    break;
                case 2:
                    if (!Model_3.GetIsSetUp())
                    {
                        Debug.LogError("DataNotSet @ Model " + (l + 1));
                    }
                    else
                    {
                        Debug.Log("DataSet @ Model " + (l + 1));
                    }
                    break;
                case 3:
                    if (!Model_4.GetIsSetUp())
                    {
                        Debug.LogError("DataNotSet @ Model " + (l + 1));
                    }
                    else
                    {
                        Debug.Log("DataSet @ Model " + (l + 1));
                    }
                    break;
                case 4:
                    if (!Model_5.GetIsSetUp())
                    {
                        Debug.LogError("DataNotSet @ Model " + (l + 1));
                    }
                    else
                    {
                        Debug.Log("DataSet @ Model " + (l + 1));
                    }
                    break;
                case 5:
                    if (!Model_6.GetIsSetUp())
                    {
                        Debug.LogError("DataNotSet @ Model " + (l + 1));
                    }
                    else
                    {
                        Debug.Log("DataSet @ Model " + (l + 1));
                    }
                    break;

            }


        }
    }

    private bool TryProcessType2ModelTemplate(string path,out int Sets,out TrainingSet[] TrainingSets)
    {
        File.Copy(path, TemporaryyFileCopy, true);

        bool SkipInsertFase  =false;

        List<float[]> SetInputOutputData = new List<float[]>();
        int DataLine = 0; List<string> NameIDList = new List<string>();
        Sets = 0;

        int Layers = 0;

        Debug.Log("------------------------------------TryProcessType2Model--------------------------------------");
        bool value = false;

        File.Copy(path, TemporaryyFileCopy,true);

        StreamWriter SW = new StreamWriter(path);
    
        StreamReader SR = new StreamReader(TemporaryyFileCopy);

        int NumberOfInputs = 0;
        string[] LineSplit;

        string Line = SR.ReadLine();
        LineSplit = Line.Split(';');
        int.TryParse(LineSplit[4], out NumberOfInputs);
        int.TryParse(LineSplit[6], out Layers);
        if(NumberOfInputs <= 0 && Layers <= 0)
        {
            goto Exit1;
        }

        SW.WriteLine(Line);

        Line = SR.ReadLine(); // line 2 contains data on wether the model is processed or not
        SW.WriteLine(Line);
        LineSplit = Line.Split(';');

        SW.WriteLine(SR.ReadLine());//line 2
        SW.WriteLine(SR.ReadLine());//line 3
        if (int.TryParse(LineSplit[0],out int result) )
        {
            if(result == 0)
            {
                Debug.Log("Model Data is not processed Start Processing \r\n");
                // Model Data is not processed Start Processing 
            }
            else
            {
                Debug.Log("Model Data is already processed, Exit");
                // Model Data is already processed, Exit 
                value = true;
                SkipInsertFase = true;
            
            }
        }
        else
        {
            Debug.Log("Cant parse data");
            // Cant parse data
            goto Exit1;
        }
      
        
        RESET:;
        //Data processing Fase Start
        //Debug.Log("Reading Data Line " + DataLine);

        Line = SR.ReadLine();
      
        if (Line != null)
        {
            DataLine++;
          LineSplit = Line.Split(';');
        
            if (LineSplit[2] != "")
            {
                if(NameIDList.Contains(LineSplit[2]))
                {
                   // Debug.Log("NameAlreadyExists " );
                    //write corresponding ID
                }
                else
                {
                    //Debug.Log("NameIDList.Add ");
                    NameIDList.Add(LineSplit[2]);
                    //write corresponding ID
                }
                //if (LineSplit[0] == "")
                //{
                 SW.WriteLine(NameIDList.IndexOf(LineSplit[2]).ToString() + Line);
                //}
                //else
                //{
                //    SW.WriteLine(Line);
                //}
                float flt = 0;
                float[] fltArray = new float[NumberOfInputs];
                for (int i = 0; i < NumberOfInputs; i++)
                {
                    if(float.TryParse(LineSplit[5 + i], out flt))
                    {
                        fltArray[i] = flt;
                    }
                    else
                    {
                       break;
                    }
                }
               // Debug.Log("Adding float array <<<<");
                SetInputOutputData.Add(fltArray);
                goto RESET;
            }
            // if LineSplit[2] == "" then the cycle ends
        }

        //for (int i = 0; i < NameIDList.Count; i++)
        //{
        //    Debug.Log(NameIDList[i] + "ID  = " + i);
        //}

        //Data processing Fase End
        Exit1:;
        if (DataLine > 0 && NameIDList.Count > 0)
        {
            value = true;
        }

        SR.Close(); SW.Close();

        if (value )
        {
            if(!SkipInsertFase)
            {

            SR = new StreamReader(TemporaryyFileCopy);

            SW = new StreamWriter(path);
           
            SW.WriteLine(SR.ReadLine());

            string line2 = SR.ReadLine().Remove(0,1);
            SW.WriteLine(line2.Insert(0,"1"));

            SW.WriteLine(SR.ReadLine());
            SW.WriteLine(SR.ReadLine());

            for (int i = 0; i < DataLine; i++)
            {
                SW.WriteLine(SR.ReadLine());
            }

            SR.Close(); SW.Dispose();
            }
            float Min0 = 100  ; 
            TrainingSets = new TrainingSet[SetInputOutputData.Count];
            Debug.Log(SetInputOutputData.Count);
            for (int i = 0; i < SetInputOutputData.Count; i++)
            {
                if (SetInputOutputData[i][0] < Min0)
                {
                    Min0 = SetInputOutputData[i][0];
                }
            }
            Debug.LogError("Min0 = " + Min0);
            for (int i = 0; i < SetInputOutputData.Count; i++)
            {
                if(SetInputOutputData[i][0] < Min0)
                {
                    Debug.LogError("Error");
                }
                TrainingSets[i] = new TrainingSet(SetInputOutputData[i], SetInputOutputData[i]);
            }
        }
        else
        {
            Debug.Log("TrainingSets = null");
            TrainingSets = null;
        }
       
        return value;
    }

    [SerializeField]
    string NetworksDirectoryPath;

    [SerializeField]
    string ExcelNetworkTemplate;

    public bool  TrySaveNet(int NetID,int ModelID)
    {
        bool value = false;
        if(  GetNetworkRef(ModelID, NetID).GetWasAlteredAfterSaving())
        {
            LayerData[] NetData = GetNetworkRef(ModelID, NetID).GetLayerData();
            NeuralNetMainModule.ActivationFunction Act = GetNetworkRef(ModelID, NetID).GetActivationFunction();
            string ModelName = GetModelByID(ModelID).GetModelName();
            int FtL = 0;
            int IncDec = 0;
            bool IsSupervised = GetNetworkRef(ModelID, NetID).GetMyTrainingType() == TrainingModel.NetworkType.Unsupervised? true:false;
            if(IsSupervised)
            {
                GetNetworkRef(ModelID, NetID).GetAutoencoderData(out int a, out FtL);
                IncDec = GetNetworkRef(ModelID, NetID).GetIncDec();
            }
            //Debug.Log("ModelName " + ModelName);
            CreateNewNetwork_Excel(ModelID, NetID, ModelName, NetData, Act,IsSupervised,FtL, IncDec);

            GetNetworkRef(ModelID, NetID).SetWasAlteredAfterSaving(false);
            value = true;
        }

        return value;
    }

    private void CreateNewNetwork_Excel(int ModelID,int NetID,string ModelName, LayerData[] NetData,NeuralNetMainModule.ActivationFunction Act,bool IsAutoencoder,int Ftl,int IncDec)
    {
      
        string FileName = "M" + ModelID.ToString() + "Net" + NetID.ToString() + "_" + ModelName;

        if(System.IO.File.Exists(NetworksDirectoryPath + "/" + FileName + ".csv"))
        {
            Debug.Log("Overwriting Network");
        }
        else
        {
            System.IO.File.Copy(ExcelNetworkTemplate, NetworksDirectoryPath + "/" + FileName + ".csv");
            Debug.Log("Saving New Network");
        }

            string Line = null;
            string[] LineSplit = null;

            StreamReader SR = new StreamReader(ExcelNetworkTemplate);

            StreamWriter SW = new StreamWriter(NetworksDirectoryPath + "/" + FileName + ".csv");

            Line = SR.ReadLine();
            LineSplit = Line.Split(';');
            Line = null;
            LineSplit[1] = NetID.ToString();
            LineSplit[3] = NetData[0].Biases.Length.ToString();
            LineSplit[5] = NetData[NetData.Length - 1].Biases.Length.ToString();
            LineSplit[7] = NetData.Length.ToString();
            LineSplit[9] = Act.ToString();
            LineSplit[11] = ModelName;
            if(IsAutoencoder)
            {
              LineSplit[12] = Ftl.ToString();
              LineSplit[13] = Ftl.ToString();
            }

            for (int i = 0; i < LineSplit.Length; i++)
            {
                Line += LineSplit[i] + ";";
            }
            //  Debug.Log("MetaData: " + Line);
            SW.WriteLine(Line);

            Line = SR.ReadLine();
            SW.WriteLine(Line);

            string L1 = "";
            string L2 = "";

            for (int i = 0; i < NetData.Length; i++)
            {
                L1 = "";
                L2 = "";
                Debug.Log("Layer " + i);
                for (int j = 0; j < NetData[i].Biases.Length; j++)
                {
                    L1 += NetData[i].Biases[j] + ";";
                    Debug.Log("Bias " + j + " = " + NetData[i].Biases[j]);

                }
                SW.WriteLine(L1);
               if(i != NetData.Length-1)
               {
                  for (int k = 0; k < NetData[i].Weights.GetLength(0); k++)
                  {
                    for (int j = 0; j < NetData[i].Weights.GetLength(1); j++)
                    {
                        L2 += NetData[i].Weights[k, j] + "/";
                    }
                    Debug.Log("Weights " + k + " = " + L2);
                    L2 += ";";

                  }
                   SW.WriteLine(L2);
               }
           
            }

            SW.Close();
            SR.Close();

    }

    public struct LayerData
    {
        public LayerData(float[] biases, float[,] weights)
        {
            Biases = biases;
            Weights = weights;

        }
       public float[] Biases;
       public float[,] Weights;
    }

    public void ReadAllNetworks()
    {
       
        Data = System.IO.Directory.GetFiles(NetworksDirectoryPath, "*.csv");

        Debug.Log("-----------Number Of Networks " + Data.Length+"---------------");

        for (int i = 0; i < Data.Length; i++)
        {
            //Debug.Log("Reading Net " + i);
            int netID;
            int Inputs;
            int Outputs;
            int Layers;
            string ActivationFunction;
            string ModelName;

            //auto encoder params
            bool IsAutoencoder = false;
            int FtL = 0;
            int IncDec = 0;
            //end

            StreamReader SR = new StreamReader(Data[i]);

            StreamReader SR2 = new StreamReader(Data[i]);
            SR2.ReadLine();
            SR2.ReadLine();
            SR2.ReadLine();

            NeuralNetMainModule.ActivationFunction ActFunc = NeuralNetMainModule.ActivationFunction.BinaryStep;
          
            string[] DataLine = SR.ReadLine().Split(';'); 
            netID = int.Parse(DataLine[1]);// Debug.Log("Net ID " + netID);
            Inputs = int.Parse(DataLine[3]);// Debug.Log("Inputs " + Inputs);
            Outputs = int.Parse(DataLine[5]);// Debug.Log("Outputs " + Outputs);
            Layers = int.Parse(DataLine[7]); //Debug.Log("Layers " + Layers);
            ActivationFunction = DataLine[9]; //Debug.Log("Activation Function " + ActivationFunction);
            ActivationFunction.Replace(' ', '\0'); 
            ModelName = DataLine[11];// Debug.Log("Model Name " + ModelName);

            if (int.TryParse(DataLine[12],out int Value))
            {
                FtL = Value;
                IncDec = int.Parse(DataLine[13]); 
                IsAutoencoder = true;
            }


            if (ActivationFunction == "Sigmoid")
            {
                //Debug.Log(" Activation Function Sigmoid");
                ActFunc = NeuralNetMainModule.ActivationFunction.Sigmoid;
            }
            else if(ActivationFunction == "HyperbolicTangent")
            {
                //Debug.Log(" Activation Function HyperbolicTangent");
                ActFunc = NeuralNetMainModule.ActivationFunction.HyperbolicTangent;
            }
            else
            {
                //Debug.LogError("Error Activation Function Not Recognised");
                ActFunc = NeuralNetMainModule.ActivationFunction.BinaryStep;
            }

            TrainingModel[] TMRef = GetTrainingModelsArray();

            bool ModelFound = false;
            for (int j = 0; j < TMRef.Length; j++)
            {
                if (TMRef[j].GetModelName() == ModelName)
                {
                    ModelFound = true;
                    //Debug.Log("Mapped Model Found ");
                    //Adding NetToModel

                    LayerData[] Data = new LayerData[Layers];
                    SR.ReadLine();
               
                    for (int k = 0; k < Layers; k++)
                    {
                        //Debug.Log("Layer " + k);
                        //Biases
                        int NumberOfNeuronsInLayer = 0;
                        string LineDataBiases = SR.ReadLine();
                        string[] LayerBiasesArrayString = LineDataBiases.Split(';');
                        float[] TempArray = new float[16];
                        //Debug.Log("Parsing Bias");
                        for (int n = 0; n < LayerBiasesArrayString.Length; n++)
                        {
                            if (float.TryParse(LayerBiasesArrayString[n],out float value))
                            {

                                // Debug.Log("Parsing Bias @ Neuron " + n);
                                TempArray[n] = value;
                                NumberOfNeuronsInLayer++;
                            }
                            else
                            {
                                // Debug.Log("Exit @ " + n);
                                goto Exit2;
                            }
                        }
                        Exit2:;

                        float[] LayerBiasesArray = new float[NumberOfNeuronsInLayer];
                        for (int m = 0; m < NumberOfNeuronsInLayer; m++)
                        {
                            LayerBiasesArray[m] = TempArray[m];
                        }

                        if(k < Layers-1)
                        {

                    
                        //Weights
                       // Debug.Log("Parsing Weights");
                        string LineDataWeights = SR.ReadLine();
                           // Debug.Log(LineDataWeights);

                            //--Getting The Number Of Neurons in the next Layer--//
                            SR2.ReadLine();
                        string NextLayerBias = SR2.ReadLine();
                        int NumberOfNeuronsInNextLayer = 0;
                        string[] NextLayerArray = NextLayerBias.Split(';');
                        for (int b = 0; b < NextLayerArray.Length; b++)
                        {
                            if (float.TryParse(NextLayerArray[b],out float f))
                            {
                                NumberOfNeuronsInNextLayer++;
                            }
                            else
                            {
                                goto Exit3;
                            }
                        }
                        Exit3:;
                        //---------------Setting Weights Value ----------------------//
              
                        float[,] LayerWeightsMatrix = new float[NumberOfNeuronsInLayer, NumberOfNeuronsInNextLayer];
                        string[] WeightsDataArray = LineDataWeights.Split(';');
                        string[] WeightsPerNeuron = null;
                       //     Debug.Log("NumberOfNeuronsInLayer " + NumberOfNeuronsInLayer);
                            for (int x = 0; x < NumberOfNeuronsInLayer; x++)
                            {
                         //       Debug.Log("Neuron "+ x);
                                WeightsPerNeuron = WeightsDataArray[x].Split('/');
                          //      Debug.Log("WeightsPerNeuron " + WeightsPerNeuron.Length);
                                for (int y = 0; y < WeightsPerNeuron.Length; y++)
                                {
                                 
                                if (float.TryParse(WeightsPerNeuron[y],out float f))
                                {
                            //            Debug.Log("Weight " + y + " = " + f);
                                        LayerWeightsMatrix[x,y] = f;
                                }
                                else
                                {
                              //          Debug.Log("NonFloatValue " + WeightsPerNeuron[y]);
                                        if (y < NumberOfNeuronsInNextLayer - 1)
                                        {
                              //          Debug.LogError("Data Formating Error , Less Weights than Neurons in Layer L + 1");
                                            goto Exit4;
                                        }
                                }
                                }
                            }
                            Exit4:;
                            //------------------SetLayerValuesAsComputOrInputLayer-------------------//

                            Data[k] = new LayerData(LayerBiasesArray, LayerWeightsMatrix);
                        }
                        else
                        {
                            //------------------SetLayerValuesAsOutputLayer-------------------//

                            Data[k] = new LayerData(LayerBiasesArray, null);
                        }

                    }

                    TMRef[j].AddNetwork(Data, ActFunc,IsAutoencoder,FtL,IncDec);

                    goto Exit;
                }
            }
            Exit:;
            if(!ModelFound)
            {
                Debug.LogError("No Model Found");
                //Error
            }
        }

    }

    //------------------------------Spawn System----------------------------//

    [SerializeField]
    Transform RefSpawnPos;
    [SerializeField]
    GameObject NPC;

    public void SpawnNPC(NeuralNetMainModule Net)
    {
        if(SpawnedNPCs.Count < 16)
        {

            GameObject npc = Instantiate(NPC, RefSpawnPos.position, Quaternion.identity);
           npc.GetComponent<NPC_Scr>().SetUpNetwork(Net);
           npc.GetComponent<NPC_Scr>().SetUpParentModel(UIManager.UIM.GetSimSpaceSelectedModel());

           SpawnedNPCs.Add(npc);

        }
    }

    public void ActivateDeactivateNPCs(bool value)
    {
        FoodSpawner.FS.gameObject.SetActive(value);
        for (int i = 0; i < SpawnedNPCs.Count; i++)
        {
            SpawnedNPCs[i].SetActive(value);
        }
        FoodSpawner.FS.OpenClose(value);
    }

    private List<GameObject> SpawnedNPCs;
    public void RemoveNPC(int ID)
    {
        SpawnedNPCs.RemoveAt(ID);
    }


    //----------------------------Debug------------------------//

    public void DebugModel2()
    {
       
            Model_3.PrintTrainingData();
        
    }

    //-----------------------------DataAnalysis-------------------------------//

    [SerializeField]
    string CodeLayerSavesFileLocation;



    public string GetCodeLayerSavesFileLocation()
    {
        return CodeLayerSavesFileLocation; 
    }

    public string SearchForEncoderDataFile(int ModelID, int NetID)
    {
        return null;
    }

    [SerializeField]
    string ExcellDataPrefab;
    public string GetExcellDataPrefab()
    {
        return ExcellDataPrefab;
    }


    
    

}
