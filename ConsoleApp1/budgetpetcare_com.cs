using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class budgetpetcare_com
    {
        string keyword = "Advantage 20Multi";
        string niche = "DOG";
        string WebContent = "";
        private string getNiche(string niche)
        {
            string cate = "";
            switch (niche)
            {
                case "DOG":
                    cate = "dog";
                    break;
                default:
                    cate = "";
                    break;
            }
            return cate;
        }
        public List<Product> GetListProducts()
        {
            DownloadContentPage();
            return ExtractProductsInfo();
        }

        public void DownloadContentPage()
        {
            string link = "https://www.budgetpetcare.com/Product_Search.aspx?Search=" + keyword;
            string sContent = download(link);
            WebContent = sContent;
        }
        private List<Product> ExtractProductsInfo()
        {
            
            List<Product> listProducts = new List<Product>();
            // get list product
            MatchCollection mlistProduct = new Regex(@"class=""Home_ProBoxDiv"".*?(?=class=""Home_ProBoxDiv""|$)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(WebContent);
            // MatchCollection mlistProduct = new Regex(@"class=""product-small\s*col.*?(?=class=""product-small\s*col|class=""container"")", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(WebContent);
            if (mlistProduct.Count < 1)
                return null;

            for (int i = 0; i < mlistProduct.Count; i++)
            {
                if (!mlistProduct[i].Value.ToString().Contains("$"))
                    continue;
                Product oProduct = new Product();
                oProduct = getProduct(mlistProduct[i].Value);
                if (oProduct == null||oProduct.Category=="nodog")
                    continue;
                listProducts.Add(oProduct);
            }
            return listProducts;
        }
        private Product getProduct(string sProduct)
        {
            Product oProduct = new Product();
            
            Regex rxDetail = new Regex(@"href=""(.*?)"".*?data-src=""(.*?)"".*?alt=""(.*?)"".*?aspx"">(.*?)<.*?([\d.,]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Match mDetail = rxDetail.Match(sProduct);
            oProduct.SiteId = "budgetpetcare.com";
            oProduct.Name = mDetail.Groups[4].Value;
            string[] partlink = mDetail.Groups[1].Value.Trim().Split('/');
            if (!partlink[1].Contains("dog"))
            {
                oProduct.Category = "nodog";
            }
            else
            {

                oProduct.Category = partlink[1];
            }
            oProduct.Brand = "";
            //oProduct.Price = 0;
            //if (Utility.IsNumber(mDetail.Groups[5].Value.Trim()) == true)
            oProduct.Price = double.Parse(mDetail.Groups[5].Value.ToString());
            oProduct.Quantity = 0;
            oProduct.Image = mDetail.Groups[2].Value;
            oProduct.Url = "https://budgetpetcare.com"+mDetail.Groups[1].Value;
            oProduct.IsActive = true;
            //// change price
            //oProduct.UsdPrice = Utility.Exchange(oProduct.Price, this.Currency);
            return oProduct;
        }
        private string download(string url)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string data = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "useragent";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
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
