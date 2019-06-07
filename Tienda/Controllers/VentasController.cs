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
using Microsoft.Reporting.WebForms;
using System.Globalization;

namespace Tienda.Controllers
{
    public class VentasController : Controller
    {/// <summary>
    /// ///
    /// </summary>
        private DataContextLocal db = new DataContextLocal();


        #region Account
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(Usuario model, string returnUrl)
        {
          


            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {




                if (db.Usuarios.Where(u => u.Clave == model.Clave && u.Nombre == model.Nombre).Count()>0) {

                        var userID = await db.Usuarios.Where(u => u.Clave == model.Clave&&u.Nombre==model.Nombre).FirstAsync();


                if (userID!=null) {
                     
                        var userView = new Usuario
                        {
                            Clave = userID.Clave,
                            Nombre = userID.Nombre,
                            Perfil = userID.Perfil
                          
                        };
                   

                        Session["UsuarioLogin"] = userView;
                    return RedirectToAction("Index", "Ventas", new { });
                    }
                    else
                    {

                        return RedirectToAction("Login", "Ventas", new { });
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Ventas", new { });
                   
                }

               

            }
          
           

        }

        #endregion

        #region Ventas


        // GET: Ventas
        public async Task<ActionResult> Index(string sFechaInicial, string sFechaFinal,int? Validar)
         {
           if( Session["UsuarioLogin"] !=null){
                DateTime FechaInicial = DateTime.Today;
                DateTime FechaFinal = DateTime.Today.AddHours(23).AddMinutes(59);
                if (!string.IsNullOrEmpty(sFechaInicial))
                {
                    FechaInicial = DateTime.ParseExact(sFechaInicial, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(sFechaFinal))
                {
                    FechaFinal = DateTime.ParseExact(sFechaFinal, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FechaFinal = FechaFinal.AddHours(23).AddMinutes(59);

                }
                

                ViewBag.FechaInicial = FechaInicial;
                ViewBag.FechaFinal = FechaFinal;
                if (sFechaFinal==null) { 
                Ayudas.CheckMediosPagos();

            var ventas = db.Ventas.Include(v => v.Pagos).Where(v=>v.Fecha==DateTime.Today||v.CantidadPendiente>0).OrderByDescending(q=>q.VentaId);
            return View(await ventas.ToListAsync());
                }
                if (Validar==1)
                {
                    var facturas = db.Ventas.Where(f => f.Fecha >= FechaInicial && f.Fecha <= FechaFinal&&f.CantidadPendiente>0).ToList();
                    return View(facturas.OrderByDescending(f => f.VentaId));
                } else {
                    var facturas = db.Ventas.Where(f => f.Fecha >= FechaInicial && f.Fecha <= FechaFinal).ToList();
                    return View(facturas.OrderByDescending(f => f.VentaId));
                }
            }
            else
            {
                return RedirectToAction("Login", "Ventas", new { });
            }

        }
     
      

       

        
        // GET: Ventas/Details/5
        public async Task<ActionResult> Details(int? id,string Mensaje)
        {
            if (Session["UsuarioLogin"] != null)
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

                venta.TotalOrden = view.TotalNeto;
                venta.CantidadPendiente = view.TotalNeto;

                db.Entry(venta).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Mensaje = Mensaje;
           
            return View(view);
            }
            else { return RedirectToAction("Login", "Ventas", new { }); }
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

        private DetalleVentaViewModel ToDetalleViewModel(DetalleVentas detalleVentas)
        {
            return new DetalleVentaViewModel
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
            if (Session["UsuarioLogin"] != null)
            {
                ViewBag.ClienteId = new SelectList(db.Clientes, "ClienteId", "Nombre");
                ViewBag.MedioPagoId = new SelectList(db.MediosPago, "MedioPagoId", "FormaPago");
                return View(new Ventas());
            }
            else { return RedirectToAction("Login", "Ventas", new { }); }
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
            if (Session["UsuarioLogin"] != null)
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
        }else{ return RedirectToAction("Login", "Ventas", new { }); }
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
            if (Session["UsuarioLogin"] != null)
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
        }else{ return RedirectToAction("Login", "Ventas", new { }); }
        }

        // POST: Ventas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
           
                Ventas ventas = await db.Ventas.FindAsync(id);
            try
            {
                foreach (var item in ventas.DetalleVentas.ToList())
            {
                db.DetalleVentas.Remove(item);
                await db.SaveChangesAsync();
            }
            foreach (var item in ventas.Pagos.ToList())
            {
                db.Pagos.Remove(item);
                await db.SaveChangesAsync();
            }
          
                db.Ventas.Remove(ventas);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = "No puedes Borrar esta Factura porque tiene Detalles y Pagos ";
                return View(ventas);
            }
            
        }
        #endregion


