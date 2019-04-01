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
    class joybuy
    {
        public string keyword = "dog";
        public string niche = "DOG";
        public string SiteUrl = "https://www.joybuy.com/";
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
            listcate = getNiche(niche);
            //https://www.joybuy.com/search?bucket=-2&keywords=dog%20food&category=875020801
            sUrl = "https://www.joybuy.com/search?keywords={0}&category={1}";
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
                //if (!sContent.Contains(@"goods-item"))
                //    continue;

                // get collection Product
                MatchCollection mlistProduct = new Regex(@"goods-item.*?(?=goods-item|p-star)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(sContent);
                if (mlistProduct.Count < 1)
                    continue;
                for (int i = 0; i < mlistProduct.Count; i++)
                {
                    if (!mlistProduct[i].Value.ToString().Contains("p-price"))
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

            Regex rxDetail = new Regex(@"class=""item_image.*?href=""(.*?)"".*?alt=""(.*?)"".*?src=""(.*?)"".*?\$([\d.,]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
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
            oProduct.Image = HttpUtility.HtmlDecode(mDetail.Groups[3].Value);
            oProduct.Url = SiteUrl + HttpUtility.HtmlDecode(mDetail.Groups[1].Value);
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
                 { 875020805,"Pet Feeding"},
                { 875020810,"Pet Toys"},
                { 875020807,"Pet Health Supplies"},
                { 875020804,"Collars, Harnesses & Leashes"},
                { 875020801,"Pet Clothing & Accessories"},
                { 875061420,"Dishes, Feeders & Fountains"},
                { 875020806,"Pet Grooming"},
                 { 875020811,"Training & Behavioral Aids"},
                { 875020808,"Pet Houses, Cages, Fences & Doors"},
                { 875020802,"Pet Beds & Furniture"},
                { 875020803,"Bags, Luggage & Travel Products"},
                { 875020809,"Pet Memorials"}
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
