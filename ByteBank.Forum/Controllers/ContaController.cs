using AspNet.Identity.MongoDB;
using ByteBank.Forum.MongoContext;
using ByteBank.Forum.MongoContext.DAO;
using ByteBank.Forum.MongoContext.Model;
using ByteBank.Forum.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ByteBank.Forum.Controllers
{
    public class ContaController : Controller
    {
        private UserManager<Conta> _userManager;

        public UserManager<Conta> UserManager
        {
            get
            {
                if(_userManager == null)                
                    _userManager = HttpContext.GetOwinContext().GetUserManager<UserManager<Conta>>();
                
                return _userManager;
            }
            set { _userManager = value; }
        }

        // GET: Conta
        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrar(ContaRegistrarViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = UserManager.FindByEmail(model.Email);

                if(usuario != null)                
                    return RedirectToAction("Index", "Home");                
                else
                {
                    var result = UserManager.Create(new Conta()
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        NomeCompleto = model.NomeCompleto
                    }, model.Senha);

                    if (result.Succeeded)
                        return RedirectToAction("Index", "Home");
                    else
                        AdicionaErros(result);
                }                
            }
            return View();
        }

        private void AdicionaErros(IdentityResult result)
        {
            foreach(var erro in result.Errors)
            {
                ModelState.AddModelError("", erro);
            }
        }
    }
}