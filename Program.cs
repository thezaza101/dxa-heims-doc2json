using System;
using HtmlAgilityPack;
using System.Linq;
using System.Collections;

namespace HEIMS_DOC_TO_JSON
{
    class Program
    {
        static void Main(string[] args)
        {
            string data = "";
            //Read file
            //Replace "class=WordSection" with "id=WordSection"

            HtmlDocument doc = new HtmlDocument();
            doc.Load("Data1.htm");
            data = doc.GetElementbyId("WordSection3").InnerHtml;
            System.Console.WriteLine(data);
            Console.ReadLine();
            
        }
    }
}
