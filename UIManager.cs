
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking.Types;
using UnityEngine.UI;
using UnityEngine.Windows;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;


public class UIManager : MonoBehaviour
{
    //----------------------------------------TopUICanvas------------------------//

    [SerializeField]
    GameObject TopUIPanel;

    public void OpenCloseTopUIPanel(bool value)
    {
        TopUIPanel.SetActive(value);
    }

    //----------------------------------------------------------------//

    [SerializeField]
    GameObject NeuronInterfacePrefab;
    [SerializeField]
    GameObject NetworkInterfacePrefab;

    public static UIManager UIM;

    private void Awake()
    {
        UIM = GetComponent<UIManager>();
    }

    /*
   //----------------------------------------NetPool------------------------//
    [SerializeField]
    RectTransform ScrollViewSize;

    [SerializeField]
    float NetDataSlotWidth;

    UINetSlot[] NetDataSlots;

    public void CreateNewNetPool(int NumberOfNetworks)
    {
        ScrollViewSize.rect.Set(ScrollViewSize.rect.x, ScrollViewSize.rect.y, NumberOfNetworks * NetDataSlotWidth, ScrollViewSize.rect.height);
        NetDataSlots = new UINetSlot[NumberOfNetworks];
        for (int i = 0; i < NumberOfNetworks; i++)
        {
            GameObject UIelem = Instantiate(NetworkInterfacePrefab,new Vector3(0,0,0),Quaternion.identity);
            UIelem.transform.SetParent(ScrollViewSize);
            UIelem.transform.localScale = new Vector3(1, 1,1);
            UIelem.transform.localPosition = new Vector3(128 + (NetDataSlotWidth * i), -256, 0);
            NetDataSlots[i] = UIelem.GetComponent<UINetSlot>();
            NetDataSlots[i].SetData(0,i,"NullData");
        }
      
    }
    public void DestroyNetPool() 
    {
        for (int i = 0; i < NeuronSlots.Length; i++)
        {
            NetDataSlots[i].DestroySlot();
        }
    }
    */

    //----------------------------------------NeuronsUI------------------------//

    public UnityEvent ExpandNeuron;//complex visual 

    public UnityEvent CloseNeuron; //simple visual 

    bool Expanded;
    bool ShowConnections;

    [SerializeField]
    Image ExpandCloseBTNImg;
    [SerializeField]
    Sprite ExpandIcon, CloseIcon;

    public void ExpandCloseNeurons()
    {
        if (Expanded)
        {
            CloseNeuron.Invoke();
            Expanded = false;
            ExpandCloseBTNImg.sprite = ExpandIcon;
        }
        else
        {
            ExpandNeuron.Invoke();
            Expanded = true;
            ExpandCloseBTNImg.sprite = CloseIcon;
        }
    }
    public void ShowHideConnections()
    {
        ShowConnections = !ShowConnections;
    }

    UINeuron[] NeuronSlots;


    NeuralNetMainModule ActiveNet;
    [SerializeField]
    RectTransform NetworkUIRefPos;
    [SerializeField]
    float NeuronUIHeight, NeuronUIWidth;
    [SerializeField]
    int SeparationMod;

    [SerializeField]
    Text CycleRate;
    [SerializeField]
    Text CycleCount;

    [SerializeField]
    Image PausePlayBtn;
    [SerializeField]
    Sprite[] PlayPauseSprites;

    [SerializeField]
    GameObject AutoEncoderUIObject;

    [SerializeField]
    float AutoEncoderInputOutputLayerDistanceToCenter; 
    [SerializeField]
    float AutoEncoderRealValueLayerDistanceToCenter;
    [SerializeField]
    GameObject RealValueDisplayPrefab;

    Text[] AutoEncoderRealValueDisplayArray;


    public void CyclePlayPauseBTN(bool value)
    {
        if (value)
        {
            PausePlayBtn.sprite = PlayPauseSprites[0];
        }
        else
        {
            PausePlayBtn.sprite = PlayPauseSprites[1];
        }
    }

    public void UpdateCycleCount()
    {
        CycleCount.text = "Cycle Count:" + '\n' + NetTrainingSystem.NTS.GetCycleCount().ToString();
    }
    public void UpdateCycleRate()
    {
        CycleRate.text = "Cycle Rate:" + '\n' + NetTrainingSystem.NTS.GetCycleRate().ToString();
    }

    int SelectedModelID, SelectedNetworkID;

    public int GetSelectedNetworkID()
    { return SelectedNetworkID; }
    public int GetSelectedModelID()
    { return SelectedModelID; }

    [SerializeField]
    Text LearningRateDisplay;
    public void UpdateLearningRate()
    {
        LearningRateDisplay.text = NetTrainingSystem.NTS.GetTrainingRate().ToString();
    }

