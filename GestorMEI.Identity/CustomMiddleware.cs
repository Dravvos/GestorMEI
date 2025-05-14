namespace GestorMEI.Identity
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var isDevelopment = context.RequestServices.GetService<IHostEnvironment>().IsDevelopment();

            if (isDevelopment)
            {
                // Rewrite URL for development
                context.Request.PathBase = "/";
            }
            else
            {
                // Rewrite URL for production
                context.Request.PathBase = "/MEICaixa.Identity";
            }

            await _next(context);
        }
    }
}
