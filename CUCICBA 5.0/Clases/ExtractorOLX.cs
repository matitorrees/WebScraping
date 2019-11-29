using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Clases
{
    public class ExtractorOLX
    {
        List<String> names = new List<String>();
        List<String> matriculados;

        public ExtractorOLX(List<String> matr)
        {
            matriculados = matr;
        }

        public void Extract(object args)
        {
            try
            {

                String url = (String)((List<object>)args)[0];

                HttpClient client = new HttpClient();
                HttpResponseMessage response = client.GetAsync(url).Result;

                HttpContent content = response.Content;
                String data = content.ReadAsStringAsync().Result;
                content.Dispose();
                response.Dispose();
                client.Dispose();

                String b = data;
                String c = "http:";

                for (int i = 0; i < 20; i++)
                {
                    b = data;
                    b = b.Remove(b.IndexOf("<li class=" + '"' + "item "));
                    data = data.Remove(data.IndexOf("<li class=" + '"' + "item ") + 10); //extraigo link del matriculado y armo el link
                    b = b.Remove(b.IndexOf("href=") + 6);
                    c = c + b;
                    c = c.Remove(0, c.IndexOf("data") - 2);


                    HttpClient client2 = new HttpClient();
                    HttpResponseMessage response2 = client2.GetAsync(url).Result;

                    HttpContent content2 = response2.Content;
                    String data2 = content2.ReadAsStringAsync().Result;
                    content2.Dispose();
                    response2.Dispose();
                    client2.Dispose();

                    c = "http:";

                    data2 = data2.Remove(data2.IndexOf("<p class=" + '"' + "name") + 23); //extraigo el nombre del linj anterior
                    data2 = data2.Remove(0, data2.IndexOf('"' + ">"));

                    if (!IsMatriculado(data2) && !((List<String>)((List<object>)args)[2]).Contains(data2 + "" + data))
                    {
                        //////////Agrego a la lista de infractores///////////////////
                        ((List<String>)((List<object>)args)[2]).Add(data2 + "" + data);
                    }
                }

                ((List<object>)args)[3] = (int)((List<object>)args)[3] + 1;

            }
            catch (Exception ex)
            {

            }

            lock (this)
            {
                ((List<object>)args)[1] = (int)((List<object>)args)[1] - 1;
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

        }
    }
}
