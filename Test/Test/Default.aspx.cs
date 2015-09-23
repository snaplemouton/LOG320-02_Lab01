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
            FileManager fileManager = new FileManager();
            fileManager.CompressFile(file);

        }

        private void dedoThis()
        {
            
        }
    }
}
