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

            DataElementFile def = new DataElementFile();
            List<DataElement> ele = new List<DataElement>();
            def.DataElements = ele;
            Domain dm = new Domain();
            dm.domain = "Education";
            dm.acronym = "edu";
            dm.version = "2017-09-26_15:54";
            dm.sourceURL = @"http://heimshelp.education.gov.au/sites/heimshelp/2018_Data_Requirements/2018DataElements/Documents/2018-HE-Data-Element-Dictionary.docx";
            List<OutputDataElement> listOdm = new List<OutputDataElement>();
            OutputDataElement odm;

            if (true)
            {
                for (int i = 3; i <count; i++)
                {
                    System.Console.WriteLine(i);
                    String elementData = doc.GetElementbyId("WordSection"+i).InnerHtml;
                    DataElement asdf = helpers.ConvertHTML2DataElement(elementData);
                    ele.Add(asdf);
                }

                foreach (DataElement dmf in def.DataElements)
                {
                    odm = new OutputDataElement();
                    odm.Name = dmf.ElementName;
                    odm.Domain = "Education";
                    odm.Status = "Standard";
                    odm.Definition = dmf.Description;
                    odm.dataType = new outputDataType() { facets = new facet(), type = dmf.CodeFormat.First().Value};
                    odm.sourceURL = "http://heimshelp.education.gov.au/sites/heimshelp/2018_data_requirements/2018dataelements/pages/"+dmf.ElementNumber;
                    odm.identifier = "https://dxa.gov.au/definition/"+dm.acronym+"/"+dm.acronym+dmf.ElementNumber;
                    odm.guidance = "Field Name: " + dmf.FieldName ;
                    odm.usage = new List<string>();
                    odm.usage.Add("See source for more information");
                    odm.values = new List<string>();
                    listOdm.Add(odm);
                }
                dm.content = listOdm;




                StreamWriter sw = new StreamWriter("edu.json",false);
                sw.Write(JsonConvert.SerializeObject(dm));
                sw.Close();
            }
            if (false)  
            {            
                String elementData = doc.GetElementbyId("WordSection16").InnerHtml;
                DataElement asdf = helpers.ConvertHTML2DataElement(elementData);
                System.Console.WriteLine(JsonConvert.SerializeObject(asdf));
            }

            System.Console.WriteLine("Press any key to exit");
            
            Console.ReadLine();
            
        }
    }
    public static class helpers
    {

        public static DataElement ConvertHTML2DataElement (string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            string sx = doc.DocumentNode.InnerText;
            sx = sx.Substring(sx.IndexOf("ELEMENT NO.")+12, 3);

            List<HtmlNode> tables = helpers.SplitTables(doc);
            List<ExtendedHTMLNode> tablesMerged = helpers.MergeTablesOnHeaders(tables);            
            DataElement de = new DataElement();
            de.ElementNumber=sx;
            foreach (ExtendedHTMLNode d in tablesMerged)
            {
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
                    int indexOfDataType = 0;
                    int indexOfUnits = 0;
                    int indexOfWidth = 0;

                    indexOfDataType = d.node.InnerText.IndexOf("Data Type:");
                    indexOfUnits = d.node.InnerText.IndexOf("Units:");
                    indexOfWidth = d.node.InnerText.IndexOf("Width:");
                    List<KeyValue> kv = new List<KeyValue>();
                    KeyValue kvDataType = new KeyValue();
                    KeyValue kvUnits = new KeyValue();
                    KeyValue kvWidth = new KeyValue();
                    kvDataType.Attr = d.node.InnerText.Substring(indexOfDataType,9).Trim();
                    kvDataType.Value = d.node.InnerText.Substring(indexOfDataType+10, indexOfUnits - (indexOfDataType+11)).Trim();
                    kvUnits.Attr = d.node.InnerText.Substring(indexOfUnits,5).Trim();
                    kvUnits.Value = d.node.InnerText.Substring(indexOfUnits+6, indexOfWidth - (indexOfUnits+7)).Trim();
                    kvWidth.Attr = d.node.InnerText.Substring(indexOfWidth,5).Trim();
                    kvWidth.Value = d.node.InnerText.Substring(indexOfWidth+6).Trim();
                    kv.Add(kvDataType);
                    kv.Add(kvUnits);
                    kv.Add(kvWidth);
                    de.CodeFormat = kv;
                    

                }  
                if(d.type==ElementType.Clas)
                {
                    bool lastLoopAdded = false;
                    List<KeyValue> kv = new List<KeyValue>();
                    KeyValue kvInstance;

                    List<string> para = d.node.InnerHtml.GetParagraphsListFromHtml();

                    try
                    {

                        for (int i = 1; i<para.Count(); i++)
                        {
                            if (!string.IsNullOrWhiteSpace(para[i]))
                            {
                                if (!lastLoopAdded){
                                    kvInstance = new KeyValue();
                                    kvInstance.Attr = para[i];
                                    kvInstance.Value = para[i+1];
                                    if(!kv.Contains(kvInstance))
                                    {
                                        kv.Add(kvInstance);
                                    }
                                    lastLoopAdded = true;
                                }
                                else
                                {
                                    lastLoopAdded = false;
                                }
                            }
                        }
                    }
                    catch{
                        kv.Add(new KeyValue { Attr="SystemIssue", Value="Error reading"+d.type.ToString()});
                    }
                    de.Classification = kv;
                }          
                if(d.type==ElementType.CoNt)
                {
                    de.CodingNotes = d.node.InnerText;
                }    
                if(d.type==ElementType.InFi)
                {
                    int indexOfVERSION = 0;
                    List<string> para = d.node.InnerHtml.GetParagraphsListFromHtml();
                    List<string> inputFiles = new List<string>();
                    foreach (string xyz in para)
                    {
                        if (xyz.Equals("VERSION"))
                        {
                            indexOfVERSION = para.IndexOf(xyz);
                            break;
                        }
                        string[] excludeWords = new string[] 
                            {"INPUT FILES:",
                            "HEP - Student",
                            "HEP - Staff",
                            "HEP - Applications and Offers"};
                        if (!excludeWords.Contains(xyz))
                        {
                            if (!inputFiles.Contains(xyz))
                            {
                                inputFiles.Add(xyz);
                            }
                        }
                    }
                    de.InputFiles = inputFiles;

                    List<ChangeRecord> changeHist = new List<ChangeRecord>();
                    ChangeRecord cr;
                    try
                    {
                        for (int i = indexOfVERSION+3; i<para.Count();)
                        {
                            if (!string.IsNullOrWhiteSpace(para[i]))
                            {
                                cr = new ChangeRecord();
                                cr.Version = para[i];
                                cr.RevisionDate = para[i+1];
                                cr.ReportingYear = para[i+2];
                                changeHist.Add(cr);
                                i = i+2;
                            }
                        }
                    }
                    catch {}
                    de.ChangeHistory = changeHist;
                }  
                if(d.type==ElementType.CHis)
                {
                    System.Console.WriteLine(d.type.ToString());
                    System.Console.WriteLine(d.node.InnerHtml);
                    
                }  

            }
            return de;

        }
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
                    output.Last().node.InnerHtml = string.Concat(output.Last().node.InnerHtml, Splittables[i].InnerHtml);
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

        public static List<string> GetParagraphsListFromHtml(this string sourceHtml)
        {

            var pars = new List<string>();

            //first create an HtmlDocument
            HtmlDocument doc = new HtmlDocument();

            //load the html (from a string)
            doc.LoadHtml(sourceHtml);

            //Select all the <p> nodes in a HtmlNodeCollection
            HtmlNodeCollection paragraphs = doc.DocumentNode.SelectNodes(".//p");

            //Iterates on every Node in the collection
            foreach (HtmlNode paragraph in paragraphs)
            {
                //Add the InnerText to the list
                pars.Add(paragraph.InnerText); 
                //Or paragraph.InnerHtml depends what you want
            }

            return pars;
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
