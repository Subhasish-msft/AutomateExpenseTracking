using System.Web.Mvc;

namespace Automate.Expense.Tracking.Sample
{
    public class HomeController : Controller
    {
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("hello")]
        public ActionResult Hello()
        {
            return View("Index");
        }

        [Route("first")]
        public ActionResult First()
        {
            return View();
        }

        [Route("second")]
        public ActionResult Second()
        {
            return View();
        }

        [Route("configure")]
        public ActionResult Configure()
        {
            return View();
        }

        [Route("createexp")]
        public ActionResult CreateExp()
        {
            return View();
        }


        [Route("customform")]
        public ActionResult CustomForm()
        {
            return View();
        }
    }
}
