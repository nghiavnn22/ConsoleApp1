using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;

namespace ConsoleApp1
{
    class Amazon
    {
        public string WebContent;
        public List<Product> GetListProducts()
        {
            DownloadContent();
            return ExtractProductsInfo();
        }
        public void DownloadContent()
        {
            WebContent= download("https://www.amazon.com/s?k=smartwach&ref=nb_sb_noss_2");
            
        }
        private List<Product> ExtractProductsInfo()
        {
            if (WebContent == "")
                return null;
            List<Product> listProducts = new List<Product>();
            // get list product
            MatchCollection mlistProduct = new Regex(@"class=""s-include-content-margin\ss-border-bottom.*?(?=class=""s-include-content-margin\ss-border-bottom"")", RegexOptions.Singleline|RegexOptions.IgnoreCase).Matches(WebContent);
            //MatchCollection mlistProduct = new Regex(@"class=""product-small\s*col.*?(?=class=""product-small\s*col|class=""container"")", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(WebContent);
            if (mlistProduct.Count < 1)
                return null;

            for (int i = 0; i < mlistProduct.Count; i++)
            {
                if (!mlistProduct[i].Value.ToString().Contains("price"))
                {
                    continue;
                }
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
            Product product = new Product();
            Regex productRegex = new Regex(@"img\ssrc=""(.*?)"".*?alt=""(.*?)""",RegexOptions.Singleline|RegexOptions.IgnoreCase);
            Match mProduct = productRegex.Match(sProduct);
            product.Image = mProduct.Groups[1].Value.ToString();
            product.Name = mProduct.Groups[2].Value.ToString();
            return product;
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
