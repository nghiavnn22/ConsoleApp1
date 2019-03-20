using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    
    class allivet_com
    {
        public string keyword = "dog";
        public string niche = "DOG";
        Dictionary<int, string> dictionaryCate = new Dictionary<int, string> {
            { 8,"Dog Calming"},
            { 9,"Dog Dental"},
            { 20,"Dog Dewormers"},
            { 10,"Dog Diet Food"},
            { 11,"Dog Digestive"},
             { 12,"Dog Ear Care"},
            { 13,"Dog Eye Care"},
             { 7,"Dog Flea & Tick"},
            { 14,"Dog Heartworm"},
            { 15,"Dog Joints Care"},
            { 556,"Dog Mobility"},
            { 16,"Dog Prescriptions"},
            { 108,"Dog Skin & Allergy"},
            { 18,"Dog Supplements"},
            { 19,"Dog Vaccines"},
        };
        public Dictionary<int,string> WebContent;
       
        private Dictionary<int,string> getNiche(string niche)
        {
            Dictionary<int, string> cate = null;
            switch (niche)
            {
                case "DOG":
                    cate= getlistcate();
                    
                    break;
                default:
                    cate = null;
                    break;
            }
            return cate;
        }
        public Dictionary<int,string> getlistcate()
        {           
            return dictionaryCate;
        }
        public void getListProducts()
        {
            DownloadContentPage();
            ExtractProductsInfor();
        }
        public async void DownloadContentPage()
        {
            WebContent = new Dictionary<int, string>();
            Dictionary<int,string> listcate = getNiche(niche);
            foreach (var c in listcate)
            {
                var link = "https://www.allivet.com/search.aspx?SearchTerm="+keyword+"&ctid="+c.Key;
                string sContent = download(link);
                WebContent.Add(c.Key,sContent);
            }
            
        }
        private List<Product> ExtractProductsInfor()
        {
            List<Product> listProduct = new List<Product>();


            foreach (var cate in WebContent)
            {
                string cateOProdcutName = dictionaryCate[cate.Key];
                
                if (cate.Value==null)
                {
                    continue;
                }
                // get collection Product
                MatchCollection productCollection = new Regex(@"class=""item-prod"".*?(class=""item-prod"")", RegexOptions.Singleline|RegexOptions.IgnoreCase).Matches(cate.Value);
                if (productCollection.Count<-1)
                {
                    continue;
                }
                for (int i = 0; i < productCollection.Count; i++)
                {
                    if (!productCollection[i].Value.ToString().Contains(@"$"))
                    {
                        continue;
                    }
                    Product oProduct = new Product();
                    oProduct = getProduct(productCollection[i].Value.ToString());
                    oProduct.Category = cateOProdcutName;
                    if (oProduct==null|| oProduct.Price==0)
                    {
                        continue;
                    }
                    listProduct.Add(oProduct);
                }
            }
            return listProduct;


        }
        private Product getProduct(string sProduct)
        {
            Product oProduct = new Product();
            Regex rxDetail = new Regex(@"href=""(.*?)"".*?src=""(.*?)"".*?strong>(.*?)<\/b>.*?class=""value.*?([\d.,]+)", RegexOptions.Singleline |  RegexOptions.IgnoreCase);
            //Regex rxDetail = new Regex(@"product_cat-([\w-]+).*?href=""(.*?)"".*?src=""(.*?)"".*?alt=""(.*?)"".*?amount"">([\d.]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Match mDetail = rxDetail.Match(sProduct);
            oProduct.SiteId = "ALLVET";
            oProduct.Name = mDetail.Groups[3].Value.Trim().Replace("<b>","");
            //oProduct.Category = categoryName;
            
            //oProduct.Category = (mDetail.Groups[1].Value.Trim() == "") ? "" : mDetail.Groups[1].Value.Trim().Replace("-", " ");
            oProduct.Brand = "";
            //oProduct.Price = 0;
            //if (Utility.IsNumber(mDetail.Groups[5].Value.Trim()) == true)
            if (mDetail.Groups[4].Value.ToString() == "")
            {
                oProduct.Price = 0;
            }
            else
            oProduct.Price = double.Parse(mDetail.Groups[4].Value.ToString());
            
            oProduct.Quantity = 0;
            oProduct.Image = mDetail.Groups[2].Value;
            oProduct.Url = mDetail.Groups[1].Value;
            oProduct.IsActive = true;
            //// change price
            //oProduct.UsdPrice = Utility.Exchange(oProduct.Price, this.Currency);
            return oProduct;
        }
        public string download(string url)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string data = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode==HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.CharacterSet==null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }
                data = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
            }
            return data;
        }
    }
   
}
