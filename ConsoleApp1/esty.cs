using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    class esty
    {
        public string keyword = "royal";
        public string niche = "DOG";
        public string SiteUrl = "https://www.etsy.com";
        Dictionary<int, Dictionary<string, string>> listcate;
        string sUrl;
        public List<Product> getListProduct()
        {

            downloadContentPage();
            return ExtractProductInfo();
        }

        public void downloadContentPage()
        {
            // search keyword "treats"
            listcate = getNiche(niche);
            sUrl = "https://www.etsy.com/search/pet-supplies{0}?q={1}";
        }
        private List<Product> ExtractProductInfo()
        {
            DateTime begintime = DateTime.Now;
            List<Product> listProduct = new List<Product>();
            foreach (var cate in listcate)
            {
                // download content
                var sdown = String.Format(sUrl, cate.Value.ElementAt(0).Key.ToString(), keyword);
                String sContent = download(sdown);
                // category name
                string cateOProdcutName = HttpUtility.HtmlDecode(cate.Value.ElementAt(0).Value);
                // no result list product 
                if (!sContent.Contains("reorderable-listing-results"))
                    continue;
                // get collection Product
                MatchCollection mlistProduct = new Regex(@"class=""block-grid-item.*?(?=class=""block-grid-item|v2-listing-card__actions)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(sContent);
                if (mlistProduct.Count < 1)
                    continue;
                for (int i = 0; i < mlistProduct.Count; i++)
                {
                    if (!mlistProduct[i].Value.ToString().Contains("currency-value"))
                        continue;
                    Product oProduct = new Product();
                    oProduct = getProduct(mlistProduct[i].Value, listProduct);
                    if (oProduct == null || oProduct.Price == 0)
                        continue;
                    oProduct.Category = cateOProdcutName;
                    listProduct.Add(oProduct);
                }
                //var time = DateTime.Now.Second - begintime.Second;
                //if (listProduct.Count > 10 || time > 15)
                //    break;
            }
            return listProduct;
        }
        private Product getProduct(string sProduct, List<Product> listProduct)
        {
            Product oProduct = new Product();

            Regex rxDetail = new Regex(@"href=""(.*?)"".*?title=""(.*?)"".*?src=""(.*?)"".*?currency-value.*?([\d.,]+)<", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Match mDetail = rxDetail.Match(sProduct);
            if (!mDetail.Success)
                return null;
            // exits product
            if (listProduct.Where(p => p.Name == HttpUtility.HtmlDecode(mDetail.Groups[2].Value)).ToList().Count > 0)
                return null;
            //oProduct.SiteId = this.SiteID;
            oProduct.Name = HttpUtility.HtmlDecode(mDetail.Groups[2].Value.Trim());
            oProduct.Brand = "";
            //if (Utility.IsNumber(mDetail.Groups[4].Value.Trim()) == true)
            oProduct.Price = double.Parse(mDetail.Groups[4].Value.ToString());
            oProduct.Quantity = 0;
            oProduct.Image = HttpUtility.HtmlDecode(mDetail.Groups[3].Value.Split('?')[0]);
            oProduct.Url = HttpUtility.HtmlDecode(mDetail.Groups[1].Value.Split('?')[0]);
            oProduct.IsActive = true;
            // change price
            //oProduct.UsdPrice = Utility.Exchange(oProduct.Price, this.Currency);
            return oProduct;
        }
    
        private Dictionary<int, Dictionary<string, string>> getNiche(string niche)
        {
            Dictionary<int, Dictionary<string, string>> cate = new Dictionary<int, Dictionary<string, string>>();
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

        public Dictionary<int, Dictionary<string, string>> getlistcate()
        {
            Dictionary<int, Dictionary<string, string>> dictionaryCate = new Dictionary<int, Dictionary<string, string>> {
            {10 ,new Dictionary <string, string> { { "/pet-collars-and-leashes", "Pet Collars & Leashes " } } },
            { 11,new Dictionary <string, string> { { "/pet-clothing-accessories-and-shoes", "Pet Clothing, Accessories & Shoes " } } },
            { 60004,new Dictionary <string, string> { { "/riding-and-farm-animals", "Riding & Farm Animals " } } },
            { 12,new Dictionary <string, string> { { "/pet-storage", "Pet Storage" } } },
            { 14,new Dictionary <string, string> { { "/pet-health-and-wellness", "Pet Health & Wellness " } } },
            { 68,new Dictionary <string, string> { { "/urns-and-memorials", "Urns & Memorials" } } },
            { 235,new Dictionary <string, string> { { "/training", "Training " } } },
            { 63,new Dictionary <string, string> { { "/pet-toys", "Pet Toys " } } },
            { 65,new Dictionary <string, string> { { "/pet-furniture", "Pet Furniture " } } },
            { 17,new Dictionary <string, string> { { "/pet-feeding", "Pet Feeding" } } },
            { 78,new Dictionary <string, string> { { "/pet-bedding", "Pet Bedding " } } },
            { 71,new Dictionary <string, string> { { "/pet-carriers-and-houses", "Pet Carriers & Houses" } } },
            { 268,new Dictionary <string, string> { { "/pet-gates-and-fences", "Pet Gates & Fences" } } },  
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
