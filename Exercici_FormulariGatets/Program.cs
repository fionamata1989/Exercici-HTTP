using Exercici_FormulariGatets;
using System.Net;
using System.Text;

class Program
{
    //Servidor que mostri un formulari amb GET i torni el contingut del formulari.
    static void Main(string[] args)
    {
        ListenAsync();
        Console.ReadKey();
    }

    //Crida asíncrona, no bloqueja la resta de programa.
    async static void ListenAsync()
    {
        //Crida al servidor Web HTTP Api de Windows.
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:51111/");  // Listen on
        listener.Start();                                         // port 51111.

        string paraulaRebuda = "";
        while (paraulaRebuda != "STOP")
        {
            HttpListenerContext context = await listener.GetContextAsync();
            Console.WriteLine(context.Request.RawUrl);
            //Quan hagim rebut la petició la processem.
            string msg = @"<html>
                                <head>
                                        <meta charset=""UTF-8"">
                                </head>
                                <body>
                                        Error petició
                                </body>
                                </html>";

            if (context.Request.RawUrl.Contains("formulari"))
            {
                msg = @"<html>
                                        <head>
                                                <meta charset=""UTF-8"">
                                                <title> Pràctica formulari KITTIES </title>
                                        </head>
                                        <body>
                                        <h1> Select the winner Cat</h1>
                                        <form method=""GET"" action=""enviarFormulari"">
                                        <label>
                                                <input type=""radio"" name=""option"" value=""option1""> Mittens
                                                </label>
                                                <br>
                                                <label>
                                                    <input type=""radio"" name=""option"" value=""option2""> Ollie
                                                </label>
                                                <br>
                                                <label>
                                                    <input type=""radio"" name=""option"" value=""option3""> Nunu
                                                </label>
                                                <br><br>

                                                <button type=""submit"" name=""enviarFormulari"">Vote!</button>
                                        </form>
                                        </body>
                                        </html>";
            }
            else if (context.Request.RawUrl.Contains("enviarFormulari"))
            {
                var qs = context.Request.Url.Query;
                qs = qs.Replace("?", "");
                var camps = qs.Split('&');
                var camp0 = camps[0].Split('=')[1];
                var camp1 = camps[1].Split('=')[1];
                var camp2 = camps[2].Split('=')[1];
                //Windows I Love You amb les codificacions. les URL van amb Windows-1252 o latin-1.
                var name = System.Web.HttpUtility.UrlDecode(camp0, Encoding.GetEncoding("UTF-8"));
                var image = System.Web.HttpUtility.UrlDecode(camp0, Encoding.GetEncoding("UTF-8"));
                var votes = System.Web.HttpUtility.UrlDecode(camp0, Encoding.GetEncoding("UTF-8"));
                // cognom = System.Web.HttpUtility.UrlDecode(camp1, Encoding.GetEncoding("UTF-8"));
                //Console.WriteLine(cognom);
                msg = @"<html>
                        <head>
                         <meta charset=""UTF-8"">
                        </head>
                        <body> Name:" + name + "<br> Votes:" + votes + "</body>" +
                        "<img src=>"
                    "</html>";
            }
            else if (context.Request.RawUrl.Contains("STOP"))
            {
                paraulaRebuda = "STOP";
                msg = "STOP";
            }
            EscriuResposta(context, msg);
        }
        listener.Stop();
    }

    async static void EscriuResposta(HttpListenerContext context, string msg)
    {
        context.Response.ContentLength64 = Encoding.UTF8.GetByteCount(msg);
        context.Response.StatusCode = (int)HttpStatusCode.OK;
        using (Stream s = context.Response.OutputStream)
        using (StreamWriter writer = new StreamWriter(s))
            await writer.WriteAsync(msg); //Escriu quan puguis. Continua amb la resta de programa, quan acabis continua per aquí.
    }



    private static Dictionary<string, InfoCats> catData = new Dictionary<string, InfoCats>
    {
        { "option1", new InfoCats { Name = "Mittens", Image = "mittens.jpg", Votes = 0 } },
        { "option2", new InfoCats { Name = "Ollie", Image = "ollie.jpg", Votes = 0 } },
        { "option3", new InfoCats { Name = "Nunu", Image = "nunu.jpg", Votes = 0 } }
    };
}