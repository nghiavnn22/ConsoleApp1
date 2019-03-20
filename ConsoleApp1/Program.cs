using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //PETMART_VN test = new PETMART_VN();
            //test.GetListProducts();
            allivet_com allvet = new allivet_com();

            allvet.getListProducts();
        }
    }
}
