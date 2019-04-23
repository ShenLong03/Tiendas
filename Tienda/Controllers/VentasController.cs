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

namespace Tienda.Controllers
{
    public class VentasController : Controller
    {
        private DataContextLocal db = new DataContextLocal();

        // GET: Ventas
        public async Task<ActionResult> Index()
        {
            Ayudas.CheckMediosPagos();
            var ventas = db.Ventas.Include(v => v.Cliente);
            return View(await ventas.ToListAsync());
        }
        [HttpPost]
        public JsonResult getarticulo(int id)
        {


          
         
                var vArticulo = db.Productos.Where(e => e.ProductoId==id);
                List<Productos> vData = new List<Productos>();
                Productos vEmpleadoView = new Productos();
                foreach (var item in vArticulo)
                {
                    vEmpleadoView = new Productos()
                    {
                        CodigoId = item.CodigoId,
                        Descripcion = item.Descripcion,
                        Precio = item.Precio,
                      
                    };
                    vData.Add(vEmpleadoView);
                }



                return Json(new { data = vData.ToList() }, JsonRequestBehavior.AllowGet);
            

        }

        public  JsonResult getarticulos(int id)
        {


          
                var vArticulo = db.Productos.Where(e => e.CodigoId == id).ToList();
              
               

                return Json(new { data = vArticulo.ToList() }, JsonRequestBehavior.AllowGet);
         

        }
        // GET: Ventas/Details/5
        public async Task<ActionResult> Details(int? id)
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




        [HttpPost, ActionName("Details")]
        public async Task<ActionResult> AgregarProducto(int? codigoId,int? ventaId)
        {
            if (codigoId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Productos productos = await db.Productos.Where(q=>q.CodigoId==codigoId).FirstAsync();
            if (productos == null)
            {
                return HttpNotFound();
            }

            if (ventaId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ventas ventas = await db.Ventas.FindAsync(ventaId);
            if (ventas == null)
            {
                return HttpNotFound();
            }

            DetalleVentas detalle = new DetalleVentas { };

            



            return View(productos);
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

        #region DetalleVenta

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
        public  ActionResult CreateDetalleVenta(int? id)
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
            return View(new DetalleVentaViewModel{
                VentaId=ventas.VentaId,
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
        public async Task<ActionResult> CreateDetalleVenta(DetalleVentas detalleVentas)
        {
            if (ModelState.IsValid)
            {
                detalleVentas = ToProducto(detalleVentas);

                if (detalleVentas.Cantidad > 0)
                {
                    db.DetalleVentas.Add(detalleVentas);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = detalleVentas.VentaId });
                }
                else {

                    VentaViewModel model = new VentaViewModel();
                 model.Mensaje = "No cuentas Con Esa Cantidad de Prendas en Inventario";
                    return RedirectToAction("Details", new { id = detalleVentas.VentaId});
                }
            }

            ViewBag.ProductoId = new SelectList(db.Productos, "ProductoId", "Descripcion",detalleVentas.ProductoId);
            detalleVentas.Ventas = await db.Ventas.FindAsync(detalleVentas.VentaId);
            return View(detalleVentas);
        }

        private DetalleVentas ToProducto(DetalleVentas detalleVentas)
        {

            try
            {
               var Producto= db.Productos.Where(p => p.CodigoId == detalleVentas.ProductoId).First();
                detalleVentas.ProductoId = Producto.ProductoId;
                detalleVentas.Precio = Producto.Precio;
                if ((Producto.Cantidad-detalleVentas.Cantidad) >= 1)
                {

                  

                    Producto.Cantidad = Producto.Cantidad - detalleVentas.Cantidad;
                    db.Entry(Producto).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    ViewBag.mensaje = "No cuentas Con Esa Cantidad de Prendas en Inventario";
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
