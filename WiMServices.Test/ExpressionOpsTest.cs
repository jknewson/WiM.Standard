using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiM.Utilities;
using System.Linq;
using System.Collections.Generic;
namespace WiMServices.Test    
{
    [TestClass]
    public class ExpressionOpsTest
    {
        [TestMethod]
        public void MultipleFunctionTests()
        {

            string expression = "max(round(56.3*(CONTDA/424)^0.85*(CSL10_85fm/8.56)^(-0.08)*(PREG_06_10/15.2)^7.07*(JUNAVPRE/4.06)^(-0.85),3),0)";
            Dictionary<string, double?> variables = new Dictionary<string, double?>() { { "CONTDA", 3.86100386 }, { "PREG_06_10", 13.77952765 }, { "JUNAVPRE", 3.9370079 }, { "CSL10_85fm", 26.399155225 } };
            //string expression = "min(max(A+B,C),B+C)";
            //Dictionary<string, double?> variables = new Dictionary<string, double?>() { { "A", 1 }, { "B", 2 }, { "C", 3 } };

            ExpressionOps eOps = new ExpressionOps(expression,variables);

            Assert.IsTrue(eOps.IsValid && eOps.Value == 0.486);
        }
    }
}
