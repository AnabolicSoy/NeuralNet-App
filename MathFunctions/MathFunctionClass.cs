using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;
using static StructClass;
using static UnityEngine.Networking.UnityWebRequest;

public static class MathFunctionClass
{
    public static bool PolinomialRoots(float[] PolVal, float BVal, out float[] Roots)
    {

        Roots = new float[PolVal.Length];

        float Increment = 1f;

        int BatchCount = 5;

        float BatchSize = Increment * 100;

        float HighestCoef = 0;
        for (int i = 0; i < PolVal.Length; i++)
        {
            if ((PolVal[i] > 0 ? PolVal[i] : PolVal[i] * -1) > HighestCoef)
            {
                HighestCoef = PolVal[i];
                if (HighestCoef < 0)
                {
                    HighestCoef *= -1;
                }
            }
        }

        //---Find Min Max Inflection Points---//
        int Samples = 100;

        float SampleInclrement = HighestCoef / 10;

        float MaxInflection = 1f;
        float MinInflection = -1f;

        float PosXSlopeSign = 1; int PosXConsecutiveSlopeCount = 0;
        float NegXSlopeSign = 1; int NegXConsecutiveSlopeCount = 0;

        float ComputeP;
        float ComputeN;

        for (int i = 0; i < Samples; i++)
        {
            ComputeP = Mathf.Sign(PolyVal(PolVal, BVal, SampleInclrement * i));
            ComputeN = Mathf.Sign(PolyVal(PolVal, BVal, -SampleInclrement * i));

            if (ComputeP == PosXSlopeSign)
            {
                PosXConsecutiveSlopeCount++;
            }
            else
            {
                PosXSlopeSign = ComputeP;
                PosXConsecutiveSlopeCount = 0;
            }

            if (ComputeN == NegXSlopeSign)
            {
                NegXConsecutiveSlopeCount++;
            }
            else
            {
                NegXSlopeSign = ComputeN;
                NegXConsecutiveSlopeCount = 0;
            }

        }

        if (PosXConsecutiveSlopeCount > 20)
        {

        }
        if (NegXConsecutiveSlopeCount > 20)
        {

        }

        // ---------------------------------- //

        for (int i = 0; i < BatchCount; i++)
        {




        }

        return false;
    }

