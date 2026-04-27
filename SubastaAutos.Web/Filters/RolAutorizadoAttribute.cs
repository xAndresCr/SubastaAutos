using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SubastaAutos.Web.Filters
{
    public class RolAutorizadoAttribute : ActionFilterAttribute
    {
        private readonly int[] _rolesPermitidos;

        public RolAutorizadoAttribute(params int[] rolesPermitidos)
        {
            _rolesPermitidos = rolesPermitidos;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var usuarioId = session.GetInt32("UsuarioId");
            var rolUsuario = session.GetInt32("UsuarioRol");

            //este redirige porque sino cualquier vivaso se pasa el login
            if(usuarioId == null || usuarioId == 0)
            {
                context.Result = new RedirectToActionResult("LogIn", "Login", null);
                return;
            }

            //Con sesión pero sin rol permitido → acceso denegado
            if (_rolesPermitidos.Length > 0 && !_rolesPermitidos.Contains(rolUsuario ?? 0))
            {
                context.Result = new RedirectToActionResult("Denegado", "Home", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