    public void CreateNewNetworkUI(int NetworkID)
    {
           Debug.Log(">>>>>>>>>>>>>>> SelectedModelID " + SelectedModelID +" <<<<<<<<<<<<<<<<<");
        if(DataManager.DM.GetModelByID(SelectedModelID).MyNetworkType == TrainingModel.NetworkType.Supervised)
        {
            Debug.Log("Supervised Net Model");
            AutoEncoderUIObject.SetActive(false);
            if (ActiveNet != null && (SelectedNetworkID != NetworkID || SelectedModelID != SelectedModel))
            {
                DestroyNetworkUI();
            }
            SelectedNetworkID = NetworkID;
            SelectedModelID = SelectedModel;
            float B = NeuronUIWidth + SeparationMod;
            ActiveNet = DataManager.DM.GetNetworkRef(SelectedModel, NetworkID);
            NeuronSlots = new UINeuron[ActiveNet.GetNumberOfNeurons()];
            int NeuronCount = 0;
            //
            NeuronMatrixPosRef = new Transform[ActiveNet.GetNumberOfNeurons()][];
            //
            CreateWeightsTensor(ActiveNet.GetNetwork());
             for (int i = 0; i < ActiveNet.GetNumberOfLayers(); i++)
            {
                NeuronMatrixPosRef[i] = new Transform[ActiveNet.GetNumberOfNeuronsAtLayer(i)];
                for (int j = 0; j < ActiveNet.GetNumberOfNeuronsAtLayer(i); j++)
                {
                    GameObject Neuron = Instantiate(NeuronInterfacePrefab);
                    NeuronSlots[NeuronCount] = Neuron.GetComponent<UINeuron>();
                    NeuronCount++;
                    Neuron.transform.SetParent(NetworkUIRefPos);
                    Neuron.transform.localPosition = new Vector3((B / 2) + (B * j) - ((B * ActiveNet.GetNumberOfNeuronsAtLayer(i)) / 2), NeuronUIHeight / 2 + 32 + i * (NeuronUIHeight + 32), 0);
                    Neuron.transform.localScale = new Vector3(1, 1, 1);
                    NeuronMatrixPosRef[i][j] = Neuron.transform;
                }
            }
        }
        else
        {
            Debug.Log("Unsupervised Net Model");
            AutoEncoderUIObject.SetActive(true);
            if (ActiveNet != null && (SelectedNetworkID != NetworkID || SelectedModelID != SelectedModel))
            {
                DestroyNetworkUI();
            }
            SelectedNetworkID = NetworkID;
            SelectedModelID = SelectedModel;
            float B = NeuronUIWidth + SeparationMod;
            ActiveNet = DataManager.DM.GetNetworkRef(SelectedModel, NetworkID);
            ActiveNet.GetAutoencoderData(out int a, out int b);
            //a number of neurons in Input Layer
            //b number of neurons in feature layer

            //Debug.Log("number of neurons in Input Layer " + a );
            //Debug.Log("number of neurons in feature layer " + b);
            NeuronSlots = new UINeuron[(a * 2) + b];
            NeuronMatrixPosRef = new Transform[3][];
            NeuronMatrixPosRef[0] = new Transform[a];
            NeuronMatrixPosRef[1] = new Transform[b];
            NeuronMatrixPosRef[2] = new Transform[a];
            CreateWeightsTensor(ActiveNet.GetNetwork());

            int NeuronCount = 0;

            AutoEncoderRealValueDisplayArray  = new Text[a * 2];

            for (int i = 0; i < a; i++)
            {
                //Debug.Log("Spawn  Input Layer ");
                GameObject Neuron = Instantiate(NeuronInterfacePrefab);

                NeuronSlots[NeuronCount] = Neuron.GetComponent<UINeuron>();
                NeuronCount++;

                Neuron.transform.SetParent(NetworkUIRefPos);
                Neuron.transform.localPosition = new Vector3(-AutoEncoderInputOutputLayerDistanceToCenter, (((float)a/2)* -NeuronUIHeight) + (NeuronUIHeight / 2)  + i * (NeuronUIHeight ), 0);
                Neuron.transform.localScale = new Vector3(1, 1, 1);
                NeuronMatrixPosRef[0][i] = Neuron.transform;


                GameObject RealValueDisp = Instantiate(RealValueDisplayPrefab, Vector3.zero, Quaternion.identity);
                RealValueDisp.transform.SetParent(NetworkUIRefPos);
                RealValueDisp.transform.localPosition = new Vector3(-AutoEncoderRealValueLayerDistanceToCenter, ((a * -NeuronUIHeight) / 2) + (NeuronUIHeight / 2) + i * (NeuronUIHeight), 0);
                RealValueDisp.transform.localScale = new Vector3(1, 1, 1);
                AutoEncoderRealValueDisplayArray[i] = RealValueDisp.GetComponent<Text>();

            }//Spawn Input Layer 
            for (int i = 0; i < b; i++)
            {
                //Debug.Log("Spawn  Feature Layer ");
                GameObject Neuron = Instantiate(NeuronInterfacePrefab);

                NeuronSlots[NeuronCount] = Neuron.GetComponent<UINeuron>();
                NeuronCount++;

                Neuron.transform.SetParent(NetworkUIRefPos);
                Neuron.transform.localPosition = new Vector3(0, (((float)b/2) * -NeuronUIHeight) + (NeuronUIHeight / 2) + i * (NeuronUIHeight), 0);
                Neuron.transform.localScale = new Vector3(1, 1, 1);
                NeuronMatrixPosRef[1][i] = Neuron.transform;
            }//Spawn Feature Layer
            for (int i = 0; i < a; i++)
            {
                //Debug.Log("Spawn  Output Layer ");
                GameObject Neuron = Instantiate(NeuronInterfacePrefab);

                NeuronSlots[NeuronCount] = Neuron.GetComponent<UINeuron>();
                NeuronCount++;

                Neuron.transform.SetParent(NetworkUIRefPos);
                Neuron.transform.localPosition = new Vector3(AutoEncoderInputOutputLayerDistanceToCenter, ((a * -NeuronUIHeight)/2) + (NeuronUIHeight / 2) + i * (NeuronUIHeight), 0);
                Neuron.transform.localScale = new Vector3(1, 1, 1);
                NeuronMatrixPosRef[2][i] = Neuron.transform;

                GameObject RealValueDisp = Instantiate(RealValueDisplayPrefab, Vector3.zero, Quaternion.identity);
                RealValueDisp.transform.SetParent(NetworkUIRefPos);
                RealValueDisp.transform.localPosition = new Vector3(AutoEncoderRealValueLayerDistanceToCenter, ((a * -NeuronUIHeight) / 2) + (NeuronUIHeight / 2) + i * (NeuronUIHeight), 0);
                RealValueDisp.transform.localScale = new Vector3(1, 1, 1);

                AutoEncoderRealValueDisplayArray[a + i] = RealValueDisp.GetComponent<Text>();


            }//Spawn  Output Layer 



        }
    }
    public void DestroyNetworkUI()
    {
        AutoEncoderUIObject.SetActive(false);
        WeightsTensor = null; NeuronMatrixPosRef = null;
        SelectedNetworkID = -1;
        SelectedModelID = -1;
        ActiveNet = null;
        for (int i = 0; i < NeuronSlots.Length; i++)
        {
            NeuronSlots[i].DestroyNeuron();
        }
    }

    [SerializeField]
    GameObject NetViewMode;
    public void OpenCloseNetViewMode(bool value)
    {
        NetViewMode.SetActive(value);
        if (value)
        {
            CloseModelInspector();
            UpdateNetDisplayInfoData();
        }
        else
        {
            NetTrainingSystem.NTS.StopTraining();
        }
    }

    float[][][] WeightsTensor;

    Transform[][] NeuronMatrixPosRef;

