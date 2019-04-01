using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    class CrawlerName
    {
        string keyword = "";
        string gioitinh = "MALE";
        List<string> listStringLink = null;
        List<string> listWebContent = null;
        List<PersionName> listPersionName = null;
        public string getGioiTinh()
        {
            string gGioitinh="";
            switch (gioitinh)
            {
                case "MALE":
                    gGioitinh= "MALE";
                    break;
                case "FEMALE":
                    gGioitinh= "FEMALE";
                    break;
            }
            return gGioitinh;
        }
        public void DownloadConentPage()
        {
            listStringLink = new List<string>();
            listWebContent = new List<string>();
            for (int i = 0; i < 100; i++)
            {
                string link = "https://www.babycenter.com/babyNamerSearch.htm?startIndex="+i*100+"&excludeLimit=100&gender="+getGioiTinh()+ "&containing=&origin=&includeLimit=100&sort=&meaning=&endsWith=&theme=&batchSize=100&includeExclude=ALL&numberOfSyllables=&startsWith=";
                listStringLink.Add(link);
                listWebContent.Add(download(link));
            }

        }
        
        public List<PersionName> getListPersionName()
        {
            DownloadConentPage();
            return ExtractPersionNameInfor();

        }
        public  List<PersionName> ExtractPersionNameInfor()
        {
            listPersionName = new List<PersionName>();
            foreach (var swebcontent in listWebContent)
            {
                MatchCollection mListPersonName = new Regex(@"id=""name_[\d].*?htm"">(\w+)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Matches(swebcontent);
                if (mListPersonName.Count<1)
                {
                    return null;
                }
                for (int i = 0; i < mListPersonName.Count; i++)
                {
                    PersionName oPersionName = new PersionName();
                    oPersionName = getPersionName(mListPersonName[i].Value.ToString());
                    listPersionName.Add(oPersionName);
                }


            }
            return listPersionName;
        }
        public PersionName getPersionName(string sPersionName)
        {
            PersionName oPersionName = new PersionName();
            Regex rxDetail = new Regex(@"htm"">([\w]+)", RegexOptions.IgnoreCase|RegexOptions.Singleline);
            Match mDetail = rxDetail.Match(sPersionName);
            oPersionName.name = mDetail.Groups[1].Value.ToString();
            oPersionName.gioitinh = "male";
            System.IO.File.AppendAllText(@"F:\name.txt",oPersionName.name+ "\n");
            return oPersionName;
        }
        public string download(string url)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string data = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode==HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.CharacterSet==null)
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
    public class PersionName
    {
        public string name { get; set; }
        public string gioitinh { get; set; }
    }
}
