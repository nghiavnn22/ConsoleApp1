using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

/// <summary>
/// content prodcut qua phut tap
/// </summary>
namespace ConsoleApp1
{
    class christiesdirect
    {
        public string keyword = "dog food";
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
            //https://www.christiesdirect.com/SearchResults.aspx?Search=s&CategoryIDs=291
            sUrl = "https://www.christiesdirect.com/SearchResults.aspx?Search={0}&CategoryIDs={1}";
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
                if (!sContent.Contains(@"class=""product-data-list-table"))
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
            oProduct.Url = HttpUtility.HtmlDecode(mDetail.Groups[1].Value.Split('?')[0]);
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
            Dictionary<int,string> dictionaryCate = new Dictionary<int, string> { 
            { 291  ,"Dog-Accessories"  },
            { 107, "Dog Bathing Accessories"  },
            { 108, "Dog Baths"  },
            { 202, "Dog Beds"  },
            { 203, "Dog Bowls"  },
            { 115, "Dog Cages and Carriers" },
            { 97, "Dog Clippers" },
            { 118, "Dog Coat Care Products" },
            { 204, "Dog Collars and Leads"  },
            { 226, "Dog Dryers and Blasters Spare Parts"  },
            { 113, "Dog Grooming DVDs" },
            { 134, "Dog Grooming Tables" },
            { 267, "Dog Grooming Tables Spare Parts"  },
            { 127, "Dog Shampoo"  },
            { 205, "Dog Toys" },
            { 274, "Pet Health"  },
            { 280, "Pet Strollers"  },
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












