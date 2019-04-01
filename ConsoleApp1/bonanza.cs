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
    class bonanza
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
            //https://www.bonanza.com/items/search?q[filter_category_id]=177788&q[search_term]=dog%20food
            sUrl = "https://www.bonanza.com/items/search?q[filter_category_id]={1}&q[search_term]={0}";
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
            oProduct.Url = SiteUrl+ HttpUtility.HtmlDecode(mDetail.Groups[1].Value);
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
                 { 20744,"Beds"},
                { 121851,"Cages & Crates"},
                { 177788,"Carriers & Totes"},
                { 177796,"Clothing & Shoes"},
                { 63057,"Collars"},
                { 177789,"Dishes, Feeders & Fountains"},
                { 77664,"Dog Chews & Treats"},
                { 66780,"Dog Food"},
                 { 116378,"Dog Lover Products"},
                { 20749,"Flea & Tick Remedies"},
                { 177792,"Grooming"},
                { 134752,"Health Care"},
                { 146247,"Leashes & Head Collars"},
                { 11286,"Other"},
                { 46299,"Signs & Plaques"},
                { 177791,"Toys"},
                { 116381,"Training & Obedience "},
                { 116392,"Pooper Scoopers & Bags"},
                { 116377,"Blankets"},
                { 117426,"Car Seat Covers"},
                { 177790,"Tags & Charms"},
                { 46454,"Car Seats & Barriers"},
                { 116373,"Diapers & Belly Bands"},
                { 52352,"Dog Costumes"},
                { 108884,"Dog Houses"},
                 { 116379,"Doors & Flaps"},
                { 20748,"Fences & Exercise Pens"},
                { 66783,"Harnesses"},
                { 134755,"Odor & Stain Removal"},
                { 116389,"Ramps & Stairs"},
                { 117427,"Safety Vests & Life Preservers"},
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
