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
            string expression = "";
            Dictionary<string, double?> variables = null;
            ExpressionOps eOps = null;
           
            //IA (5564)
            expression = "1-(exp(-3.99+1.73*logN(DRNAREA,10)+8.21*BFI)/(1+exp(-3.99+1.73*logN(DRNAREA,10)+8.21*BFI)))";
            variables = new Dictionary<string, double?>() { { "DRNAREA", 891 }, { "BFI", 0.532 } };
            eOps = new ExpressionOps(expression, variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == 0.00414785102067905);


            expression = "1-(exp(-32.7+23.7*DRNAREA^0.05+8.61*BFI)/(1+exp(-32.7+23.7*DRNAREA^0.05+8.61*BFI)))";
            variables = new Dictionary<string, double?>() { { "DRNAREA", 0.85 }, { "BFI", 0.531059 } };
            eOps = new ExpressionOps(expression, variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == 0.990237401384276);

            //GA
            expression = "(round(PCTREG1+PCTREG2+PCTREG3+PCTREG4+PCTREG5,0)=100)*10^(0.0220*PCTREG1+0.0204*PCTREG2+0.0141*PCTREG3+0.0178*PCTREG4+0.0196*PCTREG5)*DRNAREA^(0.649+0.00130*PCTREG2+0.00109*PCTREG3)";
            variables = new Dictionary<string, double?>() { { "DRNAREA", 6.69 }, { "PCTREG1", 100 }, { "PCTREG2", 0 }, { "PCTREG3", 0 }, { "PCTREG4", 0 }, { "PCTREG5", 0 } };
            eOps = new ExpressionOps(expression, variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == 544.128609363891);

            //NY
            expression = "0.037* (DRNAREA)^(1.029)* (SLOPERATIO)^(0.317)* (STORAGE+0.5)^(-0.104)* (MAR)^(2.308)";
            variables = new Dictionary<string, double?>() { { "DRNAREA", 3.55 }, { "SLOPERATIO", 0.21 }, { "STORAGE", 0.55 }, { "MAR", 19.4 } };
            eOps = new ExpressionOps(expression, variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == 77.5484110086448);

            //RO PeakFlow
            expression = "10^2.124*DRNAREA^0.870*STRDEN^0.770*STORNHD^(-0.856)";
            variables = new Dictionary<string, double?>() { { "DRNAREA", 0.38 }, { "STRDEN", 2.21 }, { "STORNHD", 7.38 } };
            eOps = new ExpressionOps(expression, variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == 19.0782065546946);

            //CO lowFlow
            expression = "2.75423E-19* (DRNAREA)^(1.17)* (PRECIP)^(1.86)* (ELEV)^(3.56)";
            variables = new Dictionary<string, double?>() { { "DRNAREA", 2.9 }, { "PRECIP", 15.03 }, { "ELEV", 9450 } };
            eOps = new ExpressionOps(expression, variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == 0.0210226193415481);

            expression = "max(round(56.3*(CONTDA/424)^0.85*(CSL10_85fm/8.56)^(-0.08)*(PREG_06_10/15.2)^7.07*(JUNAVPRE/4.06)^(-0.85),3),0)";
            variables = new Dictionary<string, double?>() { { "CONTDA", 3.86100386 }, { "PREG_06_10", 13.77952765 }, { "JUNAVPRE", 3.9370079 }, { "CSL10_85fm", 26.399155225 } };
            eOps = new ExpressionOps(expression,variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == 0.486);
        }
    }
}
