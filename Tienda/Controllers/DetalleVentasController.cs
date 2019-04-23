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

namespace Tienda.Controllers
{
    public class DetalleVentasController : Controller
    {
        private DataContext db = new DataContext();

        // GET: DetalleVentas
        public async Task<ActionResult> Index()
        {
            var detalleVentas = db.DetalleVentas.Include(d => d.Ventas);
            return View(await detalleVentas.ToListAsync());
        }

        // GET: DetalleVentas/Details/5
        public async Task<ActionResult> Details(int? id)
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
        public ActionResult Create()
        {
            ViewBag.VentaId = new SelectList(db.Ventas, "VentaId", "VentaId");
            return View();
        }

        // POST: DetalleVentas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DetalleVentasId,Precio,Cantidad,Descuento,Subtotal,ProductoId,VentaId")] DetalleVentas detalleVentas)
        {
            if (ModelState.IsValid)
            {
                db.DetalleVentas.Add(detalleVentas);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.VentaId = new SelectList(db.Ventas, "VentaId", "VentaId", detalleVentas.VentaId);
            return View(detalleVentas);
        }

        // GET: DetalleVentas/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
        public async Task<ActionResult> Edit([Bind(Include = "DetalleVentasId,Precio,Cantidad,Descuento,Subtotal,ProductoId,VentaId")] DetalleVentas detalleVentas)
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
        public async Task<ActionResult> Delete(int? id)
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
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            DetalleVentas detalleVentas = await db.DetalleVentas.FindAsync(id);
            db.DetalleVentas.Remove(detalleVentas);
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
