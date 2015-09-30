using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiM.Utilities;
using System.Linq;

namespace WiMServices.Test
{
    [TestClass]
    public class MathOpsTest
    {
        [TestMethod]
        public void MatrixMultiplyTest()
        {
            //Matrix math: Row by column operation
            //--   --    --     --   --                         --
            //| 2, 4 | * | 1,  3 | = | (2*1)+(4*5), (2*3)+(4*-7) |
            //| 6, 8 |   | 5, -7 |   | (6*1)+(8*5), (6*3)+(8*-7) |
            //--    --   --     --   --                         --
            double[,] a = new double[,]{{2,4},{6,8}};
            double[,] b = new double[,] {{1,3},{5,-7}};
            double[,] result = new double[,]{{22,-22},{46,-38}};
            double[,] matrix = MathOps.MatrixMultiply(a, b);

            Assert.IsTrue(result.Rank == matrix.Rank &&
                Enumerable.Range(0, result.Rank).All(dimension => result.GetLength(dimension) == matrix.GetLength(dimension)) &&
                result.Cast<double>().SequenceEqual(matrix.Cast<double>()));
        }
    }//end class
}//end namespace
