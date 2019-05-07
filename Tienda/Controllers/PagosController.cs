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
using Tienda.Models;

namespace Tienda.Controllers
{
    public class PagosController : Controller
    {
        private DataContextLocal db = new DataContextLocal();

        // GET: Pagos
        public async Task<ActionResult> Index()
        {
            var pagos = db.Pagos.Include(p => p.Cliente).Include(p => p.MediosPago).Include(p => p.Ventas);
            return View(await pagos.ToListAsync());
        }

        // GET: Pagos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pagos pagos = await db.Pagos.FindAsync(id);
            if (pagos == null)
            {
                return HttpNotFound();
            }
            return View(pagos);
        }

        // GET: Pagos/Create
        public ActionResult Create()
        {
            ViewBag.ClienteId = new SelectList(db.Clientes, "ClienteId", "Nombre");
            ViewBag.MedioPagoId = new SelectList(db.MediosPago, "MedioPagoId", "FormaPago");
            ViewBag.VentaId = new SelectList(db.Ventas, "VentaId", "Nombre");
            return View();
        }

        // POST: Pagos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Pagos pagos)
        {
            if (ModelState.IsValid)
            {
                db.Pagos.Add(pagos);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ClienteId = new SelectList(db.Clientes, "ClienteId", "Nombre", pagos.ClienteId);
            ViewBag.MedioPagoId = new SelectList(db.MediosPago, "MedioPagoId", "FormaPago", pagos.MedioPagoId);
            ViewBag.VentaId = new SelectList(db.Ventas, "VentaId", "Nombre", pagos.VentaId);
            return View(pagos);
        }

        // GET: Pagos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pagos pagos = await db.Pagos.FindAsync(id);
            if (pagos == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClienteId = new SelectList(db.Clientes, "ClienteId", "Nombre", pagos.ClienteId);
            ViewBag.MedioPagoId = new SelectList(db.MediosPago, "MedioPagoId", "FormaPago", pagos.MedioPagoId);
            ViewBag.VentaId = new SelectList(db.Ventas, "VentaId", "Nombre", pagos.VentaId);
            return View(pagos);
        }

        // POST: Pagos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PagoId,Fecha,Monto,ClienteId,MedioPagoId,VentaId")] Pagos pagos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pagos).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ClienteId = new SelectList(db.Clientes, "ClienteId", "Nombre", pagos.ClienteId);
            ViewBag.MedioPagoId = new SelectList(db.MediosPago, "MedioPagoId", "FormaPago", pagos.MedioPagoId);
            ViewBag.VentaId = new SelectList(db.Ventas, "VentaId", "Nombre", pagos.VentaId);
            return View(pagos);
        }

        // GET: Pagos/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pagos pagos = await db.Pagos.FindAsync(id);
            if (pagos == null)
            {
                return HttpNotFound();
            }
            return View(pagos);
        }

        // POST: Pagos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Pagos pagos = await db.Pagos.FindAsync(id);
            db.Pagos.Remove(pagos);
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
