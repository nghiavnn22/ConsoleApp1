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
    class _1800petmeds_com
    {
        public string keyword = "royal";
        public string niche = "DOG";
        public List<string> listWebContent;
        public Dictionary<string, string> saveCatewithWebContent;

        public Dictionary<int, Dictionary<string, string>> DicCateIdToDicCateNameandLink = new Dictionary<int, Dictionary<string, string>> {
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
        public List<Product> getListProduct()
        {
            downloadCotentPage();
            return ExtractProductInfor();
        }
        private List<Product> ExtractProductInfor()
        {
            List<Product> listProduct = new List<Product>();

            foreach (var cateHtmlContent in listWebContent)
            {
                MatchCollection mProductCollection = new Regex(@"class=""product"".*?(=?class=""product"")", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(cateHtmlContent);
                if (mProductCollection.Count < 1)
                {
                    continue;
                }
                for (int i = 0; i < mProductCollection.Count; i++)
                {
                    if (!mProductCollection[i].Value.ToString().Contains("$"))
                    {
                        continue;
                    }
                    Product oProduct = new Product();
                    oProduct = getProduct(mProductCollection[i].Value.ToString());
                    oProduct.Category = saveCatewithWebContent[cateHtmlContent];
                    listProduct.Add(oProduct);
                }
            }

            return listProduct;
        }
        private Product getProduct(string sProduct)
        {
            Product oProduct = new Product();
            Regex rxDetail = new Regex(@"href=""(.*?)"".*?alt=""(.*?)"".*?data-yo-src=""(.*?)"".*?""sale"".*?([\d.,]+)<", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            //Regex rxDetail = new Regex(@"product_cat -([\w-]+).*?href=""(.*?)"".*?src=""(.*?)"".*?alt=""(.*?)"".*?amount"">([\d.]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Match mDetail = rxDetail.Match(sProduct);
            oProduct.SiteId = "1800petmeds";
            oProduct.Name = HttpUtility.HtmlDecode(mDetail.Groups[2].Value.Trim());
            //oProduct.Category = categoryName;

            //oProduct.Category = (mDetail.Groups[1].Value.Trim() == "") ? "" : mDetail.Groups[1].Value.Trim().Replace("-", " ");
            oProduct.Brand = "";
            //oProduct.Price = 0;
            //if (Utility.IsNumber(mDetail.Groups[5].Value.Trim()) == true)          
            oProduct.Price = double.Parse(mDetail.Groups[4].Value.ToString());

            oProduct.Quantity = 0;
            oProduct.Image = mDetail.Groups[3].Value;
            oProduct.Url = "https://www.1800petmeds.com" + mDetail.Groups[1].Value;
            oProduct.IsActive = true;
            //// change price
            //oProduct.UsdPrice = Utility.Exchange(oProduct.Price, this.Currency);
            return oProduct;
        }
        public Dictionary<int, Dictionary<string, string>> getNiche()
        {
            return DicCateIdToDicCateNameandLink;
        }
        public void downloadCotentPage()
        {
            saveCatewithWebContent = new Dictionary<string, string>();
            listWebContent = new List<string>();
            Dictionary<int, Dictionary<string, string>> listCateGory = getNiche();
            //for (int i = 0; i < 2; i++)
            //{
            //    Dictionary<string, string> ss = listCateGory[i];
            //    string linkSearch = ss.First().ToString();
            //    String contentForALink = download(linkSearch);
            //    listWebContent.Add(contentForALink);
            //}
            foreach (var itemDic in listCateGory)
            {
                String linkSearch = "https://www.1800petmeds.com" + itemDic.Value.First().Key.ToString() + "&Ntt=" + keyword;
                String contentForALink = download(linkSearch);
                listWebContent.Add(contentForALink);
                saveCatewithWebContent.Add(contentForALink, itemDic.Value.First().Value.ToString());
            }

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

