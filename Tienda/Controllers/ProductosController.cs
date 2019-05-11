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
    public class ProductosController : Controller
    {
        private DataContextLocal db = new DataContextLocal();

        // GET: Productos
        public async Task<ActionResult> Index()
        {
            if (Session["UsuarioLogin"] != null)
            {

                var productos = db.Productos.Include(p => p.DetalleVentas);
                return View(await productos.ToListAsync());
            }
            else { return RedirectToAction("Login", "Ventas", new { }); }
        }

        // GET: Productos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (Session["UsuarioLogin"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Productos productos = await db.Productos.FindAsync(id);
                if (productos == null)
                {
                    return HttpNotFound();
                }
                return View(productos);
            }
            else { return RedirectToAction("Login", "Ventas", new { }); }
        }

        // GET: Productos/Create
        public ActionResult Create()
        {

            if (Session["UsuarioLogin"] != null)
            {

                return View();
            }
            else { return RedirectToAction("Login", "Ventas", new { }); }
        }

        // POST: Productos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create( Productos productos)
        {
            if (ModelState.IsValid)
            {
                var pic = string.Empty;
                var folder = "~/Content/Productos";

                if (productos.FotoFile != null)
                {
                    pic = FilesHelper.UploadPhoto(productos.FotoFile, folder);
                    pic = string.Format("{0}/{1}", folder, pic);
                }

                productos.Foto = pic;
                //Productos producto = new Productos();
                
                //producto.Foto = pic;
                db.Productos.Add(productos);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

           
            return View(productos);
        }

        // GET: Productos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (Session["UsuarioLogin"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Productos productos = await db.Productos.FindAsync(id);
                if (productos == null)
                {
                    return HttpNotFound();
                }

                return View(productos);
            }
            else { return RedirectToAction("Login", "Ventas", new { }); }
        }

        // POST: Productos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Productos productos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productos).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
           
            return View(productos);
        }

        // GET: Productos/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {

            if (Session["UsuarioLogin"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Productos productos = await db.Productos.FindAsync(id);
                if (productos == null)
                {
                    return HttpNotFound();
                }
                return View(productos);
            }
            else { return RedirectToAction("Login", "Ventas", new { }); }
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Productos productos = await db.Productos.FindAsync(id);
            db.Productos.Remove(productos);
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
