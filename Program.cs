using System;
using HtmlAgilityPack;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace HEIMS_DOC_TO_JSON
{
    class Program
    {
        static void Main(string[] args)
        {

            //Read the input file into a string and make it readable by the program
            StreamReader sr = new StreamReader("Data.htm");
            string x = sr.ReadToEnd();
            sr.Close();            
            //Extract number of elements
            HtmlDocument inidoc = new HtmlDocument();
            inidoc.LoadHtml(x);
            string y = inidoc.DocumentNode.SelectSingleNode("//body").InnerHtml;
            int count = y.CountSubstring("WordSection");
            System.Console.WriteLine(count);
            
            //further processing to speeed functions            
            string data = x.Replace("class=WordSection","id=WordSection").Replace("\n","").Replace("\r","").Replace("\t","").Replace("&nbsp;", "").Replace("  ", " ");

            //read the file as a HtmlDocument
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(data);
            helpers.RemoveAttributes(doc);


            String elementData = doc.GetElementbyId("WordSection3").InnerHtml;
            
            List<HtmlNode> tables = helpers.SplitTables(elementData);
            List<ExtendedHTMLNode> tablesMerged = helpers.MergeTablesOnHeaders(tables);
            DataElement de = new DataElement();

            foreach (ExtendedHTMLNode d in tablesMerged)
            {
                System.Console.WriteLine("Table:");
                System.Console.WriteLine(d.node.InnerHtml);
                //System.Console.WriteLine(d.InnerText.Trim());;
                if (d.type == ElementType.Vers)
                {
                    de.Version = d.node.InnerText.Replace("VERSION:", "").Trim();
                }
                if (d.type == ElementType.FYear)
                {
                    de.FirstYear = d.node.InnerText.Replace("FIRST YEAR:", "").Trim();
                }
                if (d.type == ElementType.LYear)
                {
                    de.LastYear = d.node.InnerText.Replace("LAST YEAR:", "").Trim();
                }
                if (d.type == ElementType.Fld)
                {
                    de.FieldName = d.node.InnerText.Replace("FIELD NAME:", "").Trim();
                }
                if (d.type == ElementType.EleNM)
                {
                    de.ElementName = d.node.InnerText.Replace("ELEMENT NAME:", "").Trim();
                }
                if (d.type == ElementType.Desc)
                {
                    de.Description = d.node.InnerText.Replace("DESCRIPTION:", "").Trim();
                }
                if(d.type==ElementType.Frmt)
                {

                }  
                if(d.type==ElementType.Clas)
                {
                    
                }          
                if(d.type==ElementType.CoNt)
                {
                    
                }    
                if(d.type==ElementType.InFi)
                {
                    
                }  
                if(d.type==ElementType.CHis)
                {
                    
                }  

            }
            System.Console.WriteLine(JsonConvert.SerializeObject(de));
            
            Console.ReadLine();
            
        }
    }
    public static class helpers
    {
        public static List<ExtendedHTMLNode> MergeTablesOnHeaders(List<HtmlNode> Splittables)
        {
            string[] headers = new string[] 
            {"VERSION:",
            "FIRST YEAR:",
            "LAST YEAR:",
            "FIELD NAME:",
            "ELEMENT NAME:",
            "DESCRIPTION:",
            "CODE FORMAT:",
            "CLASSIFICATION:",
            "CODING NOTES:",
            "INPUT FILES:",
            "VERSION   REVISION DATE   REPORTING YEAR"};
            List<ExtendedHTMLNode> output = new List<ExtendedHTMLNode>();
            int foundElements = 0;

            ExtendedHTMLNode currentNode;
            for (int i = 0; i < Splittables.Count(); i++)
            {
                if(Splittables[i].InnerHtml.Contains(headers[foundElements]))
                {
                    currentNode = new ExtendedHTMLNode(Splittables[i],(ElementType)foundElements);                    
                    output.Add(currentNode);
                    foundElements++;
                }
                else
                {
                    output.Last().node.InnerHtml = string.Concat(Splittables[i-1].InnerHtml, Splittables[i].InnerHtml);
                }
            }
            return output;
        }

        public static List<HtmlNode> SplitTables(string HTMLString)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(HTMLString);
            return SplitTables(doc);

        }

        public static List<HtmlNode> SplitTables(this HtmlDocument HTMLDoc)
        {
            var tableNodes = HTMLDoc.DocumentNode.SelectNodes("//table");
            List<HtmlNode> tables = tableNodes.Nodes().ToList();
            List<HtmlNode> tables2 = (from t in tables where !string.IsNullOrWhiteSpace(t.InnerHtml) select t).ToList();
            return tables2;
        }



        public static void RemoveAttributes(this HtmlDocument HTMLDoc)
        {   
            //string[] attrs = new string[] 
            //{"valign","border","cellspacing", "cellpadding", "width"};

            string[] attrs = new string[] 
            {"style","class","cellspacing", "cellpadding", "width", "valign", "border"};

            foreach (string s in attrs)
            {
                RemoveAttribute(HTMLDoc, s);
            }
        }
        static void RemoveAttribute(this HtmlDocument html, string attr)
        {
            var elementwithattr = html.DocumentNode.SelectNodes("//@"+attr);

            if (elementwithattr!=null)
            {
                foreach (var element in elementwithattr)
                {
                    element.Attributes[attr].Remove();
                }
            }
        }
        public static int CountSubstring(this string text, string value)
        {                  
            int count = 0, minIndex = text.IndexOf(value, 0);
            while (minIndex != -1)
            {
                minIndex = text.IndexOf(value, minIndex + value.Length);
                count++;
            }
            return count;
        }


    }
    public enum ElementType {Vers=0, FYear=1, LYear=2, Fld=3, EleNM=4, Desc=5, Frmt=6, Clas=7, CoNt=8,InFi=9,CHis=10};

    public class ExtendedHTMLNode
    {
        public ExtendedHTMLNode(HtmlNode n, ElementType t )
        {
            node = n;
            type = t;
        }

        public HtmlNode node {get; set;}
        public ElementType type {get;set;}
    }
}