    private static float PolyVal(float[] Poly, float BVal, float x)
    {
        float value = (Poly[0] + x);

        if (Poly.Length < 12)
        {

            switch (Poly.Length)
            {
                case 0:
                    return BVal;
                case 1:
                    value = Poly[0] + x;
                    break;
                case 2:
                    value = (Poly[0] + x) * (Poly[1] + x);
                    break;
                case 3:
                    value = (Poly[0] + x) * (Poly[1] + x) * (Poly[2] + x);
                    break;
                case 4:
                    value = (Poly[0] + x) * (Poly[1] + x) * (Poly[2] + x) * (Poly[3] + x);
                    break;
                case 5:
                    value = (Poly[0] + x) * (Poly[1] + x) * (Poly[2] + x) * (Poly[3] + x);
                    break;
                case 6:
                    value = (Poly[0] + x) * (Poly[1] + x) * (Poly[2] + x) * (Poly[3] + x) * (Poly[4] + x);
                    break;
                case 7:
                    value = (Poly[0] + x) * (Poly[1] + x) * (Poly[2] + x) * (Poly[3] + x) * (Poly[4] + x) * (Poly[5] + x);
                    break;
                case 8:
                    value = (Poly[0] + x) * (Poly[1] + x) * (Poly[2] + x) * (Poly[3] + x) * (Poly[4] + x) * (Poly[5] + x) * (Poly[6] + x);
                    break;
                case 9:
                    value = (Poly[0] + x) * (Poly[1] + x) * (Poly[2] + x) * (Poly[3] + x) * (Poly[4] + x) * (Poly[5] + x) * (Poly[6] + x) * (Poly[7] + x);
                    break;
                case 10:
                    value = (Poly[0] + x) * (Poly[1] + x) * (Poly[2] + x) * (Poly[3] + x) * (Poly[4] + x) * (Poly[5] + x) * (Poly[6] + x) * (Poly[7] + x) * (Poly[8] + x);
                    break;
                case 11:
                    value = (Poly[0] + x) * (Poly[1] + x) * (Poly[2] + x) * (Poly[3] + x) * (Poly[4] + x) * (Poly[5] + x) * (Poly[6] + x) * (Poly[7] + x) * (Poly[8] + x) * (Poly[9] + x);
                    break;
                case 12:
                    value = (Poly[0] + x) * (Poly[1] + x) * (Poly[2] + x) * (Poly[3] + x) * (Poly[4] + x) * (Poly[5] + x) * (Poly[6] + x) * (Poly[7] + x) * (Poly[8] + x) * (Poly[9] + x) * (Poly[10] + x);
                    break;
            }
        }
        else
        {
            for (int i = 1; i < Poly.Length; i++)
            {
                value *= (Poly[i] + x);
            }
        }

        value += x;

        return value;
    }
    public static float[,] CalculateCovarianceMatrix(float[,] Inputs)
    {
        Debug.Log("---------Calculating Covariance Matrix----------");
        int CovMatSize = Inputs.GetLength(0);

        Debug.Log("CovMatSize" + CovMatSize);

        int L1 = Inputs.GetLength(1);

        Debug.Log("L1" + L1);

        float[,] CovarianceMatrix = new float[CovMatSize, CovMatSize];

        float[] MedianValuesArray = new float[CovMatSize];
        string DebugStr = null;
        for (int i = 0; i < CovMatSize; i++)
        {
            float Sum = 0;

            for (int j = 0; j < L1; j++)
            {
                Sum += Inputs[i, j];
            }
            MedianValuesArray[i] = Sum / L1;
            DebugStr += MedianValuesArray[i] + " | ";
        }

        Debug.Log("MedianValues  = " + DebugStr);

        for (int i = 0; i < CovMatSize; i++)
        {
            for (int j = 0; j < CovMatSize; j++)
            {
                float Sum = 0;

                for (int k = 0; k < L1; k++)
                {
                    Sum = (Inputs[i, k] - MedianValuesArray[i]) * (Inputs[j, k] - MedianValuesArray[j]);
                }
                CovarianceMatrix[j, i] = Sum / L1;

            }
        }

        return CovarianceMatrix;
    }

