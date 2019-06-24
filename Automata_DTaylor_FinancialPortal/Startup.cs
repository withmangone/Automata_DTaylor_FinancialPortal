using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Automata_DTaylor_FinancialPortal.Startup))]
namespace Automata_DTaylor_FinancialPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
