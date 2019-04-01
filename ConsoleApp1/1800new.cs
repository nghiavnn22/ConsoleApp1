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
    class _1800new
    {
        public string keyword = "royal";
        public string niche = "DOG";
        public string SiteUrl = "https://www.1800petmeds.com";
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
            sUrl = "https://www.1800petmeds.com{0}&Ntt={1}";
        }
        private List<Product> ExtractProductInfo()
        {
            DateTime begintime = DateTime.Now;
            List<Product> listProduct = new List<Product>();
            foreach (var cate in listcate)
            {
                // download content
                var sdown = String.Format(sUrl, cate.Value.ElementAt(0).Key.ToString(), niche);
                String sContent = download(sdown);
                // category name
                string cateOProdcutName = HttpUtility.HtmlDecode(cate.Value.ElementAt(0).Value);
                // no result list product 
                if (!sContent.Contains("product"))
                    continue;
                // get collection Product
                MatchCollection mlistProduct = new Regex("class=\"product\".*?type=\"checkbox\"", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(sContent);
                if (mlistProduct.Count < 1)
                    continue;
                for (int i = 0; i < mlistProduct.Count - 1; i++)
                {
                    if (!mlistProduct[i].Value.ToString().Contains("sale"))
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

            Regex rxDetail = new Regex(@"href=""(.*?)"".*?alt=""(.*?)"".*?data-yo-src=""(.*?)"".*?""sale"".*?([\d.,]+)<", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Match mDetail = rxDetail.Match(sProduct);
            if (!mDetail.Success)
                return null;
            // exits product
            if (listProduct.Where(p => p.Name == HttpUtility.HtmlDecode(mDetail.Groups[2].Value)).ToList().Count > 0)
                return null;
            //oProduct.SiteId = this.SiteID;
            oProduct.Name = HttpUtility.HtmlDecode(mDetail.Groups[2].Value.Trim());
            oProduct.Brand = "";
            oProduct.Price = 0;
            //if (Utility.IsNumber(mDetail.Groups[4].Value.Trim()) == true)
            oProduct.Price = double.Parse(mDetail.Groups[4].Value.ToString());
            oProduct.Quantity = 0;
            oProduct.Image = HttpUtility.HtmlDecode(mDetail.Groups[3].Value);
            oProduct.Url = SiteUrl + "/" + HttpUtility.HtmlDecode(mDetail.Groups[1].Value);
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
            {10 ,new Dictionary <string, string> { { "/Flea+++Tick-cat10.html?N=3552231434+3248835821","Flea &amp; Tick"} } },
            { 11,new Dictionary <string, string> { { "/Heartworm-cat11.html?N=2844824230+3248835821","Heartworm"} } },
            { 60004,new Dictionary <string, string> { { "/Pain-cat60004.html?N=217832551+3248835821","Pain"} } },
            { 12,new Dictionary <string, string> { { "/Joints-cat12.html?N=3397521782+3248835821","Joints"} } },
            { 14,new Dictionary <string, string> { { "/Medications-cat14.html?N=1404481778+3248835821","Medications"} } },
            { 68,new Dictionary <string, string> { { "/Skin+++Coat-cat68.html?N=2574635383+3248835821","Skin &amp; Coat"} } },
            { 235,new Dictionary <string, string> { { "/Vitamins-cat235.html?N=2882389639+3248835821","Vitamins"} } },
            { 63,new Dictionary <string, string> { { "/Dental-cat63.html?N=2680209219+3248835821","Dental"} } },
            { 65,new Dictionary <string, string> { { "/Ear-cat65.html?N=910489856+3248835821","Ear"} } },
            { 17,new Dictionary <string, string> { { "/Supplies-cat17.html?N=1521442510+3248835821","Supplies"} } },
            { 78,new Dictionary <string, string> { { "/Food-cat78.html?N=1853908073+3248835821","Food"} } },
            { 71,new Dictionary <string, string> { { "/Allergy+Relief-cat71.html?N=687139856+3248835821","Allergy Relief"} } },
            { 268,new Dictionary <string, string> { { "/Arthritis-cat268.html?N=1752440223+3248835821","Arthritis"} } },
            { 66,new Dictionary <string, string> { { "/Eye-cat66.html?N=4182024410+3248835821","Eye"} } },
            { 87,new Dictionary <string, string> { { "/Hormonal+Endocrine-cat87.html?N=1339613010+3248835821","Hormonal Endocrine"} } },
            { 94,new Dictionary <string, string> { { "/Urinary+Tract+++Kidneys-cat94.html?N=914874423+3248835821","Urinary Tract &amp; Kidneys"} } },
            { 74,new Dictionary <string, string> { { "/Wormers-cat74.html?N=2279194308+3248835821","Wormers"} } },
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
