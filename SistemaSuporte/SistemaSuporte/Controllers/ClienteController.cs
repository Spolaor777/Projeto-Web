using Microsoft.AspNetCore.Mvc;
using SistemaSuporte.Filters;

public class ClienteController : Controller {
    [UsuarioAutenticado]
    public IActionResult Dashboard() {
        

        return View();
    }
}

