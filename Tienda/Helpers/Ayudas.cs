using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tienda.Models;

namespace Tienda.Helpers
{
    public class Ayudas : IDisposable
    {
        DataContextLocal db = new DataContextLocal();

        public Ayudas()
        {
        }

        public static void CheckMediosPagos()
        {
            try
            {
                if (!FindByName("Efectivo"))
                {
                    Add(1, "Efectivo");
                }
                if (!FindByName("Tarjeta"))
                {
                    Add(2, "Tarjeta");
                }
             
            }
            catch (Exception)
            {

            }
        }

        //public static void CheckTallas()
        //{
        //    try
        //    {
        //        if (!FindByNameTallas("XS"))
        //        {
        //            AddTallas(1, "XS");
        //        }
        //        if (!FindByNameTallas("S"))
        //        {
        //            AddTallas(2, "S");
        //        }
        //        if (!FindByNameTallas("M"))
        //        {
        //            AddTallas(3, "M");
        //        }
        //        if (!FindByNameTallas("L"))
        //        {
        //            AddTallas(4, "L");
        //        }
        //        if (!FindByNameTallas("XL"))
        //        {
        //            AddTallas(5, "XL");
        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }
        //}

        public static void Add(int id, string nombre)
        {
            try
            {
                using (DataContextLocal db = new DataContextLocal())
                {
                    var mediospago = new MediosPago
                    {
                        MedioPagoId = id,
                        FormaPago = nombre,
                        
                    };

                    db.MediosPago.Add(mediospago);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        //public static void AddTallas(int id, string nombre)
        //{
        //    try
        //    {
        //        using (DataContextLocal db = new DataContextLocal())
        //        {
        //            var talla = new Tallas
        //            {
        //                TallaId = id,
        //                Abreviatura = nombre,

        //            };

        //            db.Tallas.Add(talla);
        //            db.SaveChanges();
        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        public static bool FindByName(string nombre)
        {
            try
            {
                using (DataContextLocal db = new DataContextLocal())
                {
                    if (db.MediosPago.Where(e => e.FormaPago == nombre).Count() > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        //public static bool FindByNameTallas(string nombre)
        //{
        //    try
        //    {
        //        using (DataContextLocal db = new DataContextLocal())
        //        {
        //            if (db.Tallas.Where(e => e.Abreviatura == nombre).Count() > 0)
        //            {
        //                return true;
        //            }
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}

        public void Dispose()
        {
            db.Dispose();
        }
    }
}

