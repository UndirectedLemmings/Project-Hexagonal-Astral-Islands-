using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project_Hexagonal_Astral_Islands.Models;
using FluentNHibernate;
using NHibernate;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project_Hexagonal_Astral_Islands.Controllers
{
    public class MapController : Controller
    {
    private readonly UserManager<ApplicationUser> _userManager;

    public MapController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }


        // GET: /Map/
        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            
            //Map map;
            //using (ISession session = NHibernateHelper.OpenSession("LOW")){ 

            //var maps = session.Query<Map>();

            //try
            //{
            //    map = maps.Select(u => u).Where(u => u.ID == user.my_map_id).First();
            //}
            //catch (InvalidOperationException) {
            //    map = null;
            //}
            //if (map == null)
            //{
            //    map = new Map();
            //    map.GenerateNew(session,5);
            //    user.my_map_id = (int)session.Save(map);
            //}
            //map = maps.Select(u => u).Where(u => u.ID == user.my_map_id).First();
            //    foreach (Unit u in map.AllUnits) {
            //        u.RetreiveStatsFromType();
            //    }

            //    /*foreach (var kvp in map.Hcd)
            //    {
            //        result = result + kvp.Key.ToString() + ": " + kvp.Value.ToString() + "\n";
            //    }*/

                
            //}

            /* 
             string result = "Пользователь не обнаружен";
             if (user != null) {
                 if (user.personal_map == null) {
                     user.personal_map = new Map();
                 }

             }*/
            ViewData["MapId"] = user.my_map_id;
            KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> k = new KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> ( "Cache-Control",  "private, max-age=0, must-revalidate");

            HttpContext.Response.Headers.Add(k);
            return View();
        }


        public ActionResult Image(int? id) {
            //Map map;
            //using (ISession session = NHibernateHelper.OpenSession("LOW"))
            //{

            //    var maps = session.Query<Map>();


            //    map = maps.Select(u => u).Where(u => u.ID == id).First();
            //    foreach (Unit u in map.AllUnits)
            //    {
            //        u.RetreiveStatsFromType();
            //    }

            /*foreach (var kvp in map.Hcd)
            {
                result = result + kvp.Key.ToString() + ": " + kvp.Value.ToString() + "\n";
            }*/


            // }
            Response.Headers.Add("Refresh",$"{Constants.MapUpdateInterval}");
            try
            {
                FileStreamResult streamResult = new FileStreamResult(new System.IO.FileStream($"./wwwroot/generatedmaps/map{id}.png", System.IO.FileMode.Open, System.IO.FileAccess.Read,System.IO.FileShare.ReadWrite), "image/png");
                return streamResult;
            }
            catch {
                return new FileStreamResult(new System.IO.FileStream($"./wwwroot/generatedmaps/map{id}.png", System.IO.FileMode.OpenOrCreate), "image/png"); ;
            }
        }
           
    }
}
