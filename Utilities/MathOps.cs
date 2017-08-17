using System;

namespace WiM.Utilities
{
    public static class MathOps
    {
        public static Double? LinearInterpolate(Double x1, Double x2, Double y1, Double y2, Double x)
        {
            try
            {
                return (((x2 - x) * y1 + (x - x1) * y2)) / (x2 - x1);
            }
            catch (Exception e)
            {

                throw new Exception("Cannot interpolate", e);
            }

        }
        public static Double[,] MatrixMultiply(Double[,] a, Double[,] b)
        {
            //Matrix math: Row by column operation
            //--   --    --     --   --                         --
            //| 2, 4 | * | 1,  3 | = | (2*1)+(4*5), (2*3)+(4*-7) |
            //| 6, 8 |   | 5, -7 |   | (6*1)+(8*5), (6*3)+(8*-7) |
            //--    --   --     --   --                         --

            try
            {
                //number of columns of the 1st matrix must equal to the number of rows of the 2nd matrix. 
                if (a.GetLength(1) != b.GetLength(0))
                    throw new Exception("number of columns of matrix a (" + a.GetLength(1) + ") must equal to the number of rows in matrix b (" + b.GetLength(0) + ").");
                double[,] resultMatrix = new double[a.GetLength(0), b.GetLength(1)];
                for (int r = 0; r < resultMatrix.GetLength(0); r++)
                {
                    for (int c = 0; c < resultMatrix.GetLength(1); c++)
                    {
                        resultMatrix[r, c] = 0;
                        for (int i = 0; i < a.GetLength(1); i++) // OR k<b.GetLength(0)
                            resultMatrix[r, c] = resultMatrix[r, c] + a[r, i] * b[i, c];
                    }//next c
                }//next r

                return resultMatrix;
            }
            catch (Exception ex)
            {
                throw new Exception("Error multiplying matrix: " + ex.Message);
            }
        }
        public static Double[,] InvertMatrixGJ(Double[,] m)
        {
            // Inverts matrix using Gauss Jordan Elimination method
            //code adapted from: http://www.planet-source-code.com/vb/scripts/ShowCode.asp?txtCodeId=13618&lngWId=3
            Double[,] MIdentity = null;
            try
            {
                MIdentity = GetMatrixIdentiy(m.GetLength(1));
                int l = m.GetLength(0);
                for (int i = 0; i < l; i++)
                {
                    Double temp = m[i, i];
                    if (temp < 0)
                        temp = temp * (-1);
                    int p = i;

                    for (int j = i + 1; j < l; j++)
                    {
                        Double tem;
                        if (m[j, i] < 0)
                            tem = m[j, i] * (-1);
                        else
                            tem = m[j, i];

                        if (temp < 0)
                            temp = temp * (-1);

                        if (tem > temp)
                        {
                            p = j;
                            temp = m[j, i];
                        }//end if 
                    }//next j

                    //row exchange in both the matricies 
                    for (Int32 j = 0; j < l; j++)
                    {
                        Double temp1;
                        Double temp2;

                        temp1 = m[i, j];
                        m[i, j] = m[p, j];
                        m[p, j] = temp1;
                        temp2 = MIdentity[i, j];
                        MIdentity[i, j] = MIdentity[p, j];
                        MIdentity[p, j] = temp2;
                    }// next j 

                    Double temp4 = m[i, i];
                    for (Int32 j = 0; j < l; j++)
                    {
                        m[i, j] = m[i, j] / temp4;
                        MIdentity[i, j] = MIdentity[i, j] / temp4;
                    }//next j 

                    //making other elements 0 in order to make the matrix a[][] 
                    //an identity matrix and obtaining a inverse b[][] matrix 
                    for (Int32 q = 0; q < l; q++)
                    {
                        if (q == i) continue; //next q
                        Double temp5 = m[q, i];
                        for (Int32 j = 0; j < l; j++)
                        {
                            m[q, j] = m[q, j] - (temp5 * m[i, j]);
                            MIdentity[q, j] = MIdentity[q, j] - (temp5 * MIdentity[i, j]);
                        }//next j 
                    }//next q 

                }//next i

                return MIdentity;
            }
            catch (Exception e)
            {

                throw;
            }
        }//end InvertMatrixGS
        public static Double[,] GetMatrixIdentiy(Int32 l)
        {
            double[,] identityMatrix = new double[l, l];
            try
            {
                for (int r = 0; r < l; r++)
                {
                    for (int c = 0; c < l; c++)
                    {
                        if (r == c) identityMatrix[r, c] = 1;
                        else identityMatrix[r, c] = 0;
                    }//next c
                }//next r

                return identityMatrix;
            }
            catch (Exception e)
            {
                throw;
            }//end getMatrixIdentity
        }//end InvertMatrixGJ
        public static T[,] ResizeArray<T>(T[,] original, Int32 newNumberOfRows, Int32 newNumberOfColumns)
        {
            var newArray = new T[newNumberOfRows, newNumberOfColumns];
            int columnCount = original.GetLength(1);
            int columnCount2 = newNumberOfColumns;
            int rows = original.GetUpperBound(0);

            for (int row = 0; row <= rows; row++)
                Array.Copy(original, row * columnCount, newArray, row * columnCount2, columnCount);

            return newArray;

        }//end ResizeArray
    }//end class
}//end namespace
