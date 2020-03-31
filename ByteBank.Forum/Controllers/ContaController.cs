using ByteBank.Forum.MongoContext.Model;
using ByteBank.Forum.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
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

        private SignInManager<Conta, string> _signInManager;

        public SignInManager<Conta, string> SignInManager
        {
            get
            {
                if (_signInManager == null)
                    _signInManager = HttpContext.GetOwinContext().GetUserManager<SignInManager<Conta, string>>();

                return _signInManager;
            }
            set { _signInManager = value; }
        }

        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return Request.GetOwinContext().Authentication;
            }
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

        public async Task<ActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginPorAutenticacaoExterna(string provider)
        {
            SignInManager.AuthenticationManager.Challenge(new AuthenticationProperties
            {
                RedirectUri = Url.Action("LoginPorAutenticacaoExternaCallback")
            }, provider);
            return new HttpUnauthorizedResult();
        }

        public async Task<ActionResult> LoginPorAutenticacaoExternaCallback()
        {
            var loginInfo = await SignInManager.AuthenticationManager.GetExternalLoginInfoAsync();

            if (loginInfo != null)
            {
                var signInResultado = await SignInManager.ExternalSignInAsync(loginInfo, true);

                if (signInResultado == SignInStatus.Success)
                    return RedirectToAction("Index", "Home");
            }

            return View("Error");
        }

        [HttpPost]
        public async Task<ActionResult> Login(ContaLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = await UserManager.FindByEmailAsync(model.Email);

                if (usuario == null)
                    return SenhaOuUsuarioInvalidos();

                var signInResult = await SignInManager.PasswordSignInAsync(usuario.UserName, model.Senha, model.ContinuarLogado, true);
                switch (signInResult)
                {
                    case SignInStatus.Success:
                        if (!usuario.EmailConfirmed)
                        {
                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            return View("AguardandoConfirmacao");
                        }

                        return RedirectToAction("Index", "Home");
                    case SignInStatus.LockedOut:
                        var senhaCorreta = await UserManager.CheckPasswordAsync(usuario, model.Senha);
                        if (senhaCorreta)
                            ModelState.AddModelError("", "A conta se encontra bloqueada!");
                        else
                            return SenhaOuUsuarioInvalidos();
                        break;
                    default:
                        return SenhaOuUsuarioInvalidos();
                }
            }
            return View(model);
        }

        public ActionResult EsqueciSenha()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> EsqueciSenhaAsync(EsqueciSenhaViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = await UserManager.FindByEmailAsync(model.Email);

                if (usuario != null)
                {
                    var token = await UserManager.GeneratePasswordResetTokenAsync(usuario.Id);

                    var linkCallBack = Url.Action("ConfirmacaoAlteracaoSenha", "Conta", new { usuarioId = usuario.Id, token = token }, Request.Url.Scheme);

                    await UserManager.SendEmailAsync(usuario.Id, "Fórum ByteBank - Alteração de senha", $"Bem vindo ao Fórum ByteBank! Clique aqui para alterar a sua senha {linkCallBack}.");

                }
                return View("EmailAlteracaoSenhaEnviado");
            }

            return View();
        }

        public ActionResult ConfirmacaoAlteracaoSenha(string usuarioId, string token)
        {
            var model = new ContaConfirmacaoAlteracaoSenhaViewModel()
            {
                UsuarioId = usuarioId,
                Token = token
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ConfirmacaoAlteracaoSenha(ContaConfirmacaoAlteracaoSenhaViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.ResetPasswordAsync(model.UsuarioId, model.Token, model.NovaSenha);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                AdicionaErros(result);
            }
            return View();
        }

        [HttpPost]
        public ActionResult Logoff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Conta");
        }

        private ActionResult SenhaOuUsuarioInvalidos()
        {
            ModelState.AddModelError("", "Credências inválida.");
            return View();
        }

        [HttpPost]
        public ActionResult RegistrarPorAutenticacaoExterna(string provider)
        {
            SignInManager.AuthenticationManager.Challenge(new AuthenticationProperties
            {
                RedirectUri = Url.Action("RegistrarPorAutenticacaoExternaCallback")
            }, provider);
            return new HttpUnauthorizedResult();
        }

        public async Task<ActionResult> RegistrarPorAutenticacaoExternaCallback()
        {
            var loginInfo = await SignInManager.AuthenticationManager.GetExternalLoginInfoAsync();

            if (loginInfo != null)
            {
                var usuarioExistente = await UserManager.FindByEmailAsync(loginInfo.Email);

                if (usuarioExistente != null)
                    return View("Error");

                var novoUsuarioAplicacao = new Conta()
                {
                    Email = loginInfo.Email,
                    UserName = loginInfo.Email,
                    NomeCompleto = loginInfo.ExternalIdentity.FindFirstValue(loginInfo.ExternalIdentity.NameClaimType)
                };

                var resultado = await UserManager.CreateAsync(novoUsuarioAplicacao);

                if (resultado.Succeeded)
                {
                    var resultadoAddLoginInfo = await UserManager.AddLoginAsync(novoUsuarioAplicacao.Id, loginInfo.Login);

                    if (resultadoAddLoginInfo.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

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