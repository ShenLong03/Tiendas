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

                var productos = db.Productos.Include(p => p.DetalleVentas).Include(c=>c.Categorias).OrderByDescending(q => q.ProductoId);
                return View(await productos.ToListAsync());
            }
            else { return RedirectToAction("Login", "Ventas", new { }); }
        }

        public JsonResult BuscarPrefijo(int CategoriaId)
        {


            db.Configuration.ProxyCreationEnabled = false;
            var Categoria = db.Categorias.Find(CategoriaId);





            return Json(Categoria.Prefijo, JsonRequestBehavior.AllowGet);




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
                return View(new ProductosViewModel
                {
                    
                    GetCategorias = db.Categorias.ToList()
                });
               
            }
            else { return RedirectToAction("Login", "Ventas", new { }); }
        }

        // POST: Productos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.

        [HandleError]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductosViewModel productos)
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
                var detalleProducto = ToViewProducto(productos);
                db.Productos.Add(detalleProducto);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

           
            return View(productos);
        }
        private ProductosViewModel ToProductoView(Productos productos)
        {
            return new ProductosViewModel
            {
                CategoriaId = productos.CategoriaId,
                Cantidad = productos.Cantidad,
                CodigoId = productos.CodigoId,
                Descripcion = productos.Descripcion,
                Precio = productos.Precio,
                ProductoId = productos.ProductoId,
                Foto = productos.Foto,
                Talla=productos.Talla,
                

            };


        }

        private Productos ToViewProducto(ProductosViewModel productos)
        {
            return new Productos
            {
                CategoriaId = productos.CategoriaId,
                Cantidad = productos.Cantidad,
                CodigoId = productos.CodigoId,
                Descripcion = productos.Descripcion,
                Precio = productos.Precio,
                ProductoId = productos.ProductoId,
                Foto = productos.Foto,
                Talla = productos.Talla,


            };


        }
        // GET: Productos/Edit/5
        [HandleError]
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
                var detalleProducto = ToProductoView(productos);
                detalleProducto.GetCategorias = db.Categorias.ToList();
                return View(detalleProducto);
            }
            else { return RedirectToAction("Login", "Ventas", new { }); }
        }

        // POST: Productos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HandleError]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductosViewModel productos)
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

                var detalleProducto = ToViewProducto(productos);

                db.Entry(detalleProducto).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
           
            return View();
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
