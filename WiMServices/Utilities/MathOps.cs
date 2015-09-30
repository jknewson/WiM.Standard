using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
                
                throw new Exception("Cannot interpolate");
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
                    throw new Exception("number of columns of matrix a ("+a.GetLength(1)+") must equal to the number of rows in matrix b ("+b.GetLength(0)+").");
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
                throw new Exception("Error multiplying matrix: " +ex.Message);
            }
        }
    }//end class
}//end namespace