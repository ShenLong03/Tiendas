using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tienda.Models;

namespace Tienda
{
    public partial class Prueba : System.Web.UI.Page
    {
        DataContext db = new DataContext();
        protected void Page_Load(object sender, EventArgs e)
        {
            var prendas = db.Clientes.Take(28).ToList();

            foreach (var item in prendas)
            {

                var botones = new Button();


                botones.ID = item.ClienteId.ToString();
                botones.Text = item.Nombre.ToString();
               botones.Width = new Unit("100px");
                botones.Height = new Unit("100px");
               
                this.Panel1.Controls.Add(botones);


                
            }


        }
    }
}