using METU.VRS.Services;
using System.Web.Mvc;

namespace METU.VRS.Controllers.Base
{
    public class ControllerBase : Controller
    {
        private readonly DatabaseContext db = null;
        public ControllerBase()
        {

        }

        public ControllerBase(DatabaseContext db)
        {
            //for mock/stub
            this.db = db;
        }

        protected DatabaseContext GetNewDBContext()
        {
            if (db == null)
            {
                return new DatabaseContext();
            }
            else
            {
                return db;
            }
        }

    }
}