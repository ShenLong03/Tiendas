using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Domain;
using Tienda.Helpers;
using Tienda.Models;
using System.IO;
using System.Data.Entity.Infrastructure;
using Rotativa;

namespace Tienda.Controllers
{
    public class VentasController : Controller
    {/// <summary>
    /// ///
    /// </summary>
        private DataContextLocal db = new DataContextLocal();


        #region Ventas

    
        // GET: Ventas
        public async Task<ActionResult> Index()
        {
            Ayudas.CheckMediosPagos();
            var ventas = db.Ventas.Include(v => v.Cliente).OrderBy(q=>q.Fecha);
            return View(await ventas.ToListAsync());
        }
     
      

       

        
        // GET: Ventas/Details/5
        public async Task<ActionResult> Details(int? id,string Mensaje)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ventas ventas = await db.Ventas.FindAsync(id);
            if (ventas == null)
            {
                return HttpNotFound();
            }
            VentaViewModel view = Toview(ventas);

            view = SetTotal(view);

            Ventas venta = db.Ventas.Find(ventas.VentaId);

            venta.TotalOrden =view.TotalNeto;


            db.Entry(venta).State = EntityState.Modified;
            db.SaveChanges();
            ViewBag.Mensaje = Mensaje;
            return View(view);
        }

        private VentaViewModel SetTotal(VentaViewModel view)
        {
            if (view.DetalleVentas.Count()>0)
            {
                view.TotalBruto = view.DetalleVentas.Sum(q => q.Precio);
                view.TotalNeto = view.DetalleVentas.Sum(q => q.Subtotal);

            }
            else
            {
                view.TotalBruto = 0;
                view.TotalNeto = 0;
            }

            return view;
        }

        private VentaViewModel Toview(Ventas ventas)
        {
            return new VentaViewModel
            {
                ClienteId = ventas.ClienteId,
                Cliente = ventas.Cliente,
                DetalleVentas = ventas.DetalleVentas,
                Fecha = ventas.Fecha,
                Nombre = ventas.Nombre,
                Pagos = ventas.Pagos,
                TotalOrden = ventas.TotalOrden,
                VentaId = ventas.VentaId
                
            };


        }

        private DetalleVentas ToDetalleVenta(DetalleVentaViewModel detalleVentas)
        {
            return new DetalleVentas
            {
                VentaId = detalleVentas.VentaId,
                Cantidad = detalleVentas.Cantidad,
                Descuento = detalleVentas.Descuento,
                DetalleVentasId = detalleVentas.DetalleVentasId,
                Precio = detalleVentas.Precio,
                ProductoId = detalleVentas.ProductoId,
                Productos = detalleVentas.Productos
               
            };


        }


      

        // GET: Ventas/Create
        public ActionResult Create()
        {
            ViewBag.ClienteId = new SelectList(db.Clientes, "ClienteId", "Nombre");
            ViewBag.MedioPagoId = new SelectList(db.MediosPago, "MedioPagoId", "FormaPago");
            return View(new Ventas());
        }

        // POST: Ventas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Ventas ventas)
        {
            if (ModelState.IsValid)
            {
                db.Ventas.Add(ventas);
                await db.SaveChangesAsync();
                return RedirectToAction("Details",new {id=ventas.VentaId});
            }

            ViewBag.ClienteId = new SelectList(db.Clientes, "ClienteId", "Nombre", ventas.ClienteId);
            return View(ventas);
        }

        // GET: Ventas/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ventas ventas = await db.Ventas.FindAsync(id);
            if (ventas == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClienteId = new SelectList(db.Clientes, "ClienteId", "Nombre", ventas.ClienteId);
            return View(ventas);
        }

