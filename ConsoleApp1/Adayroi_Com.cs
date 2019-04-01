using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Adayroi_Com
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
                    cate = "A1893";
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
            string link = "https://www.adayroi.com/tim-kiem?q=" + keyword + "%3A%3Arelevance%3AleafCategory%3" + getNiche(niche);
            string sContent = download(link);
            WebContent = sContent;
        }
        private List<Product> ExtractProductsInfo()
        {
            if (WebContent == "")
                return null;
            // limit web content
            Match mResult = new Regex(@"class=""product-list__container"".*?rcm-block", RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(WebContent);
            if (!mResult.Success)
                return null;
            List<Product> listProducts = new List<Product>();
            // get list product
            //group list product content
            MatchCollection mlistProduct = new Regex(@"class=""product-item__container.*?(?=class=""product-item__container|polish-version-icon-giftcode-16px)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(WebContent);
            if (mlistProduct.Count < 1)
                return null;

            for (int i = 0; i < mlistProduct.Count; i++)
            {
                if (!mlistProduct[i].Value.ToString().Contains("product-item__info-price-sale"))
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
            Regex rxDetail = new Regex(@"href=""(.*?)"".*?src=""(.*?)"".*?alt=""(.*?)"".*?info-price-sale.*?([\d.,]+).<", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Match mDetail = rxDetail.Match(sProduct);
            oProduct.SiteId = "adayroi.com";
            oProduct.Name = mDetail.Groups[3].Value;

            oProduct.Price = double.Parse(mDetail.Groups[4].Value.Replace(".","").ToString());
            oProduct.Quantity = 0;
            oProduct.Image =  mDetail.Groups[2].Value;
            oProduct.Url = "https://www.adayroi.com" + mDetail.Groups[1].Value;
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
