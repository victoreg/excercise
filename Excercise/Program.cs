using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excercise
{
    class Program
    {
        static void Main(string[] args)
        {

            JobLogger jobLogger = new JobLogger(true, false, false, true, true, true);
            //no estatico, para obligar a crear la instancia y enviar los valores en el constructor
            jobLogger.LogMessage("Log event Nro. 1", false, true, false);
        }
    }
}
