using AspNet.Identity.MongoDB;
using ByteBank.Forum.App_Start.Identity;
using ByteBank.Forum.MongoContext;
using ByteBank.Forum.MongoContext.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Owin;

//[assembly: OwinStartup(typeof(ByteBank.Forum.Startup))]
namespace ByteBank.Forum
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            builder.CreatePerOwinContext<IUserStore<Conta>>(() =>
            {
                DbContext dbContext = new DbContext();
                return new UserStore<Conta>(dbContext.GetCollectionConta());
            });

            builder.CreatePerOwinContext<UserManager<Conta>>((opcoes, contextOwin) =>
            {
                var context = contextOwin.Get<IUserStore<Conta>>();
                var userManager = new UserManager<Conta>(context);
                var userValidator = new UserValidator<Conta>(userManager);
                userValidator.RequireUniqueEmail = true;
                userManager.PasswordValidator = new App_Start.Identity.PasswordValidator()
                {
                    MinLengthPassword = 6,
                    SpecialCharacterRequired = true,
                    UpperCaseRequired = true,
                    LowerCaseRequired = true,
                    DigitsRequired = true
                };
                userManager.UserValidator = userValidator;
                userManager.EmailService = new EmailService();
                var protectionProvider = opcoes.DataProtectionProvider;
                var protectionProviderCreated = protectionProvider.Create("ByteBank.Forum");
                userManager.UserTokenProvider = new DataProtectorTokenProvider<Conta>(protectionProviderCreated);
                return userManager;
            });

            builder.CreatePerOwinContext<SignInManager<Conta, string>>((opcoes, contextOwin) =>
            {
                var userManager = contextOwin.Get<UserManager<Conta>>();
                var signInManager = new SignInManager<Conta, string>(userManager, contextOwin.Authentication);
                return signInManager;
            });
        }
    }
}