    public static float CalculateMatrixDeterminant(float[,] Matrix)
    {
        int Matlen = Matrix.GetLength(0);

        float[,] LowerTriangularMatrix = new float[Matrix.Length, Matrix.Length];
        bool[,] LowerTriangularMatrixBool = new bool[Matrix.Length, Matrix.Length];
        float[,] UpperTriangularMatrix = new float[Matrix.Length, Matrix.Length];
        bool[,] UpperTriangularMatrixBool = new bool[Matrix.Length, Matrix.Length];

        bool[,] DebugMat = new bool[Matlen, Matlen];


        for (int i = 0; i < Matrix.GetLength(0); i++)
        {
            LowerTriangularMatrix[i, i] = 1;
            LowerTriangularMatrixBool[i, i] = true;

            UpperTriangularMatrix[i, 0] = Matrix[i, 0];
            UpperTriangularMatrixBool[i, 0] = true;

            LowerTriangularMatrix[0, i] = Matrix[0, i] / Matrix[0, 0];
            LowerTriangularMatrixBool[0, i] = true;
        }
        string s = null;
        Debug.Log("------------Lower Triangular Matrix----------");
        for (int i = 0; i < Matlen; i++)
        {
            for (int j = 0; j < Matlen; j++)
            {
                s += " , " + Math.Round(LowerTriangularMatrix[j, i], 2);
            }
            Debug.Log(s);
            s = null;
        }
        s = null; Debug.Log("------------Upper Triangular Matrix----------");
        for (int i = 0; i < Matlen; i++)
        {
            for (int j = 0; j < Matlen; j++)
            {
                s += " , " + Math.Round(UpperTriangularMatrix[j, i], 2);
            }
            Debug.Log(s);
            s = null;
        }
        s = null;

        Debug.Log("V[0,2] = " + UpperTriangularMatrix[0, 2]);
        Debug.Log("V[2,0] = " + UpperTriangularMatrix[2, 0]);

        Debug.Log("U[2,0] = " + LowerTriangularMatrix[2, 0]);
        Debug.Log("U[0,2] = " + LowerTriangularMatrix[0, 2]);

        //Compute Start

        bool IteratingRow = true;
        int Row = 1;
        int Col = 1;

        Reset:;
        if (IteratingRow)
        {
            Debug.Log(" Iterating Row " + Row);
            for (int SlowIterator = Col; SlowIterator < Matlen; SlowIterator++)
            {
                //Debug.Log("Col " + SlowIterator);
                float Sum = 0;

                for (int FastIterator = 0; FastIterator < Col; FastIterator++)
                {
                    Sum += LowerTriangularMatrix[FastIterator, Row] * UpperTriangularMatrix[SlowIterator, FastIterator];
                    //Debug.Log("Sum += U[" + FastIterator + "," + Row + "]* V[" + SlowIterator + "," + FastIterator + "] (" + Sum + ")");
                    Debug.Log("U[" + FastIterator + "," + Row + "] " + "V[" + SlowIterator + "," + FastIterator + "]");
                }
                UpperTriangularMatrix[SlowIterator, Row] = (Matrix[SlowIterator, Row] - Sum) / LowerTriangularMatrix[Row, Row];
                Debug.Log("V[" + SlowIterator + "," + Row + "] = " + "( A[" + Row + "," + SlowIterator + "] - " + Sum + " ) / " + "U[" + Row + "," + Row + "] {" + LowerTriangularMatrix[Col, Row] + "}");
                DebugMat[Row, SlowIterator] = true;
            }

            Debug.Log("-----------------------");
            string str = null;
            for (int i = 0; i < Matlen; i++)
            {
                for (int j = 0; j < Matlen; j++)
                {
                    str += Math.Round(UpperTriangularMatrix[j, i], 2) + ",";
                }
                Debug.Log(str); str = null;
            }
            Debug.Log("-----------------------");

        }
        else
        {
            Debug.Log("IteratingCol " + Col);
            for (int i = Row; i < Matlen; i++)
            {
                // Debug.Log("Row " + i);

                float Sum = 0;

                for (int j = 0; j < Row; j++)
                {
                    Sum += LowerTriangularMatrix[j, i] * UpperTriangularMatrix[Col, j];
                    //Debug.Log("Sum += U[" + j + "," + i + "] * V[" + Row + "," + j + "]  (" + Sum + ")");
                    Debug.Log("U[" + j + "," + i + "] " + "V[" + Col + "," + j + "]");
                }

                LowerTriangularMatrix[Col, i] = (Matrix[Col, i] - Sum) / UpperTriangularMatrix[Col, Col];
                Debug.Log("U[" + Col + "," + i + "] = " + "( A[" + Col + "," + i + "] - " + Sum + " ) / " + "V[" + Col + "," + Row + "] {" + UpperTriangularMatrix[Col, Col] + "}");

                DebugMat[i, Col] = true;

            }

            Debug.Log("-----------------------");
            string str = null;
            for (int i = 0; i < Matlen; i++)
            {
                for (int j = 0; j < Matlen; j++)
                {
                    str += Math.Round(LowerTriangularMatrix[j, i], 2) + ",";
                }
                Debug.Log(str); str = null;
            }
            Debug.Log("-----------------------");
        }

        if (Row >= Matlen - 1 && Col >= Matlen - 1)
        {
            //Cycle Over print Result
            s = null;
            Debug.Log("------------Result Upper Triangular Matrix----------");
            for (int i = 0; i < Matlen; i++)
            {
                for (int j = 0; j < Matlen; j++)
                {
                    s += " , " + Math.Round(UpperTriangularMatrix[j, i], 2);
                }
                Debug.Log(s);
                s = null;
            }
            s = null;
            Debug.Log("------------Result Lower Triangular Matrix----------");
            for (int i = 0; i < Matlen; i++)
            {
                for (int j = 0; j < Matlen; j++)
                {
                    s += " , " + Math.Round(LowerTriangularMatrix[j, i], 2);
                }
                Debug.Log(s); s = null;
            }

            Debug.Log("------------ matrix A  ----------");
            for (int i = 0; i < Matlen; i++)
            {
                for (int j = 0; j < Matlen; j++)
                {
                    s += " , " + Math.Round(Matrix[j, i], 2);
                }
                Debug.Log(s); s = null;
            }

            Debug.Log("------------ UºV ----------");
            for (int i = 0; i < Matlen; i++)
            {
                for (int j = 0; j < Matlen; j++)
                {

                    float res = 0;

                    for (int k = 0; k < Matlen; k++)
                    {
                        res += LowerTriangularMatrix[k, i] * UpperTriangularMatrix[j, k];
                    }
                    //Math.Round(res, 2)
                    s += " , " + res;
                }
                Debug.Log(s); s = null;
            }



        }
        else
        {
            if (IteratingRow)
            {
                Row++;
            }
            else
            {
                Col++;
            }
            IteratingRow = !IteratingRow;
            goto Reset;
        }

        //Compute End

        float DeterminantUpper = UpperTriangularMatrix[0, 0];
        float DeterminantLower = LowerTriangularMatrix[0, 0];

        for (int i = 1; i < Matlen; i++)
        {
            DeterminantUpper *= UpperTriangularMatrix[i, i];

            DeterminantLower *= LowerTriangularMatrix[i, i];
        }

        return DeterminantUpper * DeterminantLower;
    }

