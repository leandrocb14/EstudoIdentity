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
                if (_userManager == null)
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
        public async Task<ActionResult> Registrar(ContaRegistrarViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = await UserManager.FindByEmailAsync(model.Email);

                if (usuario != null)
                    return RedirectToAction("AguardandoConfirmacao");
                else
                {
                    Conta conta = new Conta()
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        NomeCompleto = model.NomeCompleto
                    };
                    var result = await UserManager.CreateAsync(conta, model.Senha);

                    if (result.Succeeded)
                    {
                        await EnviarEmailConfirmacao(conta);
                        return RedirectToAction("AguardandoConfirmacao");
                    }
                    else
                        AdicionaErros(result);
                }
            }
            return View();
        }

        public ActionResult AguardandoConfirmacao()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        public async Task<ActionResult> ConfirmacaoEmail(string usuarioId, string token)
        {
            if (string.IsNullOrEmpty(usuarioId) || string.IsNullOrEmpty(token))
                return View("Error");

            var result = await UserManager.ConfirmEmailAsync(usuarioId, token);

            if (result.Succeeded)            
                return RedirectToAction("Index", "Home");            
            else            
                return View("Error");            
        }

        private async Task EnviarEmailConfirmacao(Conta conta)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(conta.Id);

            var linkCallBack = Url.Action("ConfirmacaoEmail", "Conta", new { usuarioId = conta.Id, token = token }, Request.Url.Scheme);

            await UserManager.SendEmailAsync(conta.Id, "Fórum ByteBank - Confirmação de E-mail", $"Bem vindo ao Fórum ByteBank! Clique aqui para confirmar o seu email {linkCallBack}.");
        }

        private void AdicionaErros(IdentityResult result)
        {
            foreach (var erro in result.Errors)
            {
                ModelState.AddModelError("", erro);
            }
        }
    }
}