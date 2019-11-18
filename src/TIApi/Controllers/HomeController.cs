using Microsoft.AspNetCore.Mvc;

namespace TIApi
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      return this.View();
    }
  }
}