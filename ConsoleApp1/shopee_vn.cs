using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net.Http;

namespace ConsoleApp1
{
    class shopee_vn
    {
        public string keyword = "royal";
        public string niche = "DOG";
        public string WebContent = "";
        private string getNiche(string niche)
        {
            string cate = "";
            switch (niche)
            {
                case "DOG":
                    cate = "18980";
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
            //https://shopee.vn/api/v2/search_items/?by=relevancy&keyword=royal&limit=50&match_id=18980&newest=0&order=desc&page_type=search
            string link = "https://shopee.vn/api/v2/search_items/?by=relevancy&keyword="+keyword+ "l&limit=50&match_id=" + getNiche(niche)+ "&newest=0&order=desc&page_type=search";
           // string link = "https://shopee.vn/search?category=18977&keyword=" + keyword + getNiche(niche) + "&showItems=true&subcategory=18980";
            string sContent = download("https://shopee.vn/api/v2/search_items/?by=relevancy&keyword=royal%20Ch%C4%83m%20s%C3%B3c%20th%C3%BA%20c%C6%B0ng%20Th%E1%BB%A9c%20%C4%83n%20cho%20ch%C3%B3&limit=50&newest=0&order=desc&page_type=search");
            WebContent = sContent;
        }
        private List<Product> ExtractProductsInfo()
        {
            if (WebContent == "")
                return null;
            Match mResult = new Regex(@"class=""shopee-search-item-result__items.*?", RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(WebContent);
            if (!mResult.Success)
                return null;
            List<Product> listProducts = new List<Product>();
            // get list product
            MatchCollection mlistProduct = new Regex(@"col-xs-2-4\sshopee-search-item-result__item=.*?(?=col-xs-2-4\sshopee-search-item-result__item|_1Ewdcf)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(WebContent);
            // MatchCollection mlistProduct = new Regex(@"class=""product-small\s*col.*?(?=class=""product-small\s*col|class=""container"")", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(WebContent);
            if (mlistProduct.Count < 1)
                return null;

            for (int i = 0; i < mlistProduct.Count; i++)
            {
                if (!mlistProduct[i].Value.ToString().Contains("final-price"))
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
            Regex rxDetail = new Regex(@"data-title=""(.*?)"".*?data-price=""(.*?)"".*?brand=""(.*?)"".*?\sdata-category=""(.*?)"".*?href=""(.*?)"".*?src=""(.*?)""", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Match mDetail = rxDetail.Match(sProduct);
            oProduct.SiteId = "tiki.vn";
            oProduct.Name = mDetail.Groups[1].Value;

            oProduct.Category = mDetail.Groups[4].Value;
            oProduct.Brand = mDetail.Groups[3].Value;
            //oProduct.Price = 0;
            //if (Utility.IsNumber(mDetail.Groups[5].Value.Trim()) == true)
            oProduct.Price = double.Parse(mDetail.Groups[2].Value.ToString());
            oProduct.Quantity = 0;
            oProduct.Image = mDetail.Groups[6].Value;
            oProduct.Url = mDetail.Groups[5].Value;
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
            request.ContentType = "application/json";
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
        public void downloadJson(string link)
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString(link);
            }
        }
        
    }
}
