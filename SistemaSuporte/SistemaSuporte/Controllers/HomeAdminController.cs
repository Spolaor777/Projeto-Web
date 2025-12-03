using Microsoft.AspNetCore.Mvc;

public class HomeAdminController : Controller {
    public IActionResult Index() {
        return View();
    }
}

