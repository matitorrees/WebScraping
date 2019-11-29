﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Clases
{
    public class ExtractorZonaProp
    {

        List<String> names = new List<String>();
        List<String> matriculados;
        String urlTotal;

        public ExtractorZonaProp(List<String> matr)
        {
            matriculados = matr;

        }

        public void Extract(object args)
        {
            try
            {
                String urlPart1 = "https://www.zonaprop.com.ar";
                String urlAddresss = (String)((List<object>)args)[0];
                //////////////////////////Conecto con la pagina/////////////////
                HttpClient client = new HttpClient();
                HttpResponseMessage response = client.GetAsync(urlAddresss).Result;

                HttpContent content = response.Content;
                String data = content.ReadAsStringAsync().Result;
                content.Dispose();
                response.Dispose();
                client.Dispose();

                /////////////////////////Extraigo el link de la publicacion///////////////////////////////////
                String urlPart2;
                String urlComplete;

                //Encuentro la lista
                data = data.Remove(0, data.IndexOf("data-to-posting=\"") + 17);

                for (int i = 0; i < 20; i++)
                {
                    //Saco la parte del url de la publicación                  
                    urlPart2 = data.Remove(data.IndexOf("data-index") - 2);

                    ////////////////Concateno y extraigo el nombre del vendedor desde el url de la publicacion//////////////////////
                    urlComplete = String.Concat(urlPart1, urlPart2);
                    urlTotal = urlComplete;

                    ExtractVendorName(urlComplete, ref args);
                    // ThreadPool.QueueUserWorkItem(new WaitCallback(ExtractVendorName), urlComplete);

                    //Avanzo hasta el otro url
                    data = data.Remove(0, data.IndexOf("data-to-posting=\"") + 17);
                    // Console.WriteLine(urlComplete);
                }
                ((List<object>)args)[3] = (int)((List<object>)args)[3] + 1;

            }
            catch
            {
                Console.WriteLine("mala4");
                if ((int)((List<object>)args)[4] < 5)
                    ((List<object>)args)[4] = (int)((List<object>)args)[4] + 1;

            }


            ////////////////////Espero a que terminen las tareas///////////////////
            lock (this)
            {
                //Console.WriteLine((int)((List<object>)args)[1]);
                ((List<object>)args)[1] = (int)((List<object>)args)[1] - 1;

                // Console.WriteLine("clarin bueno");
            }

        }

        public bool IsMatriculado(String n)
        {
            if (matriculados.Contains((String)n))
                return true;
            else return false;
        }

        public void ExtractVendorName(Object u, ref object args)
        {
            try
            {
                String urlAddress = (String)u;
                HttpClient client = new HttpClient();

                //////////////////////////Conecto con la pagina/////////////////
                HttpResponseMessage response = client.GetAsync(urlAddress).Result;
                HttpContent content = response.Content;
                String data = content.ReadAsStringAsync().Result;
                content.Dispose();
                response.Dispose();
                client.Dispose();

                ////////////////////Saco el nombre///////////////////////////////
                data = data.Remove(0, data.IndexOf("class=\"publisher-subtitle\"><b>") + 30);
                data = data.Remove(data.IndexOf("</b></h3>"));
                data = data.Replace("&#241;", "ñ");
                data = data.Replace("&#237;", "í");
                //names.Add(data);

                /////////////////////////Pregunto si es infractor////////////////////
                if (!IsMatriculado(data) && !((List<String>)((List<object>)args)[2]).Contains(data + "-" + urlTotal))
                {
                    //////////Agrego a la lista de infractores///////////////////
                    data = String.Concat(data + "-" + urlTotal);
                    ((List<String>)((List<object>)args)[2]).Add(data);
                    //Console.WriteLine(data);
                }

            }
            catch { }


        }



    }
}