    public static float[,] CalculateAproxEigenVectors(float[,] Matrix,int NumberOfIterations)
    {
        int Matlen = Matrix.GetLength(0); //number of dimentional axis

        Debug.Log("Matlen = " + Matlen);

        StructClass.Vec[] TestVectors = new StructClass.Vec[ (int)Mathf.Pow(2,Matlen)]; 
        
        Debug.Log("TestVectors Count = " + TestVectors.Length);

        int[] CycleRef = new int[Matlen];
        int[] Counter = new int[Matlen];
        int[] SwitchVector = new int[Matlen];

        for (int i = 0; i < Matlen; i++)
        {
            CycleRef[i] = (int)Mathf.Pow(2, i);
            Debug.Log("CycleRef[" + i + "] = " + CycleRef[i]);
            Debug.Log(CycleRef[i]);
            Counter[i] = 0;
            SwitchVector[i] = 1;
        }

        CycleRef[0] = 2;
        Counter[1] = 1;

        //Settting Up Vector Space
        string s = null;
        for (int i = 0; i < TestVectors.Length; i++)
        {
   
            float[] VecCoord = new float[Matlen];
            for (int j = 0; j < Matlen; j++)
            {
              
                if (Counter[j] >= CycleRef[j])
                {
                    SwitchVector[j] *= -1;
                    Counter[j] =0;
                }
                Counter[j]++;
                VecCoord[j] = SwitchVector[j];
                s += VecCoord[j];
            }

            Debug.Log(s);
            s = null;
            TestVectors[i] = new StructClass.Vec(VecCoord);
        }
        for (int i = 0; i < TestVectors.Length; i++)
        {
            string str = "Vector " + i + " [";

            for (int j = 0; j < Matlen; j++)
            {
                TestVectors[i].GetValueI(j, out float v);
                if (j < Matlen - 1)
                {
                    str += v + " | ";

                }
                else
                {
                    str += v;
                }

            }

            str += "]";

            Debug.Log(str);
        }

        // VectorSpace º Matrix
        int tl = TestVectors.Length;
        float[,] VectorValues = new float[Matlen,tl];
        float[] FinalVectorValues = new float[Matlen];

        for (int i = 0; i < tl; i++)
        {
            for (int j = 0; j < Matlen; j++)
            {
                VectorValues[j,i] = TestVectors[i].GetVector()[j];
            }
        }
        for (int k = 0; k < NumberOfIterations; k++)
        {

            Debug.Log("-----------ITERATION " +k+ " -----------");

            for (int i = 0; i < tl; i++)//itterate all vectors in the vector space
            {
                for (int l = 0; l < Matlen; l++)
                {
                    FinalVectorValues[l] = 0;
                }//set finalVector To 0
                for (int j = 0; j < Matlen; j++)
                {
                    for (int l = 0; l < Matlen; l++)
                    {
                        FinalVectorValues[j] += VectorValues[l,i] * Matrix[j,l];
               
                    }
                }//iterate through the vector axis
                float[] a = CalculateNomralizedNVector(FinalVectorValues);
                for (int n = 0; n < a.Length; n++)
                {
                    VectorValues[n,i] = a[n];
                }
                //TestVectors[i] = new StructClass.Vec(a);

               
                    string str1 = null;
                    for (int p = 0; p < Matlen; p++)
                    {
                      str1 += VectorValues[p, i] + " | ";
                    }
                    Debug.Log(str1);          
            }
        }

        Debug.Log("---------------Restults---------------");
        //float ratio = 0;
        //for (int i = 0; i < tl; i++)
        //{
        //    string str = "Vector " + i + " [";
        //    ratio = 1/VectorValues[Matlen-1, i];
        //    for (int j = 0; j < Matlen; j++)
        //    {
                
        //        if (j < Matlen - 1)
        //        {
        //            str += ratio * VectorValues[j,i] + " | ";

        //        }
        //        else
        //        {
        //            str += ratio * VectorValues[j, i];
        //        }

        //    }

        //    str += "]";

        //    Debug.Log(str);
        //}

        float[] EiganVector1 = new float[Matlen];
        float EiganValue1 ;
        float[] EiganVector2 ;
        float EiganValue2 ;

        string str2 = "EigenVector1 [";
        for (int i = 0; i < Matlen; i++)
        {
            EiganVector1[i] = VectorValues[i, 0];
            str2 += EiganVector1[i].ToString() + "|";
        }
        Debug.Log(str2 + "]");

        Debug.Log(" ------------ Calculating Eigen Vector 2 ------------ ");

        EiganVector2 = CalculateRandomOrthogonalVector(EiganVector1, false);
        float mag1 = CalculateVectorMagnitude(EiganVector2);

        float[] prod = EiganVector2; // CalculateMatrixProduct<float[]>(Matrix,EiganVector2);
        float mag2 = CalculateVectorMagnitude(prod);

        PrintVector(prod, true, "EigenVector2 [ ");

        //PrintVector(prod, false, "EiganVector2 º Matrix[");

        //for (int i = 0; i < prod.Length; i++)
        //{
        //    prod[i] /= (mag2/mag1);
        //}

        // PrintVector(prod, false, "Calibrated EiganVector2ºMatrix[ ");

        int iteration = 0;

        for (int i = iteration; i > 0; i--)
        {
           prod = CalculateMatrixProduct<float[]>(Matrix, prod);
           prod = ScaleVector(prod, 1/CalculateVectorMagnitude(prod));
           PrintVector(prod, true, "iteration " + i + " prod = ");
        }


        Debug.Log("Orthogonal Check => ResultVector º InputVector = " + CalculateDotProduct(prod, EiganVector1));


        Debug.Log(" ------------ Calculating Eigen Values ------------ ");

        EiganVector1 = CalculateNomralizedNVector(EiganVector1);
        EiganValue1 = CalculateVectorMagnitude(CalculateMatrixProduct<float[]>(Matrix, EiganVector1));

        Debug.Log("EigenValue1 = " + EiganValue1);

        EiganVector2 = CalculateNomralizedNVector(prod);
        EiganValue2 = CalculateVectorMagnitude(CalculateMatrixProduct<float[]>(Matrix, EiganVector2));

        Debug.Log("EigenValue2 = " + EiganValue2 );

        PrintVector(EiganVector1, true, "EigenVector1 = [");
        PrintVector(EiganVector2, true, "EigenVector2 = [");

        float[,] Result = new float[2, Matrix.GetLength(0)];

        for (int i = 0; i < Matrix.GetLength(0); i++)
        {
            Result[0, i] = EiganVector1[i];
            Result[1, i] = prod[i];
        }


        return Result;
    }
    public static float CalculateVectorMagnitude(float[] vec)
    {
        float res = 0;

        for (int i = 0; i < vec.Length; i++)
        {
            res += vec[i] * vec[i];
        }

        return Mathf.Sqrt(res);
    }
    public static float[] CalculateRandomOrthogonalVector(float[] vector,bool CalibrateToOnes)
    {
        Debug.Log("Calculating Orthogonal Vector");

        float[] result = new float[vector.Length];

        float sum = 0;

        for (int i = 0; i < vector.Length-1; i++)
        {
            //Debug.Log("i = " + i);
            if(CalibrateToOnes)
            {
                result[i] = 1f;
            }
            else
            {
                result[i] = UnityEngine.Random.Range(-5f, 5f);
            }
           
           
            sum += vector[i] * result[i];
            //Debug.Log("Sum = " + sum);
        }

        result[result.Length-1] = -sum / vector[vector.Length-1];

      
        // Debug.Log("R*V = " + result[result.Length - 1] * vector[vector.Length - 1]);

        string s = "ResultVector = [";

        for (int i = 0; i < result.Length; i++)
        {
            s += result[i] + " | ";
        }

        Debug.Log(s + "]");

        Debug.Log("Orthogonal Check => ResultVector º InputVector = " + CalculateDotProduct(result,vector ));


        return result;
    }
    public static float[] CalculateNomralizedNVector(float[] vector)
    {
        float[] result = vector;
        float magnitude = 0;

        for (int i = 0; i < vector.Length; i++)
        {
            magnitude += vector[i] * vector[i];
        }

        magnitude = Mathf.Sqrt(magnitude);

        for (int i = 0; i < vector.Length; i++)
        {
            result[i] = vector[i] / magnitude;
        }
        return result;
    }   

