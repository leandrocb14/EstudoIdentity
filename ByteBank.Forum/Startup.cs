using AspNet.Identity.MongoDB;
using ByteBank.Forum.MongoContext;
using ByteBank.Forum.MongoContext.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ByteBank.Forum.Startup))]
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
                return new UserManager<Conta>(context);
            });
        }
    }
}