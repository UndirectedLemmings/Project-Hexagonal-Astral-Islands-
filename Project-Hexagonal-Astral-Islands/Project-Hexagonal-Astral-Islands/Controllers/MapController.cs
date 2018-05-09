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
            string result = "";
            Map map;
            using (ISession session = NHibernateHelper.OpenSession()){ 

            var maps = session.Query<Map>();

            try
            {
                map = maps.Select(u => u).Where(u => u.ID == user.my_map_id).First();
            }
            catch (InvalidOperationException) {
                map = null;
            }
            if (map == null)
            {
                map = new Map();
                map.GenerateNew(session);
                user.my_map_id = (int)session.Save(map);
            }
            map = maps.Select(u => u).Where(u => u.ID == user.my_map_id).First();

                /*foreach (var kvp in map.Hcd)
                {
                    result = result + kvp.Key.ToString() + ": " + kvp.Value.ToString() + "\n";
                }*/

                ViewData["MapLoc"] = ImageGen.GenerateImage(map);
            }

            /* 
             string result = "Пользователь не обнаружен";
             if (user != null) {
                 if (user.personal_map == null) {
                     user.personal_map = new Map();
                 }

             }*/
            return View();
        }

           
    }
}
