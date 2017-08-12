using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;
using System.Xml.Linq;

namespace OXml
{

    public class StuInfoXmlClass
    {
        public string name;
        public string value;
    }

    public static class ToXDocumentClass
    {
        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }
    }


    class OperateXml
    {
        /*
         public XmlDocument GetXMLFromUrl(string strUrl)
        {
            
            
            //方法二  
            Uri uri = new Uri(strUrl);
            System.Net.WebClient wb = new System.Net.WebClient();
            wb.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            MemoryStream ms = new MemoryStream(wb.DownloadData(strUrl));
            System.Xml.XmlTextReader rdr = new System.Xml.XmlTextReader(ms);
            XmlDocument doc = new XmlDocument();
            doc.Load(rdr);
            return doc;
            
        }

    */

        public XmlDocument GetXMLFromUrl(string strUrl)
        {
            // 该方法必须设定Encoding编码格式  
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
            request.Method = "POST";
            HttpWebResponse response;
            Stream responseStream;
            StreamReader reader;
            XmlDocument outxml = new XmlDocument();
            response = request.GetResponse() as HttpWebResponse;
            responseStream = response.GetResponseStream();
            reader = new System.IO.StreamReader(responseStream, Encoding.GetEncoding("UTF-8"));
            string srcString = reader.ReadToEnd();
            reader.Close();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(srcString);
            return doc;
        }

        

        public void GetStuInfoFromXml(XmlDocument xmlDoc)
        {

            /*
            XmlNamespaceManager nsp = new XmlNamespaceManager(xmlDoc.NameTable);
            nsp.AddNamespace("sso", "sso-namespace");

            XmlNode xn = xmlDoc.SelectSingleNode("sso:attributes",nsp);

            //XmlNodeList a = root.SelectNodes("child::sml:root[starts-with(@leaf,'d')]",nsp);

            MessageBox.Show(xn.ToString());

            XmlNodeList xnl = xn.ChildNodes;

            foreach (XmlNode xn1 in xnl)
            {
                StuInfoXmlClass stuInfoxml = new StuInfoXmlClass();
                // 将节点转换为元素，便于得到节点的属性值
                XmlElement xe = (XmlElement)xn1;
                // 得到name,type和value三个属性的属性值
                stuInfoxml.name = xe.GetAttribute("name").ToString();
                stuInfoxml.value = xe.GetAttribute("value").ToString();
                MessageBox.Show(stuInfoxml.name);
                MessageBox.Show(stuInfoxml.value);
            }

            */

            List<StuInfoXmlClass> StuInfoXmlList = new List<StuInfoXmlClass>();

            XDocument doc = ToXDocumentClass.ToXDocument(xmlDoc);

            XNamespace sso = @"sso-namespace";
            foreach (XElement element in doc.Descendants(sso + "attributes"))
            {
                foreach (XElement inelement in element.Descendants(sso + "attribute"))
                {
                    StuInfoXmlClass stuInfoXml = new StuInfoXmlClass();
                    stuInfoXml.name = (inelement.Attribute("name").Value);
                    stuInfoXml.value = (inelement.Attribute("value").Value);
                    StuInfoXmlList.Add(stuInfoXml);
                }
            }

            StuInfoXmlList.ForEach((element) =>
            {
                //姓名
                if (element.name == "user_name") SInfoClass.StuInfoClass.StuName = element.value;
                //角色
                if (element.name == "id_type") SInfoClass.StuInfoClass.role = int.Parse(element.value); 
                //学号
                if (element.name == "userName") SInfoClass.StuInfoClass.StuNumber = int.Parse(element.value);
                //部门or学院
                if (element.name == "unit_name") SInfoClass.StuInfoClass.department = element.value;
            });


        }


    }
}
