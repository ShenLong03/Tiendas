using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tienda.Models;
using Domain.DataModel;
using System.Data.SqlClient;
using System.Data;
using Tienda.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Domain;

namespace Tienda
{
    public partial class Report : System.Web.UI.Page
    {
        private DataContext db = new DataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarData();
               
            }
        }

     public void   CargarData()
        {
            ///metodo usando linq
            ///using(datacontex de = new datacontex())
            ///{
            ///var v=(from a in dc.precedure()
            ///select a);
            ///ReportetDataSource rd=new ReporteDataSource("NOMBRE DATASET",v.tolist());
            ///reportViewer1.LocalReport.DAtasourseces.add(rd);
            ///this.reportViewer1.LocalReport.RefreshReport();
            ///
            /// }
            /// 
            //using(var context = new TiendaEntities())
            //{
            //    var courses = context.sp_CargarDataFacuras(1);
           // DataTable table = new DataTable();
           // SqlConnection conn = new SqlConnection("Data Source=BRAINER-PC\\SQLEXPRESS;Initial Catalog=Tienda;Integrated Security=True");
           //// String SpNombre;
           // SpNombre="sp_CargarDataFacuras";
           // //SqlCommand cmd = new SqlCommand();
           // //SqlParameter[] param = new SqlParameter[1];
           // //param[0] = new SqlParameter("@idVenta", SqlDbType.Int);
           // //param[0].Value = 1;
           // //cmd.CommandType = CommandType.StoredProcedure;
           // //cmd.CommandText = "sp_CargarDataFacuras";
           // //cmd.Connection = conn;
           // //cmd.Parameters.AddRange(param);
           // //DataSet ds = new DataSet();
           // SqlDataAdapter da = new SqlDataAdapter(SpNombre, conn);
           // conn.Open();

           // da.SelectCommand.CommandType = CommandType.StoredProcedure;
           // da.SelectCommand.Parameters.Add("@idVenta", SqlDbType.Int).Value = 1;
           //var data= da.Fill(table);

       
            //    TiendaEntities tienda = new TiendaEntities();
            //var datas = tienda.sp_CargarDataFacuras(1);
            //   var datas = db.sp_CargarDataFacuras(1);

            // var Datos = db.Ventas.Select(q=>new { q.VentaId,q.Nombre,q.Fecha,q.TotalOrden }).ToList();


            var Datos = (from v in db.Ventas
                         join dv in db.DetalleVentas on v.VentaId equals dv.VentaId
                         join pv
in db.Productos on dv.ProductoId equals pv.ProductoId
                         where v.VentaId == 2
                         select new { v.VentaId,v.Nombre,v.TotalOrden,v.Fecha,dv.Cantidad,dv.Descuento,dv.Precio,pv.Descripcion,pv.Talla }).ToList();

            //var Datos2 = db.DetalleVentas.Select(q => new { q.Cantidad, q.Descuento, q.Precio, q.Subtotal }).ToList();
            //System.Data.DataTable data = new System.Data.DataTable("Table1");
            //data.Fill(Datos);

            ReportViewer1.ProcessingMode = ProcessingMode.Local;
           ReportViewer1.LocalReport.ReportPath =
            Server.MapPath("~/Reportes/Report2.rdlc");


            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataVentasDetalle", Datos));
           // ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", Datos2));
            ReportViewer1.LocalReport.Refresh();

            ReportViewer1.AsyncRendering = false;
            ReportViewer1.SizeToReportContent = true;
            ReportViewer1.ZoomMode = ZoomMode.FullPage;


          //  AutoPrintCls autoprintme = new AutoPrintCls(ReportViewer1.LocalReport);
          //  autoprintme.Print();
            //  }
        }



        protected void LinqDataSource1_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var Datos = db.Ventas.Where(q => q.VentaId == 1).ToList();

            //System.Data.DataTable data = new System.Data.DataTable("Table1");
            //data.Fill(Datos);

            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.ReportPath =
             Server.MapPath("~/Reportes/ReporteVentas.rdlc");


            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource("TiendaDataSet", Datos));

            ReportViewer1.LocalReport.Refresh();

            ReportViewer1.AsyncRendering = false;
            ReportViewer1.SizeToReportContent = true;
            ReportViewer1.ZoomMode = ZoomMode.FullPage;
        }
    }
}