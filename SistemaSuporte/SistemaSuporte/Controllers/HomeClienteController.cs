using Microsoft.AspNetCore.Mvc;

public class HomeClienteController : Controller {
    public IActionResult Index() {
        return View();
    }
}

