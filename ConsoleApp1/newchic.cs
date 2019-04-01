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
    class newchic
    {
        public string keyword = "royal";
        public string niche = "DOG";
        public string SiteUrl = "https://www.etsy.com";
        Dictionary<string, string> listcate;
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
            //https://sea.newchic.com/nc/dog/dogs-toys-c-5595.html
            sUrl = "https://sea.newchic.com/nc/{1}/{0}.html";
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
                if (!sContent.Contains(@"class=""search_results_items_container"))
                    continue;

                // get collection Product
                MatchCollection mlistProduct = new Regex(@"data-item-position=.*?(?=data-item-position=|icons_container)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(sContent);
                if (mlistProduct.Count < 1)
                    continue;
                for (int i = 0; i < mlistProduct.Count; i++)
                {
                    if (!mlistProduct[i].Value.ToString().Contains("item_price"))
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

        private Dictionary<string, string> getNiche(string niche)
        {
            Dictionary<string, string> cate = new Dictionary<string, string>();
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
        public string downloads(string link)
        {
            string s = "";
            using (WebClient wc = new WebClient())
            {
                s= wc.DownloadString(link);
            }
            return s;
        }

        public Dictionary<string, string> getlistcate()
        {
            Dictionary<string, string> dictionaryCate = new Dictionary<string, string> {
             { "dogs-beds-and-mats-c-5589", "Beds & Mats"  },
             { "dogs-carriers-and-travel-products-c-5599", "Carriers & Travel Products" },
            { "clothing-and-shoes-c-5591", "Clothing & Shoes"  },
            { "dogs-collars-and-harnesses-and-leashes-c-5590", "Collars & Harnesses & Leashes"  },
            { "dogs-feeder-and-waterer-c-5596", "Feeder & Waterer" },
           { "dogs-cleaning-and-grooming-and-beauty-c-5594", "Cleaning & Grooming & Beauty"  },
            { "dogs-training-aids-c-5597", "Training Aids "  },
             { "dogs-health-c-5647", "Health "  },
             { "dogs-accessories-c-5592", "Accessories " },
             { "crates-and-kennels-and-pens-c-5587", "Crates & Kennels & Pens"  },
              { "dogs-toys-c-5595", "Toys"  }
            };
            return dictionaryCate;
        }

        public string download(string url)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string data = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "user";
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
