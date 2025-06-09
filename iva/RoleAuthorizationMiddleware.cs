namespace iva
{
    public class RoleAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        //public async Task InvokeAsync(HttpContext context)
        //{
        //    //// Fetch the user role from session
        //    //var userRole = context.Session.GetString("userRole");
        //    //var currentController = context.Request.RouteValues["controller"]?.ToString();

        //    //// Authorization logic: If the user is an admin and tries to access "Device" controller
        //    //if (userRole == "admin" && currentController == "Device")
        //    //{
        //    //    // Redirect if unauthorized
        //    //    context.Response.Redirect("/Home/AccessDenied");
        //    //    return;
        //    //}
        //    //await _next(context);

        //}
        public async Task InvokeAsync(HttpContext context)
        {
            var userRole = context.Session.GetString("userRole");
            var currentController = context.Request.RouteValues["controller"]?.ToString();

            // Check if the user role is 'admin' and trying to access the 'Device' controller
            if (userRole == "admin" && currentController == "Device")
            {
                // Set the response status code to 403 (Forbidden)
                context.Response.StatusCode = 403; 

                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(@"
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Access Denied</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f8f9fa;
            margin: 0;
            padding: 0;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
        }
        .container {
            text-align: center;
            background-color: #ffffff;
            border-radius: 10px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            padding: 40px;
            width: 80%;
            max-width: 600px;
        }
        h1 {
            color: #e74c3c;
            font-size: 48px;
        }
        p {
            font-size: 18px;
            color: #555;
        }
        .btn {
            margin-top: 20px;
            padding: 10px 20px;
            background-color: #3498db;
            color: #fff;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 16px;
        }
        .btn:hover {
            background-color: #2980b9;
        }
    </style>
</head>
<body>
    <div class='container'>
        <h1>Access Denied</h1>
        <p>You do not have permission to access this page.</p>
        <button class='btn' onclick='window.history.back();'>Go Back</button>
    </div>
</body>
</html>");
                return;
            }


            await _next(context);
        }

    }
}
