using System.Web.Mvc;

namespace SignalRChatRoom.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}
