using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SistemaSuporte.Filters {
    public class UsuarioAutenticado : ActionFilterAttribute {
        public override void OnActionExecuting(ActionExecutingContext context) {
            var http = context.HttpContext;

            if (http.Session.GetInt32("UsuarioId") == null) {
                context.Result = new RedirectToActionResult("Index", "Login", null);
            }

            base.OnActionExecuting(context);
        }
    }
}

