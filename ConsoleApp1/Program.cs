using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
       
        static void Main(string[] args)
        {
            // PETMART_VN test = new PETMART_VN();
            //test.GetListProducts();
            //budgetpetcare_com test = new budgetpetcare_com();
            //test.GetListProducts();
            //allivet_com allvet = new allivet_com();

            //allvet.getListProducts();
            //petsmart_com test = new petsmart_com();
            //test.GetListProducts();

            //string slink = @"/dog/collars-harnesses-and-leashes/harnesses/puppia-sports-adjustable-dog-harness-20396.html";
            //int batdau = slink.IndexOf(@"/",2);
            //int ketthuc = slink.IndexOf(@"/",1);
            //string[] parts = slink.Split('/');
            //string category = parts[2].Replace("-", " ");
            //Console.WriteLine(category);

            // CrawlerName test = new CrawlerName();
            //test.getListPersionName();
            //_1800petmeds_com test = new _1800petmeds_com();
            //test.getListProduct();
            //Amazon_com test = new Amazon_com();
            //test.getListProduct();
            //Dog_Com test = new Dog_Com();
            //test.DownloadContentPage();
            //Adayroi_Com test = new Adayroi_Com();
            //test.GetListProducts();
            //Sendo test = new Sendo();
            //test.getListProduct();
            //shopee_vn test = new shopee_vn();
            //test.GetListProducts();
            // ebay test = new ebay();
            //  test.GetListProduct();
            //bonanza test = new bonanza();
            //test.GetListProduct();
            //Dog_Com test = new Dog_Com();
            // test.getListProduct();
            // newchic test = new newchic();
            //test.getListProduct();
            // inspireuplift test = new inspireuplift();
            // test.GetListProducts();
            christiesdirect test = new christiesdirect();
            test.GetListProduct();
        }
    }
}
