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
    class Entirelypets_com
    {

        string keyword = "food";
        string niche = "DOG";
        string WebContent = "";
        
        private string getNiche(string niche)
        {
            string cate = "";
            switch (niche)
            {
                case "DOG":
                    cate = "%3Adogs";
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
            string link = "https://search.entirelypets.com/search?w=" + keyword + "&af=pettype" + getNiche(niche)+ "&cnt=300";
          //  string link = "https://search.entirelypets.com/search?lbc=entirelypets&method=and&p=Q&ts=custom&uid=737278970&w=" + keyword + "&af=" + getNiche(niche) ;
            string sContent = download(link);
            WebContent = sContent;
        }
        private List<Product> ExtractProductsInfo()
        {
            Match mResult = new Regex(@"data-sli-selector=""products-container.*?st_show-per-page-before-word", RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(WebContent);
            if (!mResult.Success)
                return null;
            List<Product> listProducts = new List<Product>();
            // get list product
            MatchCollection mlistProduct = new Regex(@"data-tb-sid=""st_result-container-wrapper"".*?(?=data-tb-sid=""st_result-container-wrapper""|$)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(WebContent);
            // MatchCollection mlistProduct = new Regex(@"class=""product-small\s*col.*?(?=class=""product-small\s*col|class=""container"")", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(WebContent);
            if (mlistProduct.Count < 1)
                return null;

            for (int i = 0; i < mlistProduct.Count; i++)
            {
                if (!mlistProduct[i].Value.ToString().Contains("$"))
                    continue;
                Product oProduct = new Product();
                oProduct = getProduct(mlistProduct[i].Value);
                if (oProduct == null || oProduct.Category == "nodog")
                    continue;
                listProducts.Add(oProduct);
            }
            return listProducts;
        }
        private Product getProduct(string sProduct)
        {
            Product oProduct = new Product();

            Regex rxDetail = new Regex(@"data-direct-url=""(.*?)"".*?src=""(.*?)"".*?title=""(.*?)"".*?sli_price.*?([\d.,]+)<", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Match mDetail = rxDetail.Match(sProduct);
            oProduct.SiteId = "entirelypets.com";
            oProduct.Name = HttpUtility.HtmlDecode(mDetail.Groups[3].Value);
            
            oProduct.Brand = "";
            //oProduct.Price = 0;
            //if (Utility.IsNumber(mDetail.Groups[5].Value.Trim()) == true)
            oProduct.Price = double.Parse(mDetail.Groups[4].Value.ToString());
            oProduct.Quantity = 0;
            oProduct.Image = "https://" + HttpUtility.HtmlDecode(mDetail.Groups[2].Value);
            oProduct.Url = HttpUtility.HtmlDecode(mDetail.Groups[1].Value);
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