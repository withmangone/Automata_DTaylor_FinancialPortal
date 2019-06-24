using System.Web;
using System.Web.Mvc;

namespace Automata_DTaylor_FinancialPortal
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
