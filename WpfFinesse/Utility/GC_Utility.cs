using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfFinesse.Models;

namespace WpfFinesse.Utility
{
    public static class GC_Utility
    {
        public static string CreateComand(string commandType,string commandParameter)
        {
            try
            {
                return commandType +"#"+commandParameter;
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