    private void CreateWeightsTensor(NeuronScr[][] Net)
    {
        WeightsTensor = new float[Net.Length][][];
        for (int i = 0; i < Net.Length; i++)
        {
            WeightsTensor[i] = new float[Net[i].Length][];

            for (int j = 0; j < Net[i].Length; j++)
            {
                if (i < Net.Length - 1)
                {
                    WeightsTensor[i][j] = new float[Net[i + 1].Length];
                }
                else
                {
                    WeightsTensor[i][j] = new float[0];
                }
            }
        }
    }
    public void UpdateConnections()
    {
        if (SelectedNetworkID >= 0)
        {
            // Debug.Log("======================================Updating Connections==================================");
            for (int i = 0; i < WeightsTensor.Length; i++)
            {
                for (int j = 0; j < WeightsTensor[i].Length; j++)
                {
                    for (int k = 0; k < WeightsTensor[i][j].Length; k++)
                    {
                        if (WeightsTensor[i][j][k] > 0)
                        {
                            // Debug.Log("[" + i + "]"+ "[" + j + "]"+ "[" + k + "] = " + WeightsTensor[i][j][k]);
                            Debug.DrawLine(NeuronMatrixPosRef[i][j].position, NeuronMatrixPosRef[i + 1][k].position, Color.Lerp(Color.black, Color.blue, WeightsTensor[i][j][k]), 15f);
                        }
                        else
                        {
                            // Debug.Log("[" + i + "]" + "[" + j + "]" + "[" + k + "] = " + WeightsTensor[i][j][k]);
                            Debug.DrawLine(NeuronMatrixPosRef[i][j].position, NeuronMatrixPosRef[i + 1][k].position, Color.Lerp(Color.black, Color.red, -WeightsTensor[i][j][k]), 15f);
                        }

                    }
                }
            }

        }
    }
    public void SetWeightTensor(int Layer, int Neuron, int ConnectionID, float value)
    {
        if (WeightsTensor != null)
            WeightsTensor[Layer][Neuron][ConnectionID] = value;
    }
    public void UpdateNeuronValues()
    {
        if (SelectedNetworkID >= 0)
        {
            if(ActiveNet.GetMyTrainingType() == TrainingModel.NetworkType.Supervised)
            {
                NeuronScr[][] Net = ActiveNet.GetNetwork();
                int count = 0;
                for (int i = 0; i < Net.Length; i++)
                {
                    for (int j = 0; j < Net[i].Length; j++)
                    {
                       // Debug.Log("SettingValueNeuron [" + i + "]" + "[" + j + "]");
                        NeuronSlots[count].SetValues(Net[i][j].GetSigmaValue(), Net[i][j].GetActivationValue());
                        count++;
                    }
                }
            }
            else
            {
                float[] Maxes = DataManager.DM.GetModelByID(SelectedModelID).GetInputMaxArray();
                NeuronScr[][] Net = ActiveNet.GetNetwork();
                int count = 0;
                int NumberOfLayers = ActiveNet.GetNumberOfLayers();
                int i = 0;
                for (int j = 0; j < Net[i].Length; j++)
                {
                    //  Debug.Log("SettingValueNeuron [" + i + "]" + "[" + j + "]");
                    string str = ((Net[i][j].GetActivationValue() * Maxes[j])).ToString() ;
                    //Debug.Log(str) ;
                    AutoEncoderRealValueDisplayArray[j].text  = str.Substring(0, Mathf.Clamp(4, 1, str.Length));
                    NeuronSlots[count].SetValues(Net[i][j].GetSigmaValue(), Net[i][j].GetActivationValue());
                    count++;
                }
                int count2 = count;
                i = (NumberOfLayers - 1 )/ 2;
                for (int j = 0; j < Net[i].Length; j++)
                {
                  
                    //Debug.Log("SettingValueNeuron [" + i + "]" + "[" + j + "]");
                    NeuronSlots[count].SetValues(Net[i][j].GetSigmaValue(), Net[i][j].GetActivationValue());
                    count++;
                }
                i = NumberOfLayers - 1;
                for (int j = 0; j < Net[i].Length; j++)
                {
                    // Debug.Log("SettingValueNeuron [" + i + "]" + "[" + j + "]");
                    string str = ((Net[i][j].GetActivationValue() * Maxes[j])).ToString();
                    AutoEncoderRealValueDisplayArray[count2 + j].text = str.Substring(0, 4);
                    NeuronSlots[count].SetValues(Net[i][j].GetSigmaValue(), Net[i][j].GetActivationValue());
                    count++;
                }
            }
        }
    }

    [SerializeField]
    Text NetDisplay_NetID, NetDisplay_ModelName, NetDisplay_Loss;
    public void UpdateNetLossInfoData()
    {
        NetDisplay_Loss.text = "Loss: " + ActiveNet.GetLoss().ToString();
    }
    private void UpdateNetDisplayInfoData()
    {
        if (ActiveNet != null)
        {
            NetDisplay_Loss.text = "Loss: " + ActiveNet.GetLoss().ToString();
            NetDisplay_NetID.text = "Net ID " + SelectedNetworkID.ToString();
            switch (SelectedModelID)
            {
                case 0:
                    NetDisplay_ModelName.text = "Model ID " + DataManager.DM.GetModel1().GetModelName().ToString();
                    break;
                case 1:
                    NetDisplay_ModelName.text = "Model ID " + DataManager.DM.GetModel2().GetModelName().ToString();
                    break;
                case 2:
                    NetDisplay_ModelName.text = "Model ID " + DataManager.DM.GetModel3().GetModelName().ToString();
                    break;
                case 3:
                    NetDisplay_ModelName.text = "Model ID " + DataManager.DM.GetModel4().GetModelName().ToString();
                    break;
                case 4:
                    NetDisplay_ModelName.text = "Model ID " + DataManager.DM.GetModel5().GetModelName().ToString();
                    break;
                case 5:
                    NetDisplay_ModelName.text = "Model ID " + DataManager.DM.GetModel6().GetModelName().ToString();
                    break;
            }

        }
    }

    //---------------------------------------ModelManager-------------------------//
    // --------------------------ModelView----------------------------------//
    [SerializeField]
    GameObject FileDirectoryDisplay;

    public void ShowFileDirectory(bool value)
    {
        FileDirectoryDisplay.SetActive(value);
    }

