using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Web;

namespace ConsoleApp1
{
    class Aliexpress
    {
        //https://vi.aliexpress.com/wholesale?catId=200002889&SearchText=dog
        public string keyword = "food";
        public string niche = "DOG";
        public string WebContent = "";
        public string SiteUrl = "https://www.aliexpress.com";
        private string getNiche(string niche)
        {
            string cate = "";
            switch (niche)
            {
                case "DOG":
                    cate = "200002889";
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
            string link = "https://vi.aliexpress.com/wholesale?catId=" + getNiche(niche) + "&SearchText=" + keyword;
            string sContent = download(link);
            WebContent = sContent;
        }
        private List<Product> ExtractProductsInfo()
        {
            if (WebContent == "")
                return null;

            Match mResult = new Regex(@"id=""hs-below-list-items.*?id=""alitalks", RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(WebContent);
            if (!mResult.Success)
                return null;
            List<Product> listProducts = new List<Product>();
            // get list product
            MatchCollection mlistProduct = new Regex(@"picRind\shistory-item.*?(?=picRind\shistory-item|order-num-a)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(WebContent);
            // MatchCollection mlistProduct = new Regex(@"class=""product-small\s*col.*?(?=class=""product-small\s*col|class=""container"")", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(WebContent);
            if (mlistProduct.Count < 1)
                return null;

            for (int i = 0; i < mlistProduct.Count; i++)
            {
                if (!mlistProduct[i].Value.ToString().Contains("price-m"))
                    continue;
                Product oProduct = new Product();
                oProduct = getProduct(mlistProduct[i].Value);
                if (oProduct == null)
                    continue;
                listProducts.Add(oProduct);
            }
            return listProducts;
        }
        private Product getProduct(string sProduct)
        {
            Product oProduct = new Product();
            Regex rxDetail = new Regex(@"href=""(.*?)"".*?src=""(.*?)"".*?alt=""(.*?)"".*?price.*?([\d,.]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Match mDetail = rxDetail.Match(sProduct);
            oProduct.SiteId = SiteUrl;
            oProduct.Name =HttpUtility.HtmlDecode(mDetail.Groups[3].Value);
            oProduct.Category = "Dog Feeding";
            //oProduct.Brand = mDetail.Groups[3].Value;
            //oProduct.Price = 0;
            //if (Utility.IsNumber(mDetail.Groups[5].Value.Trim()) == true)
            oProduct.Price = double.Parse(mDetail.Groups[4].Value.ToString());
            oProduct.Quantity = 0;
            oProduct.Image = "https:" + HttpUtility.HtmlDecode(mDetail.Groups[2].Value);
            oProduct.Url = "https:"+HttpUtility.HtmlDecode(mDetail.Groups[1].Value.Split('?')[0]);
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
