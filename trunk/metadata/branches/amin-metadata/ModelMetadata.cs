using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace Landis.Library.Metadata
{
   





class Program
{
    public static void Start(string[] args)
    {
        var john = new MonthlyLog
        {  
            avgAnnualPPT = Guid.NewGuid(),
            avgJJAtemp = "John",
            avgNEEc = 12.9,
        };

        
        //var tblJohn = john.ToDataTable();
        //var clonedJohn = tblJohn.Rows[0].ToDataObject<Customer>();
    }
}



public class MonthlyLog
{
    [DataField(Unit=FiledUnits.g_C_m_2 ,Desc=" " )]
    public Guid avgAnnualPPT { get; set; }

    [DataField(Unit = FiledUnits.g_N_m2_yr1, Desc = " ")]
    public string avgJJAtemp { get; set; }

    [DataField(Unit = FiledUnits.g_C_m2_yr1, Desc = " ")]
    public double avgNEEc { get; set; }
}


}