    public static T CalculateMatrixProduct<T>(float[,] M1, float[,] M2)
    {
       // int M1X = M1.GetLength(0);

        int M1Y = M1.GetLength(1);

        int M2X = M2.GetLength(0);

        int M2Y = M2.GetLength(1);

        if (typeof(T) == typeof(float[,]) && M1.GetType() == M2.GetType())
        {
            float[,] result = new float[M2X, M1Y];

            for (int i = 0; i < M1Y; i++)
            {
                for (int j = 0; j < M2X; j++)
                {
                   
                    float res = 0;
                    for (int k = 0; k < M2Y; k++)
                    {
                        res += M1[k,i]* M2[j,k];
                    }
                    result[j, i] = res;
                }
            }

            return (T)Convert.ChangeType(result, typeof(T));
 
        }
        else
        {
            return default;
        }
        
    }
    public static T CalculateMatrixProduct<T>(float[,] M1, float[] M2)
    {
        int matlen = M1.GetLength(0);
        if(M2.Length != M1.GetLength(0))
        {
            Debug.LogError("Error");
        }
        if (M1.GetLength(1) != M1.GetLength(0))
        {
            Debug.LogError("Error");
        }
        if (typeof(T) == typeof(float[]) && M1.GetType() != M2.GetType())
        {
            float[] result = new float[matlen];

            for (int i = 0; i < matlen; i++)
            {
               float res = 0;
               for (int k = 0; k < matlen; k++)
               {
                    
                    res += M1[k, i] * M2[k];
               }
               result[i] = res;
            }

            return (T)Convert.ChangeType(result, typeof(T));
        }
        else
        {
            return default;
        }

    }
    public static T CalculateMatrixProduct<T>(float[] M1, float[] M2)
    {
        int matlen = M1.GetLength(0);
        if (typeof(T) == typeof(float[]) && M1.GetType() != M2.GetType())
        {
            float[] result = new float[matlen];
        
                for (int k = 0; k < matlen; k++)
                {
                  result[k] = M1[k] * M2[k];
                }

            return (T)Convert.ChangeType(result, typeof(T));
        }
        else
        {
            return default;
        }

    }