        // POST: Ventas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit( Ventas ventas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ventas).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ClienteId = new SelectList(db.Clientes, "ClienteId", "Nombre", ventas.ClienteId);
            return View(ventas);
        }

        // GET: Ventas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ventas ventas = await db.Ventas.FindAsync(id);
            if (ventas == null)
            {
                return HttpNotFound();
            }
            return View(ventas);
        }

        // POST: Ventas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Ventas ventas = await db.Ventas.FindAsync(id);
            db.Ventas.Remove(ventas);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region DetalleVenta

        [HttpPost]
        public async Task<ActionResult> BuscarArticulo(DetalleVentas detalleVenta)
        {


            detalleVenta = ToProducto(detalleVenta);

            try
            {
                if (detalleVenta.Cantidad == 0)
                {
                    var CantidadActual = db.Productos.Where(q => q.ProductoId == detalleVenta.ProductoId).FirstOrDefault().Cantidad;

                    ViewBag.Mensaje = "No Cuenta Con Esa Cantidad de Articulos, De Ese Producto Tienes en Inventario " + CantidadActual;




                    return RedirectToAction("CreateDetalleVenta", new { id = detalleVenta.VentaId, Mensaje = ViewBag.Mensaje });
                }
                else
                {

                    db.DetalleVentas.Add(detalleVenta);
                    await db.SaveChangesAsync();

                    return RedirectToAction("Details", new { id = detalleVenta.VentaId });

                }



            }
            catch (Exception ex)
            {

            }




            return View();




        }

        [HttpPost]
        public ActionResult BuscarPrecio(DetalleVentas detalleVenta)
        {



            var Producto = db.Productos.Where(p => p.ProductoId == detalleVenta.ProductoId).First();

            try
            {




            }
            catch (Exception ex)
            {

            }




            return Json(Producto.Precio, JsonRequestBehavior.AllowGet);




        }


        public async Task<ActionResult> DetailsDetalleVenta(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetalleVentas detalleVentas = await db.DetalleVentas.FindAsync(id);
            if (detalleVentas == null)
            {
                return HttpNotFound();
            }
            return View(detalleVentas);
        }

        // GET: DetalleVentas/Create
        public  ActionResult CreateDetalleVenta(int? id,string Mensaje)
   {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ventas ventas =  db.Ventas.Find(id);
            if (ventas == null)
            {
                return HttpNotFound();
            }
            ViewBag.Mensaje = Mensaje;
            return View(new DetalleVentaViewModel{
                VentaId=ventas.VentaId,
                Precio=ventas.DetalleVentas.FirstOrDefault().Precio,
                Ventas=ventas,
                GetProductos=db.Productos.ToList()
            });
        }


        // GET: DetalleVentas/Create
        public  ActionResult CreateDetalleVentaPartial(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ventas ventas =  db.Ventas.Find(id);
            if (ventas == null)
            {
                return HttpNotFound();
            }
            //ViewBag.ProductoId = new SelectList(db.Productos, "ProductoId", "Descripcion");
            return PartialView("_CreateDetalleVenta",new DetalleVentas
            {
                VentaId = ventas.VentaId,
                Ventas = ventas
            });
        }

        // POST: DetalleVentas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateDetalleVenta(DetalleVentaViewModel detalleVentas)
        {
            if (ModelState.IsValid)

            {
                var  detalleVenta = ToDetalleVenta(detalleVentas);
                detalleVenta = ToProducto(detalleVenta);

                if (detalleVenta.Cantidad > 0)
                {
                    db.DetalleVentas.Add(detalleVenta);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = detalleVentas.VentaId });
                }
                else {

                    var CantidadActual = db.Productos.Where(q=>q.ProductoId==detalleVenta.ProductoId).FirstOrDefault().Cantidad;
                  
                    ViewBag.Mensaje = "No Cuenta Con Esa Cantidad de Articulos, De Ese Producto Tienes en Inventario " + CantidadActual;

                    return RedirectToAction("Details", new { id = detalleVentas.VentaId,Mensaje=ViewBag.Mensaje });
                }
            }

            ViewBag.ProductoId = new SelectList(db.Productos, "ProductoId", "Descripcion",detalleVentas.ProductoId);
            detalleVentas.Ventas = await db.Ventas.FindAsync(detalleVentas.VentaId);
            return View((new DetalleVentaViewModel
            {
                VentaId = detalleVentas.VentaId,
                Precio = detalleVentas.Precio,
                Ventas = detalleVentas.Ventas,
                GetProductos = db.Productos.ToList()
            }));
        }


        private DetalleVentas ToProducto(DetalleVentas detalleVentas)
        {

            try
            {
               var Producto= db.Productos.Where(p => p.CodigoId == detalleVentas.ProductoId).First();
                detalleVentas.ProductoId = Producto.ProductoId;
                detalleVentas.Precio = Producto.Precio;
                if ((Producto.Cantidad-detalleVentas.Cantidad) >= 0)
                {

                  

                    Producto.Cantidad = Producto.Cantidad - detalleVentas.Cantidad;
                    db.Entry(Producto).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                   
                    detalleVentas.Cantidad = 0;
                    return detalleVentas;
                    
                    
                }

                return detalleVentas;
            }
            catch (Exception)
            {

                return detalleVentas;
            }
        }

        // GET: DetalleVentas/Edit/5
        public async Task<ActionResult> EditDetalleVenta(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetalleVentas detalleVentas = await db.DetalleVentas.FindAsync(id);
            if (detalleVentas == null)
            {
                return HttpNotFound();
            }
            ViewBag.VentaId = new SelectList(db.Ventas, "VentaId", "VentaId", detalleVentas.VentaId);
            return View(detalleVentas);
        }

        // POST: DetalleVentas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDetalleVenta( DetalleVentas detalleVentas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(detalleVentas).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.VentaId = new SelectList(db.Ventas, "VentaId", "VentaId", detalleVentas.VentaId);
            return View(detalleVentas);
        }

        // GET: DetalleVentas/Delete/5
        public async Task<ActionResult> DeleteDetalleVenta(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetalleVentas detalleVentas = await db.DetalleVentas.FindAsync(id);
            if (detalleVentas == null)
            {
                return HttpNotFound();
            }
            return View(detalleVentas);
        }

        // POST: DetalleVentas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedDetalleVenta(int id)
        {
            DetalleVentas detalleVentas = await db.DetalleVentas.FindAsync(id);
            db.DetalleVentas.Remove(detalleVentas);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Pagos

        public ActionResult CreatePago(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ventas ventas =  db.Ventas.Find(id);
            if (ventas == null)
            {
                return HttpNotFound();
            }
            ViewBag.MedioPagoId = new SelectList(db.MediosPago, "MedioPagoId", "FormaPago");
            var view = Toview(ventas);
            view = SetTotal(view);
            return PartialView(new PagoViewModel {VentaId=ventas.VentaId,ClienteId=ventas.ClienteId,TotalNeto=view.TotalNeto });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreatePago(PagoViewModel view)
        {
            if (ModelState.IsValid)
            {
               var pago= ToPago(view);
                pago.Monto -= view.Vuelto;
                db.Pagos.Add(pago);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

           
            ViewBag.MedioPagoId = new SelectList(db.MediosPago, "MedioPagoId", "FormaPago", view.MedioPagoId);
            return View(view);
        }

        private Pagos ToPago(PagoViewModel pago)
        {
            return new Pagos
            {
                ClienteId = pago.ClienteId,
                Fecha = pago.Fecha,
                MedioPagoId = pago.MedioPagoId,
                Monto = pago.Monto,
                VentaId = pago.VentaId,
                PagoId=pago.PagoId

            };
        }

        #endregion


        #region Reportes


        public ActionResult ExportPDF()

        {
            return new ActionAsPdf("Details")
            {

                FileName = Server.MapPath("contenc/invoice.pdf")
            };
        }
        //public ActionResult Report(string id)

        //{
        //    // LocalReport lr = new LocalReport();
        //    // string path = Path.Combine(Server.MapPath("~/Reportes"), "ReporteVentas.rdlc");

        //    // if (System.IO.File.Exists(path))
        //    // {
        //    //     lr.ReportPath = path;
        //    // }
        //    // else
        //    // {
        //    //     return View();
        //    // }

        //    // var Datos = db.Ventas.Where(q => q.VentaId == 1).ToList();

        //    // ReportDataSource report = new ReportDataSource("TiendaDataSet", Datos);

        //    // lr.DataSources.Add(report);
        //    // string reportType = id;
        //    // string mimeType = id;
        //    // string encoding;
        //    // string fileNameExtension;

        //    // //string deviceInfo =

        //    // //    "<DeviceInfo>" +
        //    // //    "<OutputFormat>" + id + "</OutputFormat>" +
        //    // //    "<PageWidth>8.5in</PageWidth>" +
        //    // //    "<PageHeight>11in<PageHeight>" +
        //    // //    "<MarginTop>0.5in<MarginTop>" +
        //    // //    "<MarginLeft>1in<MarginLeft>" +
        //    // //    "<MarginRight>1in<MarginRight>" +
        //    // //    "<MarginBottom>0.5in<MarginBottom>" +
        //    // //    "</DeviceInfo>";
        //    // string deviceInfo =
        //    //"<DeviceInfo>" +

        //    // "  <OutputFormat>PDF</OutputFormat>" +

        //    // "  <PageWidth>8.5in</PageWidth>" +

        //    // "  <PageHeight>11in</PageHeight>" +

        //    // "  <MarginTop>0.5in</MarginTop>" +

        //    // "  <MarginLeft>1in</MarginLeft>" +

        //    // "  <MarginRight>1in</MarginRight>" +

        //    // "  <MarginBottom>0.5in</MarginBottom>" +

        //    // "</DeviceInfo>";

        //    // Warning[] warnings;
        //    // string[] streams;
        //    // byte[] renderedBytes;

        //    // renderedBytes = lr.Render(
        //    //     reportType,
        //    //     deviceInfo,
        //    //     out mimeType,
        //    //     out encoding,
        //    //     out fileNameExtension,
        //    //     out streams,
        //    //     out warnings);

        //    // return File(renderedBytes, mimeType);

        //    var Datos = db.Ventas.Where(q => q.VentaId == 1).ToList();



        //    //var reportViewer = new ReportViewer();
        //    //reportViewer.LocalReport.ReportPath =
        //    //Server.MapPath("~/Reportes/ReporteVentas.rdlc");
        //    //reportViewer.LocalReport.DataSources.Clear();
        //    //reportViewer.LocalReport.DataSources.Add(new ReportDataSource("TiendaDataSet", Datos));
        //    //// ReportDataSource report = new ReportDataSource("TiendaDataSet", Datos);
        //    //reportViewer.LocalReport.Refresh();
        //    //reportViewer.ProcessingMode = ProcessingMode.Local;
        //    //reportViewer.AsyncRendering = false;
        //    //reportViewer.SizeToReportContent = true;
        //    //reportViewer.ZoomMode = ZoomMode.FullPage;

        //    //ViewBag.ReportViewer1 = reportViewer;
        //    //return View();
        //}

        //public ActionResult GenerateReport(int month, int year, int snapshottypeid, long taskrunid)
        //{
        //    var Datos = db.Ventas.Where(q => q.VentaId == 1).ToList();

        //    //Step 1 : Create a Local Report.
        //    LocalReport localReport = new LocalReport();

        //    //Step 2 : Specify Report Path.
        //    localReport.ReportPath = Server.MapPath("~/Reportes/ReporteVentas.rdlc");

        //    //Step 3 : Create Report DataSources
        //    //ReportDataSource dsUnAssignedLevels = new ReportDataSource();
        //    //dsUnAssignedLevels.Name = "UnAssignedLevels";
        //    //dsUnAssignedLevels.Value = dataSet.UnAssignedLevels;

        //    ReportDataSource dsReportInfo = new ReportDataSource("TiendaDataSet", Datos);
        //    //dsReportInfo.Name = "ReportInfo";
        //    //dsReportInfo.Value = dataSet.ReportInfo;

        //    //Step 4 : Bind DataSources into Report
        //  //  localReport.DataSources.Add(dsUnAssignedLevels);
        //    localReport.DataSources.Add(dsReportInfo);

        //    //Step 5 : Call render method on local Report to generate report contents in Bytes array
        //    string deviceInfo = "<DeviceInfo>" +
        //     "  <OutputFormat>PDF</OutputFormat>" +
        //     "</DeviceInfo>";
        //    Warning[] warnings;
        //    string[] streams;
        //    string mimeType;
        //    byte[] renderedBytes;
        //    string encoding;
        //    string fileNameExtension;
        //    //Render the report           
        //    renderedBytes = localReport.Render("PDF", deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);


        //    //Step 6 : Set Response header to pass filename that will be used while saving report.
        //    Response.AddHeader("Content-Disposition",
        //     "attachment; filename=UnAssignedLevels.pdf");

        //    //Step 7 : Return file content result
        //    return new FileContentResult(renderedBytes, mimeType);
        //}

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
