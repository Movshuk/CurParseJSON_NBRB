using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Web;
//using System.ComponentModel.DataAnnotations;

namespace CurParseJSON
{
   class Program
   {
      // метод принимает  JSON ответ и возвращает коллекцию строк {}
      static List<string> ParseJsonString(string jsonStr)
      {
         List<string> arrObj = new List<string>();

         for (int i = 0; i < jsonStr.Length; i++)
         {
            if (jsonStr[i] == '{')
            {
               string strTmp = null;
               for (int j = 0; jsonStr[i] != '}'; i++, j++)
               {
                  strTmp += jsonStr[i].ToString();
               }
               strTmp += "}";
               arrObj.Add(strTmp);
            }
         }
         return arrObj;
      }

      static void Main(string[] args)
      {
         WebRequest request = WebRequest.Create("http://www.nbrb.by/API/ExRates/Rates?Periodicity=0");

         WebResponse response = request.GetResponse();

         string jsonStr = null; // строка json массив

         using (Stream stream = response.GetResponseStream())
         {
            StreamReader reader = new StreamReader(stream);
            jsonStr = reader.ReadToEnd();
            reader.Close();
         }

         // парс ответа
         List<string> colectJS = new List<string>();
         colectJS.AddRange(ParseJsonString(jsonStr));

         // конвертирую json в объект Currency
         List<Currency> myCur = new List<Currency>();
         for (int i = 0; i < colectJS.Count; i++)  
         {
            // для каждой строки в коллекции colectJS
            Currency c = JsonConvert.DeserializeObject<Currency>(colectJS[i]);
            // добавляю в коолекцию объектов Currency
            myCur.Add(c);
         }

         Console.WriteLine("Курсы валют НБ РБ установленные на сегодня: " + DateTime.Now.ToShortDateString());
         Console.WriteLine("---------------------------------------------------------------------------");
         foreach (var i in myCur)
         {
            Console.WriteLine(i.Cur_Abbreviation + "\t" + i.Cur_Scale + "\t" + i.Cur_Name + "\t" + i.Cur_OfficialRate + "\t" + i.Date.ToShortDateString());
         }
      }
   }
}
