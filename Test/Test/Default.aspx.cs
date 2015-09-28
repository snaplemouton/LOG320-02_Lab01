using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Test.Classes;

namespace Test
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void doThis(object sender, EventArgs e)
        {
            HttpPostedFile file = Request.Files["fileDo"];
            if (file.ContentLength > 0)
            {
                FileManager fileManager = new FileManager();
                fileManager.CompressFile(file);
            }
            else
            {
                popupAlert("Veuillez choisir un fichier à compresser.");
            }

        }

        protected void dedoThis(object sender, EventArgs e)
        {
            HttpPostedFile file = Request.Files["fileDedo"];
            if (file.ContentLength > 0)
            {
                FileManager fileManager = new FileManager();
                fileManager.DecompressFile(file);
            }
            else
            {
                popupAlert("Veuillez choisir un fichier à décompresser.");
            }
        }

        private void popupAlert(string message)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<script type = 'text/javascript'>");
            sb.Append("window.onload=function(){");
            sb.Append("alert('");
            sb.Append(message);
            sb.Append("')};");
            sb.Append("</script>");
            ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", sb.ToString());
        }
    }
}
