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
    class Ali
    {
        public string keyword = "dog food";
        public string niche = "DOG";
        public string SiteUrl = "https://www.aliexpress.com";
        Dictionary<int, string> listcate;
        string sUrl;
        public List<Product> GetListProduct()
        {

            downloadContentPage();
            return ExtractProductInfo();
        }

        public void downloadContentPage()
        {
            // search keyword 
            //
            listcate = getNiche(niche);
            sUrl = "https://www.aliexpress.com/w/wholesale-dog-food.html?site=glo&SearchText={0}&CatId={1}";
            //SearchText
        }
        private List<Product> ExtractProductInfo()
        {
            DateTime begintime = DateTime.Now;
            List<Product> listProduct = new List<Product>();
            foreach (var cate in listcate)
            {
                // download content
                var sdown = String.Format(sUrl,keyword,cate.Key);
                String sContent = download(sdown);
                // category name
                string cateOProdcutName = HttpUtility.HtmlDecode(cate.Value);
                // no result list product 
                if (!sContent.Contains("hs-below-list-items"))
                    continue;
                Match mResult = new Regex(@"id=""hs-below-list-items.*?id=""alitalks", RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(sContent);

               // get collection Product
                MatchCollection mlistProduct = new Regex(@"picRind\shistory-item.*?(?=picRind\shistory-item|order-num-a)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(mResult.Value);
                if (mlistProduct.Count < 1)
                    continue;
                for (int i = 0; i < mlistProduct.Count ; i++)
                {
                    if (!mlistProduct[i].Value.ToString().Contains("price-m"))
                        continue;
                    Product oProduct = new Product();
                    oProduct = getProduct(mlistProduct[i].Value, listProduct);
                    if (oProduct == null || oProduct.Price == 0)
                        continue;
                    oProduct.Category = cateOProdcutName;
                    listProduct.Add(oProduct);
                }
                var time = DateTime.Now.Second - begintime.Second;
                if (listProduct.Count > 9 || time > 15)
                    break;
            }
            return listProduct;
        }
        private Product getProduct(string sProduct, List<Product> listProduct)
        {
            Product oProduct = new Product();

            Regex rxDetail = new Regex(@"href=""(.*?)"".*?src=""(.*?)"".*?alt=""(.*?)"".*?price.*?([\d,.]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Match mDetail = rxDetail.Match(sProduct);
            if (!mDetail.Success)
                return null;
            // exits product
            if (listProduct.Where(p => p.Name == HttpUtility.HtmlDecode(mDetail.Groups[3].Value)).ToList().Count > 0)
                return null;
            //oProduct.SiteId = this.SiteID;
            oProduct.Name = HttpUtility.HtmlDecode(mDetail.Groups[3].Value.Trim());
            oProduct.Brand = "";
            oProduct.Price = double.Parse(mDetail.Groups[4].Value.ToString());
            oProduct.Quantity = 0;
            oProduct.Image = "https:" + HttpUtility.HtmlDecode(mDetail.Groups[2].Value);
            oProduct.Url = "https:" + HttpUtility.HtmlDecode(mDetail.Groups[1].Value.Split('?')[0]);
            oProduct.IsActive = true;
            // change price
            //oProduct.UsdPrice = Utility.Exchange(oProduct.Price, this.Currency);
            return oProduct;
        }
        private Dictionary<int, string> getNiche(string niche)
        {
            Dictionary<int, string> cate = new Dictionary<int, string>();
            switch (niche)
            {
                case "DOG":
                    cate = getlistcate();
                    break;
                default:
                    cate = null;
                    break;
            }
            return cate;
        }

        public Dictionary<int,string> getlistcate()
        {
            Dictionary<int, string> dictionaryCate = new Dictionary<int, string> {
                { 200002893,"Dog Toys"},
                { 200215847,"Dog Collars & Leads"},
                { 200036008,"Dog Clothing & Shoes"},
                { 200215853,"Dog Training Aids"},
                { 200215848,"Dog Litter & Housebreaking"},
                { 200218056,"Dog Grooming"},
                { 200002889,"Dog Feeding"},     
            };
            return dictionaryCate;
        }

        public string download(string url)
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
