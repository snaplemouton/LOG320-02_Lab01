using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.HTTP_Handler
{
    /// <summary>
    /// Summary description for FileDownload
    /// </summary>
    public class FileDownload : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.ContentType = "text/plain";
            response.AddHeader("Content-Disposition",
                               "attachment; filename=" + context.Request.QueryString["fileName"] + ";");
            response.TransmitFile(context.Server.MapPath(context.Request.QueryString["mapPath"]));
            response.Flush();
            response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}