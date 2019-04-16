using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WIM.Utilities;
using System.Collections.Generic;


namespace WIM.Test
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

            expression = "S^(0.159)";
            variables = new Dictionary<string, double?>() { { "S", 0.003 }};
            eOps = new ExpressionOps(expression, variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == 0.397065614767193);

            //CO ()
            expression = "5.01187E-62*(DRNAREA)^(1.23)*(ELEV)^(15.22)";
            variables = new Dictionary<string, double?>() { { "DRNAREA", 6.2 }, { "ELEV", 11129 } };
            eOps = new ExpressionOps(expression, variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == 18.2685493685393);

            //CO LIMIT ()
            expression = "1.94984E-51*(DRNAREA)^(0.89)*(ELEV)^(12.42)";
            variables = new Dictionary<string, double?>() { { "DRNAREA", 22.7 }, { "ELEV", 1459 } };
            eOps = new ExpressionOps(expression, variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == 6.22914892060179E-11);

            //ME LIMIT ()
            expression = "-2.123*DRNAREA";
            variables = new Dictionary<string, double?>() { { "DRNAREA", 2 } };
            eOps = new ExpressionOps(expression, variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == -4.246);

            //NC LIMIT ()
            expression = "(DRNAREA>=1) AND (DRNAREA<3.0) AND (LC06IMP<0.1)";
            variables = new Dictionary<string, double?>() { { "DRNAREA", 2 }, { "LC06IMP", 0.03 } };
            eOps = new ExpressionOps(expression, variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == 1);

            //IL ()
            expression = "22.2* (DRNAREA)^(0.749)*(CSL10_85)^(0.401)* (SOILPERM)^(-0.224)* (1.62*(ILREG3))";
            variables = new Dictionary<string, double?>() { { "DRNAREA", 77.8 }, { "SOILPERM", 1.38 }, { "ILREG3", 1 }, { "CSL10_85", 1 } };
            eOps = new ExpressionOps(expression, variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == 872.72776285817);

            //VT (456)
            expression = "0.145*DRNAREA^0.900*(LC06STOR+1)^(-0.274)*PRECPRIS10^1.569";
            variables = new Dictionary<string, double?>() { { "DRNAREA", 77.8 }, { "LC06STOR", 1.38 }, { "PRECPRIS10", 47 } };
            eOps = new ExpressionOps(expression, variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == 2418.73947406818);

            //OH (5517)
            expression = "DRNAREA*(STREAM_VARG<=0.80)*(0.795 -3.740*STREAM_VARG +6.633*STREAM_VARG^2 -5.234*STREAM_VARG^3 +1.543*STREAM_VARG^4)";
            variables = new Dictionary<string, double?>() { { "DRNAREA", 0.73 }, { "STREAM_VARG", 0.61 } };
            eOps = new ExpressionOps(expression, variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == 0.00537431301989999);

            // MA (3412)
            expression = "e#^(2.8084+ (0.9884*(ln(DRNAREA)))+ (0.0111*(PCTSNDGRV))+ (-0.0233*(FOREST))+ (0.75*(MAREGION)))/(1+e#^(2.8084+ (0.9884*(ln(DRNAREA)))+ (0.0111*(PCTSNDGRV))+ (-0.0233*(FOREST))+ (0.75*(MAREGION))))";
            variables = new Dictionary<string, double?>() { { "DRNAREA", 4.4 }, { "PCTSNDGRV", 30.43 }, { "FOREST", 59.98 }, { "MAREGION", 1 } };
            eOps = new ExpressionOps(expression, variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == 0.981349522299977);

            //IA (5564)
            expression = "1-(exp(-3.99+1.73*logN(DRNAREA,10)+8.21*BFI)/(1+exp(-3.99+1.73*logN(DRNAREA,10)+8.21*BFI)))";
            variables = new Dictionary<string, double?>() { { "DRNAREA", 891 }, { "BFI", 0.532 } };
            eOps = new ExpressionOps(expression, variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == 0.00414785102067905);


            expression = "1-(exp(-32.7+23.7*DRNAREA^0.05+8.61*BFI)/(1+exp(-32.7+23.7*DRNAREA^0.05+8.61*BFI)))";
            variables = new Dictionary<string, double?>() { { "DRNAREA", 0.85 }, { "BFI", 0.531059 } };
            eOps = new ExpressionOps(expression, variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == 0.990237406140594);

            //GA
            expression = "(round(PCTREG1+PCTREG2+PCTREG3+PCTREG4+PCTREG5,0)=100)*10^(0.0220*PCTREG1+0.0204*PCTREG2+0.0141*PCTREG3+0.0178*PCTREG4+0.0196*PCTREG5)*DRNAREA^(0.649+0.00130*PCTREG2+0.00109*PCTREG3)";
            variables = new Dictionary<string, double?>() { { "DRNAREA", 6.69 }, { "PCTREG1", 100 }, { "PCTREG2", 0 }, { "PCTREG3", 0 }, { "PCTREG4", 0 }, { "PCTREG5", 0 } };
            eOps = new ExpressionOps(expression, variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == 544.128596151087);

            //NY
            expression = "0.037* (DRNAREA)^(1.029)* (SLOPERATIO)^(0.317)* (STORAGE+0.5)^(-0.104)* (MAR)^(2.308)";
            variables = new Dictionary<string, double?>() { { "DRNAREA", 3.55 }, { "SLOPERATIO", 0.21 }, { "STORAGE", 0.55 }, { "MAR", 19.4 } };
            eOps = new ExpressionOps(expression, variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == 77.5484142150604);

            //RO PeakFlow
            expression = "10^2.124*DRNAREA^0.870*STRDEN^0.770*STORNHD^(-0.856)";
            variables = new Dictionary<string, double?>() { { "DRNAREA", 0.38 }, { "STRDEN", 2.21 }, { "STORNHD", 7.38 } };
            eOps = new ExpressionOps(expression, variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == 19.0782060102076);

            //CO lowFlow
            expression = "2.75423E-19* (DRNAREA)^(1.17)* (PRECIP)^(1.86)* (ELEV)^(3.56)";
            variables = new Dictionary<string, double?>() { { "DRNAREA", 2.9 }, { "PRECIP", 15.03 }, { "ELEV", 9450 } };
            eOps = new ExpressionOps(expression, variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == 0.0210226182581476);

            expression = "max(round(56.3*(CONTDA/424)^0.85*(CSL10_85fm/8.56)^(-0.08)*(PREG_06_10/15.2)^7.07*(JUNAVPRE/4.06)^(-0.85),3),0)";
            variables = new Dictionary<string, double?>() { { "CONTDA", 3.86100386 }, { "PREG_06_10", 13.77952765 }, { "JUNAVPRE", 3.9370079 }, { "CSL10_85fm", 26.399155225 } };
            eOps = new ExpressionOps(expression, variables);
            Assert.IsTrue(eOps.IsValid && eOps.Value == 0.486);
        }
    }
}
