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
    class banggod
    {
        public string keyword = "dog";
        public string niche = "DOG";
        public string SiteUrl = "https://www.bonanza.com/";
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
            sUrl = "https://sea.banggood.com/search/{0}/{1}-0-0-1-1-44-0-price-0-0_p-1.html";
            //SearchText
        }
        private List<Product> ExtractProductInfo()
        {
            //DateTime begintime = DateTime.Now;
            List<Product> listProduct = new List<Product>();
            foreach (var cate in listcate)
            {
                // download content
                var sdown = String.Format(sUrl, keyword, cate.Key);
                String sContent = download(sdown);
                // category name
                string cateOProdcutName = HttpUtility.HtmlDecode(cate.Value);
                // no result list product 
                if (!sContent.Contains(@"class=""good_box_min"))
                    continue;

                // get collection Product
                MatchCollection mlistProduct = new Regex(@"data-pid.*?(?=data-pid|free_shipping)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(sContent);
                if (mlistProduct.Count < 1)
                    continue;
                for (int i = 0; i < mlistProduct.Count; i++)
                {
                    if (!mlistProduct[i].Value.ToString().Contains("oriPrice"))
                        continue;
                    Product oProduct = new Product();
                    oProduct = getProduct(mlistProduct[i].Value, listProduct);
                    if (oProduct == null || oProduct.Price == 0)
                        continue;
                    oProduct.Category = cateOProdcutName;
                    listProduct.Add(oProduct);
                }
                //var time = DateTime.Now.Second - begintime.Second;
                //if (listProduct.Count > 15 || time > 10)
                //    break;
            }
            return listProduct;
        }
        private Product getProduct(string sProduct, List<Product> listProduct)
        {
            Product oProduct = new Product();

            Regex rxDetail = new Regex(@"href=""(.*?)"".*?original=""(.*?)"".*?alt=""(.*?)"".*?oriPrice=""([\d.,]+)""", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Match mDetail = rxDetail.Match(sProduct);
            if (!mDetail.Success)
                return null;
            // exits product
            if (listProduct.Where(p => p.Name == HttpUtility.HtmlDecode(mDetail.Groups[2].Value)).ToList().Count > 0)
                return null;
            //oProduct.SiteId = this.SiteID;
            oProduct.Name = HttpUtility.HtmlDecode(mDetail.Groups[3].Value.Trim());
            oProduct.Brand = "";
            oProduct.Price = double.Parse(mDetail.Groups[4].Value.ToString());
            oProduct.Quantity = 0;
            oProduct.Image = HttpUtility.HtmlDecode(mDetail.Groups[2].Value);
            oProduct.Url =  HttpUtility.HtmlDecode(mDetail.Groups[1].Value.Split('?')[0]);
            oProduct.IsActive = true;
            //change price
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
                 { 2226,"Dog Feeding & Watering Supplies"},
                { 2222,"Dog Clothes & Shoes"},
                { 2225,"Dog Kennels,Beds & Houses"},
                { 2223,"Dog Collars, Harnesses & Leashes"},
                { 2224,"Dog Training & Security"},
                { 3330,"Dog Carriers"},
                { 2228,"Dog Toys"},
                { 3346,"Crates & Gates"},
                 { 4493,"Dog Tags & Charms"},
                { 3739,"Dog Health Supplies"},
                
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