    public static float CalculateDotProduct(float[] V1, float[] V2)
    {
        float result = 0;

        for (int i = 0; i < V1.Length; i++)
        {
            result += V1[i] * V2[i];
            Debug.Log("Result["+i+"] " + result);
        }

        return result;
    }

    public static float[] ScaleVector(float[] V1, float Scalar) 
    {
        float[] result = new float[V1.Length] ;
        for (int i = 0; i < V1.Length; i++)
        {
           result[i] = V1[i] * Scalar;
        }  
        return result;
    }

    public static float[] CalculateNormalizedVector(float[] V1) 
    {
        float[] result = new float[V1.Length];
        float Mag = CalculateVectorMagnitude(V1);
        for (int i = 0; i < V1.Length; i++)
        {
            V1[i] = V1[i]/Mag;
        }
        return result;
    }

    public static void PrintVector(float[] V1,bool NormalizeForLastValue, string DebugLogComment)
    {
        if(NormalizeForLastValue)
        {
            float Ratio = 1/V1[V1.Length - 1];
            string str4 = "[";

            for (int j = 0; j < V1.Length; j++)
            {
                str4 += V1[j] * Ratio + "|";
            }
            str4 += "]";
            Debug.Log(DebugLogComment + str4);
        }
        else
        {
            string str4 = "[";

            for (int j = 0; j < V1.Length; j++)
            {
                str4 += V1[j] + "|";
            }
            str4 += "]";
            Debug.Log(DebugLogComment + str4);
        }
    }

}