    [SerializeField]
    GameObject ModelView;

    [SerializeField]
    ModelSlotDataScr[] ModelSlots;

    public void UpdateModels()
    {
        TrainingModel[] Array = DataManager.DM.GetTrainingModelsArray();
        for (int i = 0; i < 6; i++)
        {
            ModelSlots[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < Array.Length; i++)
        {
            ModelSlots[i].gameObject.SetActive(true);


            if (Array[i].MyNetworkType == TrainingModel.NetworkType.Supervised)
            {
                ModelSlots[i].SetUp(Array[i].GetModelName(), Array[i].GetID(), "I" + Array[i].GetNumberOfInputs() + " O" + Array[i].GetNumberOfOutputs() + " S" + Array[i].GetNumberOfLayers(), Array[i].GetNumberOfNetworks().ToString(), Array[i].GetNumberOfTrainingSets().ToString(), Array[i].MyNetworkType);

            }
            else
            {
                ModelSlots[i].SetUp(Array[i].GetModelName(), Array[i].GetID(), "Unsupervised", Array[i].GetNumberOfNetworks().ToString(),  Array[i].GetNumberOfTrainingSets().ToString(), Array[i].MyNetworkType);

            }

        }
    }

    public void OpenCloseModelView(bool value)
    {
        if (value)
        {
            ModelView.SetActive(true);
            UpdateModels();
            SetUpCustomInputs(false);
        }
        else
        {
            ModelView.SetActive(false);
            SelectedModelID = -1; SelectedModel = -1; SelectedNetworkID = -1;
        }
    }

    public void DeleteModel(int ID)
    {

    }

    //--------------------------ModelInspector----------------------------//

    [SerializeField]
    GameObject ModelInspector;

    [SerializeField]
    NetSlotDataScr[] NetSlots;

    [SerializeField]
    Text ModelData, ModelName, ModelNetworkCount, ModelTrainingDataSetsCount;

    public void CloseModelInspector()
    {
        ModelInspector.SetActive(false);
    }
    public void OpenCloseModelInspector(bool value, int ModelID)
    {
        if (value)
        {
            TrainingModel TM = null;
            ModelView.SetActive(false);
            ModelInspector.SetActive(true);

            int a = 0;
            switch (ModelID)
            {
                case 0:
                    a = DataManager.DM.GetModel1().GetNumberOfNetworks();
                    TM = DataManager.DM.GetModel1();
                    break;
                case 1:
                    a = DataManager.DM.GetModel2().GetNumberOfNetworks();
                    TM = DataManager.DM.GetModel2();
                    break;
                case 2:
                    a = DataManager.DM.GetModel3().GetNumberOfNetworks();
                    TM = DataManager.DM.GetModel3();
                    break;
                case 3:
                    a = DataManager.DM.GetModel4().GetNumberOfNetworks();
                    TM = DataManager.DM.GetModel4();
                    break;
                case 4:
                    a = DataManager.DM.GetModel5().GetNumberOfNetworks();
                    TM = DataManager.DM.GetModel5();
                    break;
                case 5:
                    a = DataManager.DM.GetModel6().GetNumberOfNetworks();
                    TM = DataManager.DM.GetModel6();
                    break;
            }
            for (int i = 0; i < NetSlots.Length; i++)
            {
                if (i < a)
                {
                    NetSlots[i].gameObject.SetActive(true);
                    NetSlots[i].SetUp(TM.GetNetworkByID(i).GetNameID(), TM.GetNetworkByID(i).GetCycles(), TM.GetNetworkByID(i).GetLoss(), i, TM.GetNetworkByID(i).GetWasAlteredAfterSaving(),TM.MyNetworkType);
                    
                }
                else
                {
                    NetSlots[i].gameObject.SetActive(false);
                }
            }

            ModelData.text = "I" + TM.GetNumberOfInputs().ToString() + " O" + TM.GetNumberOfOutputs().ToString() + " S" + TM.GetNumberOfLayers().ToString();
            ModelName.text = TM.GetModelName();
            ModelNetworkCount.text = TM.GetNumberOfNetworks().ToString();
            ModelTrainingDataSetsCount.text = TM.GetNumberOfTrainingSets().ToString();


            SelectedModel = ModelID;
            SelectedModelID = ModelID;
        }
        else
        {
            ModelInspector.SetActive(false);
        }
    }

    int SelectedModel;

    public void TryCreateNewNet()
    {
        switch (SelectedModel)
        {
            case 0:
                DataManager.DM.GetModel1().CreateNewNetwork(); UpdateNetsDisplay(); //Debug.Log("Try Create New Net @Model1");
                ModelNetworkCount.text = DataManager.DM.GetModel1().GetNumberOfNetworks().ToString();
                break;
            case 1:
                DataManager.DM.GetModel2().CreateNewNetwork(); UpdateNetsDisplay(); //Debug.Log("Try Create New Net @Model2");
                ModelNetworkCount.text = DataManager.DM.GetModel2().GetNumberOfNetworks().ToString();
                break;
            case 2:
                DataManager.DM.GetModel3().CreateNewNetwork(); UpdateNetsDisplay(); //Debug.Log("Try Create New Net @Model3");
                ModelNetworkCount.text = DataManager.DM.GetModel3().GetNumberOfNetworks().ToString();
                break;
            case 3:
                DataManager.DM.GetModel4().CreateNewNetwork(); UpdateNetsDisplay(); //Debug.Log("Try Create New Net @Model4");
                ModelNetworkCount.text = DataManager.DM.GetModel4().GetNumberOfNetworks().ToString();
                break;
            case 4:
                DataManager.DM.GetModel5().CreateNewNetwork(); UpdateNetsDisplay(); //Debug.Log("Try Create New Net @Model5");
                ModelNetworkCount.text = DataManager.DM.GetModel5().GetNumberOfNetworks().ToString();
                break;
            case 5:
                DataManager.DM.GetModel6().CreateNewNetwork(); UpdateNetsDisplay(); //Debug.Log("Try Create New Net @Model6");
                ModelNetworkCount.text = DataManager.DM.GetModel6().GetNumberOfNetworks().ToString();
                break;
        }


    }

    public void UpdateNetsDisplay()
    {
        TrainingModel TM = null;
        int a = 0;
        switch (SelectedModel)
        {
            case 0:
                a = DataManager.DM.GetModel1().GetNumberOfNetworks();
                TM = DataManager.DM.GetModel1();
                break;
            case 1:
                a = DataManager.DM.GetModel2().GetNumberOfNetworks();
                TM = DataManager.DM.GetModel2();
                break;
            case 2:
                a = DataManager.DM.GetModel3().GetNumberOfNetworks();
                TM = DataManager.DM.GetModel3();
                break;
            case 3:
                a = DataManager.DM.GetModel4().GetNumberOfNetworks();
                TM = DataManager.DM.GetModel4();
                break;
            case 4:
                a = DataManager.DM.GetModel5().GetNumberOfNetworks();
                TM = DataManager.DM.GetModel5();
                break;
            case 5:
                a = DataManager.DM.GetModel6().GetNumberOfNetworks();
                TM = DataManager.DM.GetModel6();
                break;
        }
        for (int i = 0; i < NetSlots.Length; i++)
        {
            if (i < a)
            {
                NetSlots[i].gameObject.SetActive(true);
                NetSlots[i].SetUp(TM.GetNetworkByID(i).GetNameID(), TM.GetNetworkByID(i).GetCycles(), TM.GetNetworkByID(i).GetLoss(), i, TM.GetNetworkByID(i).GetWasAlteredAfterSaving(), TM.MyNetworkType);
            }
            else
            {
                NetSlots[i].gameObject.SetActive(false);
            }
        }



    }

    //--------------------------CUSTOM INPUTS SYSTEM----------------------------//
    [SerializeField]
    GameObject CustomInputPanel;

    [SerializeField]
    GameObject CustomInputSubPanel;

    [SerializeField]
    CustomInputSlotScr[] CustomInputsObj;

    float[] CustomInputValues = new float[12];

    int ActiveSlots = 0;

    public void SetUpCustomInputs(bool OpenClose)
    {
        if (OpenClose)
        {

            ActiveSlots = NeuronMatrixPosRef[0].Length;
            CustomInputPanel.SetActive(true);
            for (int i = 0; i < CustomInputsObj.Length; i++)
            {
                if (i < ActiveSlots)
                {
                    CustomInputsObj[i].gameObject.SetActive(true);
                    CustomInputsObj[i].SetTo0();
                }
                else
                {
                    CustomInputsObj[i].gameObject.SetActive(false);
                }
                CustomInputValues[i] = 0;
            }
        }
        else
        {
            CustomInputPanel.SetActive(false);
        }
    }

    public void MinMaxCustomInputsView()
    {
        if (CustomInputSubPanel.activeInHierarchy)
        {

            CustomInputSubPanel.SetActive(false);
        }
        else
        {
            CustomInputSubPanel.SetActive(true);
        }
    }

    public float ModifyCustomInputValue(float delta, int ID)
    {
        CustomInputValues[ID] += delta;
        if (CustomInputValues[ID] < -1)
        {
            CustomInputValues[ID] = -1;
        }
        else if (CustomInputValues[ID] > 1)
        {
            CustomInputValues[ID] = 1;
        }


        return CustomInputValues[ID];
    }

    public float[] GetCustomInputValues()
    { return CustomInputValues; }


    //----------------------------SimulationSpace----------------------//
    [SerializeField]
    GameObject SimSpace;

    [SerializeField]
    GameObject SimSpaceModuleSelection;
    [SerializeField]
    GameObject SimSpaceNetworkSelection;

    public void OpenCloseSimSpace(bool value)
    {
        if (value)
        {
            SimSpace.SetActive(true);
            OpenCloseModelView(false);
            CloseModelInspector(); 
            OpenCloseNetViewMode(false);
        }
        else
        {
            SimSpace.SetActive(false);
          
        }
        DataManager.DM.ActivateDeactivateNPCs(value);
        OpenCloseTopUIPanel(!value);
    }

    [SerializeField]
    SimSpaceSelectionSlot[] ModuleDisplaySlots;
    [SerializeField]
    SimSpaceSelectionSlot[] NetworkDisplaySlots;

    int SimSpaceSelectedModel;
    public int GetSimSpaceSelectedModel() { return SimSpaceSelectedModel; }
    public void OpenCloseSimSpaceModuleDisplay(bool value)
    {
        if(value)
        {
            for (int i = 0; i < DataManager.DM.GetNumberOfTrainingModels(); i++)
            {

                ModuleDisplaySlots[i].gameObject.SetActive(true);
                if (DataManager.DM.GetModelByID(i).MyNetworkType == TrainingModel.NetworkType.Supervised)
                {
                    ModuleDisplaySlots[i].SetData(DataManager.DM.GetModelByID(i).GetModelName(), DataManager.DM.GetModelByID(i).GetMetaData());

                }
                else
                {
                    ModuleDisplaySlots[i].SetData("ND", "UnsupervisedModel");

                }
            }
            for (int i = DataManager.DM.GetNumberOfTrainingModels(); i < ModuleDisplaySlots.Length; i++)
            {
                ModuleDisplaySlots[i].gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < ModuleDisplaySlots.Length; i++)
            {
                ModuleDisplaySlots[i].gameObject.SetActive(false);
            }
        }
        SimSpaceModuleSelection.SetActive(value); InputCustomizationArea.SetActive(value);
    }

    public void OpenCloseSimSpaceNetworkSelection(bool value,int ID)
    { 
        SimSpaceNetworkSelection.SetActive(value);
        if(value)
        {
            SimSpaceSelectedModel = ID;
            int NumberOfNets = DataManager.DM.GetModelByID(SimSpaceSelectedModel).GetNumberOfNetworks();
            Debug.Log("NumberOfNets " + NumberOfNets);
            for (int i = 0; i < NetworkDisplaySlots.Length; i++)
            {
                if(i < NumberOfNets)
                {
                    NetworkDisplaySlots[i].gameObject.SetActive(true);
                    NetworkDisplaySlots[i].SetData(DataManager.DM.GetModelByID(SimSpaceSelectedModel).GetNetworkByID(i).GetNameID(), DataManager.DM.GetModelByID(SimSpaceSelectedModel).GetNetworkByID(i).GetLoss().ToString());

                }
                else
                {
                    NetworkDisplaySlots[i].gameObject.SetActive(false);
                }



            }
           
                
            
        }
        else
        {
            SelectedModel = -1;
        }
    }

    //-------------------------InputCustomization----------------------//

    [SerializeField]
    GameObject InputCustomizationArea;

    [SerializeField]
    Sprite[] InputMappingIcons;
    [SerializeField]
    Sprite[] OutputMappingIcons;

    public void OpenCloseInputCustomizationArea(bool value)
    {
        OpenCloseInputOutputCustomFields(value);
        if (value)
        {
            InputCustomizationArea.SetActive(true);
            int NumberOfInputs = DataManager.DM.GetModelByID(SelectedModelID).GetNumberOfInputs();
            Debug.Log("Number OF Inputs " + NumberOfInputs);
            for (int i = 0; i < SimSpaceInputNeurons.Length; i++)
            {
                if (i < NumberOfInputs)
                {
                    
                    SimSpaceInputNeurons[i].gameObject.SetActive(true);
                    SimSpaceInputMapping[i].gameObject.SetActive(true);
                    int id = DataManager.DM.GetModelByID(SimSpaceSelectedModel).GetCustomInputMapping()[i];

                    Debug.Log(id);
                    SimSpaceInputNeurons[i].SetIcon(InputMappingIcons[id], id);                 
                }
                else
                {
                    SimSpaceInputNeurons[i].gameObject.SetActive(false);
                    SimSpaceInputMapping[i].gameObject.SetActive(false);
                }
            }
            int NumberOfOutputs = DataManager.DM.GetModelByID(SelectedModelID).GetNumberOfOutputs();
            Debug.Log("Number Of Outputs " + NumberOfOutputs);
            for (int i = 0; i < SimSpaceOutputNeurons.Length; i++)
            {
                if (i < NumberOfOutputs)
                {
                    SimSpaceOutputNeurons[i].gameObject.SetActive(true);
                    SimSpaceOutputMapping[i].gameObject.SetActive(true);
                    int id = DataManager.DM.GetModelByID(SimSpaceSelectedModel).GetCustomOutputMapping()[i];

                    Debug.Log(id);
                    SimSpaceOutputNeurons[i].SetIcon(OutputMappingIcons[id], id);
                }
                else
                {
                    SimSpaceOutputNeurons[i].gameObject.SetActive(false);
                    SimSpaceOutputMapping[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            InputCustomizationArea.SetActive(false);
        }
    }

    int SimSpaceSelectedTrainingModel; int SimSpaceSelectedNetwork;

    int CustomInputAreaSelectedSlotID; int CustomOutputAreaSelectedSlotID;

    [SerializeField]
    GameObject InputCustomField;
    [SerializeField]
    GameObject OutputCustomField;
    [SerializeField]
    GameObject SimSpaceCloseBTN;
    public void OpenCloseInputOutputCustomFields(bool value)
    {
        InputCustomField.SetActive(value);
        OutputCustomField.SetActive(value);
        SimSpaceCloseBTN.SetActive(value);
    }

    [SerializeField]
    SlotCustomizationScr[] SimSpaceInputNeurons;
    [SerializeField]
    SlotCustomizationScr[] SimSpaceInputMapping;
    [SerializeField]
    SlotCustomizationScr[] SimSpaceOutputNeurons;
    [SerializeField]
    SlotCustomizationScr[] SimSpaceOutputMapping;

    public void CallNewSelection(int ID,SlotCustomizationScr.SlotCustomizationUIType Type)
    {
        switch (Type)
        {
            case SlotCustomizationScr.SlotCustomizationUIType.InputNeuron:
                if(ID == CustomInputAreaSelectedSlotID)
                {
                    //null
                }
                else
                {
                    SimSpaceInputNeurons[CustomInputAreaSelectedSlotID].Deselect();
                    CustomInputAreaSelectedSlotID = ID;
                }
                if (CustomOutputAreaSelectedSlotID != -1)
                {
                    SimSpaceOutputNeurons[CustomOutputAreaSelectedSlotID].Deselect();
                    CustomOutputAreaSelectedSlotID = -1;
                }
                // NEW REF
                break;
            case SlotCustomizationScr.SlotCustomizationUIType.InputMappingType:
                if(CustomInputAreaSelectedSlotID != -1)
                {
                    SimSpaceInputNeurons[CustomInputAreaSelectedSlotID].SetMappedId(ID);

                    DataManager.DM.GetModelByID(SelectedModel).SetCustomInputMapAtID(CustomInputAreaSelectedSlotID, ID);

                    SimSpaceInputNeurons[CustomInputAreaSelectedSlotID].SetIcon(InputMappingIcons[ID], ID);
                }
                if (CustomOutputAreaSelectedSlotID != -1)
                {
                    SimSpaceOutputNeurons[CustomOutputAreaSelectedSlotID].Deselect();
                    CustomOutputAreaSelectedSlotID = -1;
                }
                //Deselect Output panel
                //NEW MAPPING
                break;
            case SlotCustomizationScr.SlotCustomizationUIType.OutputNeuron:

                if(ID == CustomOutputAreaSelectedSlotID)
                {
                    //null
                }
                else
                {
                    SimSpaceOutputNeurons[CustomOutputAreaSelectedSlotID].Deselect();
                    CustomOutputAreaSelectedSlotID = ID;
                }
                if(CustomInputAreaSelectedSlotID != -1)
                {
                    SimSpaceInputNeurons[CustomInputAreaSelectedSlotID].Deselect();
                    CustomInputAreaSelectedSlotID = -1;
                }
                //NEW REF
                break;
            case SlotCustomizationScr.SlotCustomizationUIType.OutputMappingType:
                if (CustomOutputAreaSelectedSlotID != -1)
                {
                    SimSpaceOutputNeurons[CustomOutputAreaSelectedSlotID].SetMappedId(ID);

                    DataManager.DM.GetModelByID(SimSpaceSelectedModel).SetCustomOutputMapAtID(CustomOutputAreaSelectedSlotID, ID);

                    SimSpaceOutputNeurons[CustomOutputAreaSelectedSlotID].SetIcon(OutputMappingIcons[ID], ID); 
                }
                if (CustomInputAreaSelectedSlotID != -1)
                {
                    SimSpaceInputNeurons[CustomInputAreaSelectedSlotID].Deselect();
                    CustomInputAreaSelectedSlotID = -1;
                }
                //NEW MAPPING
                break;
        }
    }

    //Camer Move System

    [SerializeField]
    Transform MinXPos, MaxXPos, MaxYPos, MinYPos;

    float CameraWidth,CameraHeight;
    public void MoveCamera(Vector2 direction )
    {
        if(direction.x + Camera.main.transform.position.x + CameraWidth > MinXPos.position.x
            && direction.x + Camera.main.transform.position.x + CameraWidth < MaxXPos.position.x
            && direction.y + Camera.main.transform.position.y + CameraWidth > MinYPos.position.y
            && direction.y + Camera.main.transform.position.y + CameraWidth < MaxYPos.position.y
            )
        {
            Camera.main.transform.Translate(direction, Space.World);
        }
    }

    //-------------------------------FEATURE LAYER ANALYSIS ------------------//

    [SerializeField]
    GameObject FeatureLayerAnalysisUI;

    [SerializeField]
    RectTransform Graph;
    [SerializeField]
    GameObject DotPrefab;

    Transform[] GraphValues;

    bool AnalysisUI_LogFileExists;
    bool AnalysisUI_LogFileIsUpToDate;
    bool AnalysisUI_PCAFileExists;
    bool AnalysisUI_PCAFileIsUpToDate;

    private string LogFileAdress;
    private string PCAFileAdress;
    string[] LogFileData;
    string[] PCAFileData;

    [SerializeField]
    Text AnalysisUI_WarningsAndInfoData;
    [SerializeField]
    Text AnalysisUI_PCA_MetaData;
    [SerializeField]
    Text AnalysisUI_PCA_SubData;

    [SerializeField]
    Text AnalysisUI_ModelIDNetIDDisplay;
    [SerializeField]
    Text AnalysisUI_LogFileDirectory;
     
    public void OpenCloseFtLAnalysis(bool value)
    {
        Debug.Log("OpenCloseFtLAnalysis(bool value)");
        if (value)
        {
            //OpenCloseNetViewMode(true);
            
            //Search For Log File With Model and Net ID
            if (ExcelManager.EM.SearchFileTryGetData(ExcelManager.FileType.EncoderData_RawData, SelectedModel, SelectedNetworkID, out  LogFileAdress, out LogFileData))
            {
                AnalysisUI_LogFileExists = true;
                //Serch for PCA file with Model and Net ID
                if (ExcelManager.EM.SearchFileTryGetData(ExcelManager.FileType.EncoderData_PCA, SelectedModel, SelectedNetworkID, out string PCAFileAdress, out PCAFileData))
                {
                    AnalysisUI_PCAFileExists = true;
                }
                else
                {
                    AnalysisUI_PCAFileExists = false;
                }
            }
            else
            {
                AnalysisUI_LogFileExists = false;
            }
        }
        else
        {
            OpenCloseNetViewMode(false);
            DestroyGraph();
        }
        FeatureLayerAnalysisUI.SetActive(value);
        UpdateFtlAnalysisUI();
    }

    public void OpenCloseFtLAnalysis(bool value,int NetID)
    {
        Debug.Log("OpenCloseFtLAnalysis(bool value,int NetID)");
        SelectedNetworkID = NetID;
        if (value)
        {
            //OpenCloseNetViewMode(true);
            
            //Search For Log File With Model and Net ID
            if(  ExcelManager.EM.SearchFileTryGetData(ExcelManager.FileType.EncoderData_RawData, SelectedModel, SelectedNetworkID,out  LogFileAdress, out  LogFileData))
            {
                Debug.Log("1 " + LogFileAdress);
                AnalysisUI_LogFileExists = true;
                //Serch for PCA file with Model and Net ID
                if (ExcelManager.EM.SearchFileTryGetData(ExcelManager.FileType.EncoderData_PCA, SelectedModel, SelectedNetworkID, out  PCAFileAdress, out  PCAFileData))
                {
                    AnalysisUI_PCAFileExists = true;
                }
                else
                {
                    AnalysisUI_PCAFileExists = false;
                }
            }
            else
            {
                AnalysisUI_LogFileExists = false;
            }
            Debug.Log("2 " + LogFileAdress);
            UpdateFtlAnalysisUI();
            Debug.Log("5 " + LogFileAdress);
        }
        else
        {
            OpenCloseNetViewMode(false);
            DestroyGraph();
        }
        FeatureLayerAnalysisUI.SetActive(value);
    }

    private void UpdateFtlAnalysisUI()
    {
        AnalysisUI_ModelIDNetIDDisplay.text = "Model " + SelectedModelID + " | Net " +SelectedNetworkID;
        Debug.Log("3 " + LogFileAdress);
        //check for Data Log Files for current network architecture
        if (AnalysisUI_LogFileExists )
        {
            Debug.Log("4 " + LogFileAdress);
            AnalysisUI_LogFileDirectory.text = LogFileAdress;
            AnalysisUI_LogFileDirectory.color = Color.Lerp(Color.green, Color.cyan, 0.3f);
            Debug.Log("AnalysisUI_LogFileExists");
            if (AnalysisUI_PCAFileExists)
            {
                AnalysisUI_PassWarning(AnalysisUIWarnings.Null); 
                Debug.Log("AnalysisUI_PCAFileExists");
            }
            else
            {
                AnalysisUI_PassWarning(AnalysisUIWarnings.NoPCAFileFound); 
                Debug.Log("!AnalysisUI_PCAFileExists");
            }
        }
        else
        {
            Debug.Log("!AnalysisUI_LogFileExists");
            AnalysisUI_LogFileDirectory.text = "Null";
            AnalysisUI_LogFileDirectory.color = Color.Lerp(Color.red, Color.white, 0.2f);

            AnalysisUI_PassWarning(AnalysisUIWarnings.NoLogFileFound);
        }
    }

    enum AnalysisUIWarnings
    {
        Null,
        NoLogFileFound,
        LogRequiredForPCA,
        LogFileAreadyExists,
        PCAFileAreadyExists,
        LogFileNotUpToDate,
        PCAFileNotUpToDate,
        NoPCAFileFound,
    }

    private void AnalysisUI_PassWarning(AnalysisUIWarnings type)
    {
        string str = null;
        switch (type)
        {
            case AnalysisUIWarnings.NoLogFileFound:
                str = "Log File Not Found...";
                break;
            case AnalysisUIWarnings.LogRequiredForPCA:
                str = "Log File Required For PCA...";
                break;
            case AnalysisUIWarnings.LogFileAreadyExists:
                str = "Log File Already Exists for current Net...";
                break;
            case AnalysisUIWarnings.PCAFileAreadyExists:
                str = "PCA File Already Exists for current Log...";
                break;
            case AnalysisUIWarnings.LogFileNotUpToDate:
                str = "Log File Not Up to date with current Net...";
                break;
            case AnalysisUIWarnings.PCAFileNotUpToDate:
                str = "PCA File Not Up to date with Log File...";
                break;
            case AnalysisUIWarnings.NoPCAFileFound:
                str = "PCA File Not Found...";
                break; 
        }
        AnalysisUI_WarningsAndInfoData.text  = str;
    }
   
    public void TryCreateNewEncoderDataFile()
    {
        Debug.Log(" TryCreateNewEncoderDataFile() ");
        if (AnalysisUI_LogFileExists)
        {
            Debug.Log(" >File Exists ");
            AnalysisUI_PassWarning(AnalysisUIWarnings.LogFileAreadyExists);
        }
        else
        {
            Debug.Log(" >File Does Not Exist ");
            Debug.Log(" ModelID = " + SelectedModelID);

            Debug.Log(" NetID = " + SelectedNetworkID);

            int NumberOfSets = DataManager.DM.GetModelByID(SelectedModelID).GetNumberOfTrainingSets();
            float[][] TempLogFileData = new float[NumberOfSets][];

            Debug.Log(" NumberOfSets = " + NumberOfSets);

            TrainingModel TM = DataManager.DM.GetModelByID(SelectedModelID);
            NeuralNetMainModule NNM = TM.GetNetworkByID(SelectedNetworkID);

            Debug.Log( "Number Of Inputs Per Set" + TM.GetTrainingSetByID(0).GetInputs().Length);
            Debug.Log("Number Of Inputs In Net" + NNM.GetNumberOfNeuronsAtLayer(0));

            for (int i = 0; i < NumberOfSets; i++)
            {
                NNM.RunEncoderLayers(TM.GetTrainingSetByID(i).GetInputs(), out TempLogFileData[i]);
            }

            string path = ExcelManager.EM.CreateFile(ExcelManager.FileType.EncoderData_RawData, SelectedModel, SelectedNetworkID);

         
            string[] StartingLines = new string[3];
            StreamReader SR = new StreamReader(path);
            StartingLines[0] = SR.ReadLine(); Debug.Log(StartingLines[0]);
            StartingLines[1] = SR.ReadLine(); Debug.Log(StartingLines[1]);
            StartingLines[2] = SR.ReadLine(); Debug.Log(StartingLines[2]);
            SR.Close();
           
           
            StreamWriter SW = new StreamWriter(path);
            string[] LineSplit = null;
            string Line = null;
            for (int i = 0; i < 3; i++)
            {
                LineSplit = StartingLines[i].Split(';');
                Line = null;

                for (int j = 0; j < LineSplit.Length; j++)
                {
                    Line += LineSplit[j] + ";";
                }

                SW.WriteLine(Line);
            }
     
            for (int i = 0; i < NumberOfSets; i++)
            {
                 

                Line = i.ToString() + ";";

                for (int j = 0; j < TempLogFileData[i].Length; j++)
                {
                    Line += TempLogFileData[i][j].ToString() + ";";
                }
                SW.WriteLine(Line);
            }
            SW.Close();      
        }
    }

    public void TryCreateNewPCADataFile()
    {
        Debug.Log("TryCreateNewPCADataFile()");
        if(!AnalysisUI_PCAFileExists)
        {
           if(AnalysisUI_LogFileExists)
           {

     
              Debug.Log(" >Log File Exists ");
              AnalysisUI_PassWarning(AnalysisUIWarnings.PCAFileAreadyExists);
              if(ExcelManager.EM.TryGetNumericalDataFromFile(LogFileAdress,3,out float[,] data))
           {
                float[,] CovarianceMatrix =
                MathFunctionClass.CalculateCovarianceMatrix(data);
                    string s = null;
                    for (int x = 0; x < CovarianceMatrix.GetLength(0); x++)
                    {
                        s = null;
                        for (int y = 0; y < CovarianceMatrix.GetLength(1); y++)
                        {
                            s += CovarianceMatrix[y, x] + ";";
                        }
                        Debug.Log(s);
                    }
                  

                  
                float[,] EigenVectorMatrix = MathFunctionClass.CalculateAproxEigenVectors(CovarianceMatrix,30);

                    //Debug.Log(EigenVectorMatrix.GetLength(0));

                    float[,] ResultingData = MathFunctionClass.CalculateMatrixProduct<float[,]>(data, EigenVectorMatrix);

                    //Debug.Log(ResultingData.GetLength(0));

                    //Debug.Log(ResultingData.GetLength(1));


                    string path =  ExcelManager.EM.CreateFile(ExcelManager.FileType.EncoderData_PCA, SelectedModel, SelectedNetworkID);
               StreamReader SR = new StreamReader(path);
                string[] MetaDataLines= new string[3];
                MetaDataLines[0] = SR.ReadLine();
                MetaDataLines[1] = SR.ReadLine();
                MetaDataLines[2] = SR.ReadLine();
                  
                SR.Close();

                StreamWriter SW  = new StreamWriter(path);

                SW.WriteLine(MetaDataLines[0]);
                SW.WriteLine(MetaDataLines[1]);
                SW.WriteLine(MetaDataLines[2]);

                string Data = null;

                for (int i = 0; i < ResultingData.GetLength(1); i++)
                {
                    for (int j = 0; j < ResultingData.GetLength(0); j++)
                    {
                        Data += ResultingData[j, i] + ";";
                    }
                    SW.WriteLine(Data);
                    Data = null;
                }
                
                SW.Close();
              }
              else
           {
              Debug.Log(" Error Readin g Log File ");
           }


           }
           else
           {
                Debug.Log(" >Log File Does Not Exist ");
            }
        }
        else
        {
            Debug.Log(" >File Already Exists ");
        }
    }

    private bool TryBuildGraph()
    {
        bool value = false;



        return value;
    }

    private void DestroyGraph()
    {
            
    }

    public void CalculateClusterValues()
    {
    
    }
}

 