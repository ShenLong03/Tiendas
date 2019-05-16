using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Domain;
using Tienda.Models;

namespace Tienda.Controllers
{
    public class CategoriasController : Controller
    {
        private DataContextLocal db = new DataContextLocal();

        // GET: Categorias
        public ActionResult Index()
        {
            if (Session["UsuarioLogin"] != null)
            {
                return View(db.Categorias.ToList());
            }
            else { return RedirectToAction("Login", "Ventas", new { }); }
        }

        // GET: Categorias/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UsuarioLogin"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Categorias categorias = db.Categorias.Find(id);
                if (categorias == null)
                {
                    return HttpNotFound();
                }
                return View(categorias);
            }
            else { return RedirectToAction("Login", "Ventas", new { }); }
        }

        // GET: Categorias/Create
        public ActionResult Create()
        {
            if (Session["UsuarioLogin"] != null)
            {
                return View();
            }
            else { return RedirectToAction("Login", "Ventas", new { }); }
        }

        // POST: Categorias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Categorias categorias)
        {
            if (ModelState.IsValid)
            {
                db.Categorias.Add(categorias);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(categorias);
        }

        // GET: Categorias/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UsuarioLogin"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Categorias categorias = db.Categorias.Find(id);
                if (categorias == null)
                {
                    return HttpNotFound();
                }
                return View(categorias);
            }
            else { return RedirectToAction("Login", "Ventas", new { }); }
        }

        // POST: Categorias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Categorias categorias)
        {
            if (ModelState.IsValid)
            {
                db.Entry(categorias).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(categorias);
        }

        // GET: Categorias/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UsuarioLogin"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Categorias categorias = db.Categorias.Find(id);
                if (categorias == null)
                {
                    return HttpNotFound();
                }
                return View(categorias);
            }
            else { return RedirectToAction("Login", "Ventas", new { }); }
        }

        // POST: Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Categorias categorias = db.Categorias.Find(id);
            db.Categorias.Remove(categorias);
            db.SaveChanges();
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