        #region DetalleVenta


        public  JsonResult GetProductos(int CategoriaId)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
               


             
               var productos =  db.Productos.Where(p => p.CategoriaId == CategoriaId).ToList();
                return Json(productos, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult> BuscarArticulo(DetalleVentas detalleVenta)
        {


            detalleVenta = ToProductos(detalleVenta);

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

       

        public JsonResult BuscarPrecio(DetalleVentas detalleVenta)
        {


            db.Configuration.ProxyCreationEnabled = false;
            var Producto = db.Productos.Where(p => p.ProductoId == detalleVenta.ProductoId).First();

       



            return Json(Producto, JsonRequestBehavior.AllowGet);




        }


        public async Task<ActionResult> DetailsDetalleVenta(int? id)
        {
            if (Session["UsuarioLogin"] != null)
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
            else { return RedirectToAction("Login", "Ventas", new { }); }
        }

        // GET: DetalleVentas/Create
        public  ActionResult CreateDetalleVenta(int? id,string Mensaje)
   {
            if (Session["UsuarioLogin"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Ventas ventas = db.Ventas.Find(id);
                if (ventas == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Mensaje = Mensaje;
                var Foto = db.Productos.Where(q => q.ProductoId == 1).FirstOrDefault().Foto;
                var Categorias = db.Productos.ToList();
                return View(new DetalleVentaViewModel
                {
                    VentaId = ventas.VentaId,
                    Precio = 15000,
                    Foto = Foto,
                    GetCategorias = db.Categorias.ToList()
                });
            }
            else { return RedirectToAction("Login", "Ventas", new { }); }
        }


        // GET: DetalleVentas/Create
        public  ActionResult CreateDetalleVentaPartial(int? id)
        {
            if (Session["UsuarioLogin"] != null)
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
        }else{ return RedirectToAction("Login", "Ventas", new { }); }
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
                if (detalleVenta.ProductoId == 0)
                {
                    ViewBag.Mensaje = "Articulo No Existe, Codigo Incorrecto ";

                    return RedirectToAction("Details", new { id = detalleVentas.VentaId, Mensaje = ViewBag.Mensaje });

                    
                }
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
                if (db.Productos.Where(p => p.CodigoId == detalleVentas.ProductoId.ToString()).ToList().Count>0) {
               var Producto= db.Productos.Where(p => p.CodigoId == detalleVentas.ProductoId.ToString()).First();
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
                else
                {
                    detalleVentas.ProductoId = 0;
                    return detalleVentas; }
            }
            catch (Exception)
            {

                return detalleVentas;
            }
        }
        private DetalleVentas ToProductos(DetalleVentas detalleVentas)
        {

            try
            {
                
                    var Producto = db.Productos.Where(p => p.ProductoId == detalleVentas.ProductoId).First();
                    detalleVentas.ProductoId = Producto.ProductoId;
                    detalleVentas.Precio = Producto.Precio;
                    if ((Producto.Cantidad - detalleVentas.Cantidad) >= 0)
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
        public async Task<ActionResult> EditDetalleVenta(int? id,string Mensaje)
        {
            if (Session["UsuarioLogin"] != null)
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetalleVentas detalleVentas = await db.DetalleVentas.FindAsync(id);
            detalleVentas= ToDetalleViewModel(detalleVentas);
            if (detalleVentas == null)
            {
                return HttpNotFound();
            }
            ViewBag.Mensaje = Mensaje;
            return View(new DetalleVentaViewModel
            {
                VentaId = detalleVentas.VentaId,
                Precio = detalleVentas.Precio,
                DetalleVentasId=detalleVentas.DetalleVentasId,
                GetProductos = db.Productos.ToList()
            });
            }
            else { return RedirectToAction("Login", "Ventas", new { }); }
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



                detalleVentas = ToProducto(detalleVentas);

                if (detalleVentas.Cantidad > 0)
                {
                    db.Entry(detalleVentas).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = detalleVentas.VentaId });
                }
                else
                {

                    var CantidadActual = db.Productos.Where(q => q.ProductoId == detalleVentas.ProductoId).FirstOrDefault().Cantidad;

                    ViewBag.Mensaje = "No Cuenta Con Esa Cantidad de Articulos, De Ese Producto Tienes en Inventario " + CantidadActual;

                    return RedirectToAction("EditDetalleVenta", new { id = detalleVentas.DetalleVentasId, Mensaje = ViewBag.Mensaje });
                }
            
            }
            ViewBag.VentaId = new SelectList(db.Ventas, "VentaId", "VentaId", detalleVentas.VentaId);
            return View(detalleVentas);
        }

        // GET: DetalleVentas/Delete/5
        public async Task<ActionResult> DeleteDetalleVenta(int? id)
        {

            if (Session["UsuarioLogin"] != null)
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
        }else{ return RedirectToAction("Login", "Ventas", new { }); }
        }

