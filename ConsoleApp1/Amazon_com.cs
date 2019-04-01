using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    class Amazon_com
    {
        public string keyword = "royal";
        public string niche = "DOG";
        public string webContent = "";
        public string downloadWithWebClient(string url)
        {
            string htmlCode="";
            using(WebClient client =  new WebClient())
            {
                htmlCode = client.DownloadString(url);
            }
            return htmlCode;
        }
        public void DownloadContentPage()
        {
            string link = "https://www.amazon.com/s?k="+keyword+"&i=pets&rh="+getNiche();
            string sContent = download(link);
            webContent = sContent;
        }
        public List<Product> getListProduct()
        {
            DownloadContentPage();
            return ExtractProductInfor();
        }
        public List<Product> ExtractProductInfor()
        {

            List<Product> listProducts = new List<Product>();
            // get list prodcut 2
            Match mResult = new Regex(@"class=""s-result-list.*?s-result-list-placeholder", RegexOptions.Singleline | RegexOptions.IgnoreCase).Match(webContent);
            if (!mResult.Success)
                return null;
            MatchCollection mlistProduct = new Regex(@"data-asin=""\w+"".*?(?=data-asin=""\w+""|$)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(mResult.Value);
            if (mlistProduct.Count < 1)
                return null;
            // get list product
            //MatchCollection mlistProduct = new Regex(@"s-expand-height\ss-include-content-margin\ss-border-bottom.*?(?=s-expand-height\ss-include-content-margin\ss-border-bottom|$)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(webContent);
            //if (mlistProduct.Count < 1)
            //    return null;

            for (int i = 0; i < mlistProduct.Count; i++)
            {
                if (!mlistProduct[i].Value.ToString().Contains("$"))
                    continue;
                Product oProduct = new Product();
                oProduct = getProduct(mlistProduct[i].Value);
                if (oProduct == null)
                    continue;
                listProducts.Add(oProduct);
            }
            return listProducts;
        }
        public Product getProduct(string sProduct)
        {
            Product oProduct = new Product();           
           Regex rx = new Regex(@"href=""(.*?)"".*?src=""(.*?)"".*?alt=""(.*?)"".*?\$([\d.,]+)",RegexOptions.Singleline|RegexOptions.IgnoreCase);
            Match mProduct = rx.Match(sProduct);
            oProduct.SiteId = "AMAZON_COM";
            oProduct.Brand = "";
            oProduct.Quantity = 0;
            oProduct.IsActive = true;
            oProduct.Name = mProduct.Groups[3].Value;
            oProduct.Url = "https://amazon.com" + mProduct.Groups[1].Value.ToString();
            oProduct.Price = double.Parse(mProduct.Groups[4].Value.ToString());
            oProduct.Image = mProduct.Groups[2].Value;
            return oProduct;
        }
        public string getNiche()
        {
            string cate = "";
            switch (niche)
            {
                case "DOG":
                        cate = "n%3A2619533011";
                    break;
                default:
                    cate = "";
                    break;
            }
            return cate;
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
