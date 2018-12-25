using System.Web.Mvc;

namespace METU.VRS.UI
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new FormsAuthAttribute());
        }
    }
}