        // POST: DetalleVentas/Delete/5
        [HttpPost, ActionName("DeleteDetalleVenta")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedDetalleVenta(int id)
        {
            DetalleVentas detalleVentas = await db.DetalleVentas.FindAsync(id);
            db.DetalleVentas.Remove(detalleVentas);
            await db.SaveChangesAsync();
            return RedirectToAction("Details","Ventas", new { id = detalleVentas.VentaId });
        }
        #endregion


        #region Pagos

        public ActionResult CreatePago(int? id)
        {

            if (Session["UsuarioLogin"] != null)
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
            return PartialView(new PagoViewModel {VentaId=ventas.VentaId,ClienteId=ventas.ClienteId,TotalNeto=view.TotalNeto-ventas.CantidadPagada });
            }
            else { return RedirectToAction("Login", "Ventas", new { }); }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreatePago(PagoViewModel view)
        {
            if (ModelState.IsValid)
            {
                if (view.Abono==true)
                {
                    var pago = ToPago(view);
                    pago.Monto = view.Monto;
                    db.Pagos.Add(pago);
                    await db.SaveChangesAsync();

                    Ventas venta = db.Ventas.Find(view.VentaId);

                    venta.CantidadPagada = venta.CantidadPagada+view.Monto;
                    venta.CantidadPendiente = venta.TotalOrden - venta.CantidadPagada;

                    db.Entry(venta).State = EntityState.Modified;
                    db.SaveChanges();
                } else
                {

               
               var pago= ToPago(view);
                    if (view.Vuelto >= 0)
                    {
                        pago.Monto -= view.Vuelto;
                        db.Pagos.Add(pago);
                        await db.SaveChangesAsync();

                        Ventas venta = db.Ventas.Find(view.VentaId);

                        venta.CantidadPagada = pago.Monto;
                        venta.CantidadPendiente = venta.TotalOrden - venta.CantidadPagada;

                        db.Entry(venta).State = EntityState.Modified;
                        db.SaveChanges();
                    }
        //            var Datos = (from v in db.Ventas
        //                         join dv in db.DetalleVentas on v.VentaId equals dv.VentaId
        //                         join pv
        //in db.Productos on dv.ProductoId equals pv.ProductoId
        //                         where v.VentaId == view.VentaId
        //                         select new { v.VentaId, v.Nombre, v.TotalOrden, v.Fecha, dv.Cantidad, dv.Descuento, dv.Precio, pv.Descripcion, pv.Talla }).ToList();



        //            LocalReport rdlc = new LocalReport();//importante
        //                                                 //rdlc.ReportPath = @"C:\Users\BRAINER\Documents\Proyectos2019\Tiendas\Tienda\Reportes\Report2.rdlc";//direccion absoluta del reporte, es muy importante a la hora de ponerlo en funcionamiento.
        //            rdlc.ReportPath = @"~..\..\Reportes\Report2.rdlc";                                                                                                  // si os da algun error puede ser por no encontrar la direccion exacta del reporte, creedme ya lo pase.
        //            rdlc.ReportEmbeddedResource = "Tienda.Reportes.Report1.rdlc";
        //            // las siguientes lineas son los datos que nesecito para mi reporte.
        //            // DataTable customer = CDetalleVenta.Mostrar((int)id_now);
        //            rdlc.DataSources.Add(new ReportDataSource("DataVentasDetalle", Datos));
        //            //DataTable venta2 = CVenta.MostrarID(id_now);
        //            //DataTable infomacion = CEmpresa.Mostrar();
        //            //DataTable cliente = CCliente.MostrarID(venta2.Rows[0]["idCliente"].ToString());
        //            //los parametros dento de mi report.rdlc
        //            //ReportParameter nombre = new ReportParameter("nombre_cliente", cliente.Rows[0]["nombre"].ToString());
        //            //ReportParameter fecha = new ReportParameter("fecha", venta2.Rows[0]["fecVenta"].ToString());
        //            //rdlc.SetParameters(new ReportParameter[] { nombre, fecha });
        //            // instancio un objeto dentro de la clase Impresor
        //            Impresor impresor = new Impresor();
        //            impresor.Imprime(rdlc);
                }
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

        //public  ActionResult CargarVentas()
        //{
        //    DateTime FechaInicial = DateTime.Today;
        //    DateTime FechaFinal = DateTime.Today.AddHours(23).AddMinutes(59);


        //    ViewBag.FechaInicial = FechaInicial;
        //    ViewBag.FechaFinal = FechaFinal;
        //    return View();
        //}

        [HandleError]
        public async Task<ActionResult> CargarVentas(string sFechaInicial, string sFechaFinal)
        {

            if (Session["UsuarioLogin"] != null)
            {
                DateTime FechaInicial = DateTime.Today;
            DateTime FechaFinal = DateTime.Today.AddHours(23).AddMinutes(59);
            if (!string.IsNullOrEmpty(sFechaInicial))
            {
                    FechaInicial= DateTime.ParseExact(sFechaInicial,"dd/MM/yyyy",CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(sFechaFinal))
            {
                    FechaFinal= DateTime.ParseExact(sFechaFinal, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                FechaFinal = FechaFinal.AddHours(23).AddMinutes(59);

            }
            var facturas =  db.Ventas.Where(f => f.Fecha >= FechaInicial && f.Fecha <= FechaFinal).ToList();
            double totalBruto = 0;
            double totalEfectivo = 0;
            double totaltarjeta = 0;
            double totaltrans = 0;
                foreach (var item in facturas)
            {
                totalBruto = totalBruto + item.DetalleVentas.Sum(d => d.Subtotal);
                    foreach (var items in item.Pagos)
                    {
                        if (items.MedioPagoId==1) { totalEfectivo = totalEfectivo + items.Monto; }
                        if (items.MedioPagoId == 2) { totaltarjeta = totaltarjeta + items.Monto; }
                        if (items.MedioPagoId == 3) { totaltrans = totaltrans + items.Monto; }
                    } 

            }

            ViewBag.FechaInicial = FechaInicial;
            ViewBag.FechaFinal = FechaFinal;
            ViewBag.Total = totalBruto;
            ViewBag.TotalEfectivo = totalEfectivo;
            ViewBag.Totaltarjeta = totaltarjeta;
            ViewBag.Totaltrans = totaltrans;
                return View(facturas.OrderByDescending(f=>f.VentaId));
            }
            else { return RedirectToAction("Login", "Ventas", new { }); }
        }
   
  
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
