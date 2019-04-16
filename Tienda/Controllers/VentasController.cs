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

namespace Tienda.Controllers
{
    public class VentasController : Controller
    {
        private DataContext db = new DataContext();

        // GET: Ventas
        public async Task<ActionResult> Index()
        {
            Ayudas.CheckMediosPagos();
            var ventas = db.Ventas.Include(v => v.Cliente);
            return View(await ventas.ToListAsync());
        }
        public ActionResult getarticulo(int id)
        {


            using (DataContext db = new DataContext())
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
            return View(ventas);
        }

        // GET: Ventas/Create
        public ActionResult Create()
        {
            ViewBag.ClienteId = new SelectList(db.Clientes, "ClienteId", "Nombre");
            ViewBag.MedioPagoId = new SelectList(db.MediosPago, "MedioPagoId", "FormaPago");
            return View();
        }

        // POST: Ventas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "VentaId,Fecha,TotalOrden,ClienteId,MedioPagoId")] Ventas ventas)
        {
            if (ModelState.IsValid)
            {
                db.Ventas.Add(ventas);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
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
        public async Task<ActionResult> Edit([Bind(Include = "VentaId,Fecha,TotalOrden,ClienteId,MedioPagoId")] Ventas ventas)
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
