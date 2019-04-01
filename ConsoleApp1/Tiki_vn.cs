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
    class Tiki_vn
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
                    cate = "c5456";
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
            string link = "https://tiki.vn/thuc-an-cho-cho/" + getNiche(niche) + "?q=" + keyword;
            string sContent = downloadJson(link);
            WebContent = sContent;
        }
        private List<Product> ExtractProductsInfo()
        {
            if (WebContent == "")
                return null;
            Match mResult = new Regex(@"class=""product-box-list.*?category-bottom-banner-wrap", RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(WebContent);
            if (!mResult.Success)
                return null;
            List<Product> listProducts = new List<Product>();
            // get list product
            MatchCollection mlistProduct = new Regex(@"data-seller-product-id=.*?(?=data-seller-product-id=|sale-tag sale-tag-square)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(WebContent);
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
            oProduct.Name =HttpUtility.HtmlDecode(mDetail.Groups[1].Value);
            
            oProduct.Category = HttpUtility.HtmlDecode(mDetail.Groups[4].Value);
            oProduct.Brand = mDetail.Groups[3].Value;
            //oProduct.Price = 0;
            //if (Utility.IsNumber(mDetail.Groups[5].Value.Trim()) == true)
            oProduct.Price = double.Parse(mDetail.Groups[2].Value.ToString());
            oProduct.Quantity = 0;
            oProduct.Image = mDetail.Groups[6].Value;
            oProduct.Url =  mDetail.Groups[5].Value;
            oProduct.IsActive = true;
            //// change price
            //oProduct.UsdPrice = Utility.Exchange(oProduct.Price, this.Currency);
            return oProduct;
        }
        public string downloadJson(string link)
        {
            string s = "";
            using (WebClient wc = new WebClient())
            {
                 s = wc.DownloadString(link);
            }
            return s;
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

