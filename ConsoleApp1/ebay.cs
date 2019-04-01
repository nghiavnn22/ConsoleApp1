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
    class ebay
    {
        public string keyword = "royal";
        public string niche = "DOG";
        public string SiteUrl = "https://www.ebay.com";
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
            sUrl = "https://www.ebay.com/sch/{1}/i.html?_from=R40&_nkw={0}&_ipg=200";
            //SearchText
        }
        private List<Product> ExtractProductInfo()
        {
            DateTime begintime = DateTime.Now;
            List<Product> listProduct = new List<Product>();
            foreach (var cate in listcate)
            {
                // download content
                var sdown = String.Format(sUrl, keyword, cate.Key);
                String sContent = download(sdown);
                // category name
                string cateOProdcutName = HttpUtility.HtmlDecode(cate.Value);
                // no result list product 
                if (!sContent.Contains(@"class=""srp-results"))
                    continue;
                Match mResult = new Regex(@"class=""srp-results\ssrp-list\sclearfix.*?id=""srp-river-results-SEARCH_PAGINATION_MODEL_V2-w0", RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(sContent);

                // get collection Product
                MatchCollection mlistProduct = new Regex(@"id=""srp-river-results-listing\d+"".*?(?=id=""srp-river-results-listing|s-item__location\ss-item__itemLocation)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(mResult.Value);
                if (mlistProduct.Count < 1)
                    continue;
                for (int i = 0; i < mlistProduct.Count; i++)
                {
                    if (!mlistProduct[i].Value.ToString().Contains("s-item__price"))
                        continue;
                    Product oProduct = new Product();
                    oProduct = getProduct(mlistProduct[i].Value, listProduct);
                    if (oProduct == null || oProduct.Price == 0)
                        continue;
                    oProduct.Category = cateOProdcutName;
                    listProduct.Add(oProduct);
                }        
                var time = DateTime.Now.Second - begintime.Second;
                if (listProduct.Count > 10 || time > 15)
                    break;
            }
            return listProduct;
        }
        private Product getProduct(string sProduct, List<Product> listProduct)
        {
            Product oProduct = new Product();

            Regex rxDetail = new Regex(@"href=""(.*?)"".*?alt=""(.*?)"".*?src=.*src=""(.*?)"".*?price.*?([\d.,]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Match mDetail = rxDetail.Match(sProduct);
            if (!mDetail.Success)
                return null;
            // exits product
            if (listProduct.Where(p => p.Name == HttpUtility.HtmlDecode(mDetail.Groups[2].Value)).ToList().Count > 0)
                return null;
            //oProduct.SiteId = this.SiteID;
            oProduct.Name = HttpUtility.HtmlDecode(mDetail.Groups[2].Value.Trim());
            oProduct.Brand = "";
            oProduct.Price = double.Parse(mDetail.Groups[4].Value.ToString());
            oProduct.Quantity = 0;
            oProduct.Image =  HttpUtility.HtmlDecode(mDetail.Groups[3].Value);
            oProduct.Url =  HttpUtility.HtmlDecode(mDetail.Groups[1].Value.Split('?')[0]);
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

        public Dictionary<int, string> getlistcate()
        {
            
            Dictionary<int, string> dictionaryCate = new Dictionary<int, string> {
                { 66780,"Dog Food"},
                { 177789,"Dishes, Feeders & Fountains"},
                { 177791,"Toys"},
                { 77664,"Dog Chews & Treats"},
                { 11286,"Other Dog Supplies"},
                { 134752,"Health Care"},
                { 116381,"Training & Obedience"},
                 { 46299,"Signs & Plaques"},
                { 20744,"Beds"},
                { 177788,"Carriers & Totes"},
                { 116378,"Dog Lover Products"},
                { 63057,"Collars"},
                { 146247,"Leashes & Head Collars"},
                { 52352,"Costumes"},                
                { 20749,"Flea & Tick Remedies"},
                { 177792,"Grooming"},
                { 116392,"Pooper Scoopers & Bags"},
                { 116377,"Blankets"},
                { 117426,"Car Seat Covers"},
                { 177790,"Tags & Charms"},
                { 134793,"Whelping Supplies"},
                { 134755,"Odor & Stain Removal"}
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
