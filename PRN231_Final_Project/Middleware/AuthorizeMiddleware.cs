using BusinessObjects;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PRN231_Final_Project.Middleware
{
    public class AuthorizedMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _publicEndpoints = { "/api/user/sign-in", "/api/user/register", "/api/user/forgot-password", "/api/user/validate-reset-code", "/api/user/reset-password", "/api/user/refresh-token" };
        private readonly IMemoryCache _memoryCache;

        public AuthorizedMiddleware(RequestDelegate next, IMemoryCache memoryCache)
        {
            _next = next;
            _memoryCache = memoryCache;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value;
            var token = context.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
            string accessToken = (string)_memoryCache.Get("accessToken");
            if ((token != accessToken) && !IsPublicEndpoint(path))
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                var customResponse = new CustomResponse
                {
                    Data = { },
                    Message = "Unauthorized",
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Success = false
                };

                await context.Response.WriteAsJsonAsync(customResponse);
                return;
            }

            await _next.Invoke(context);
        }

        private bool IsPublicEndpoint(string path)
        {
            foreach (var endpoint in _publicEndpoints)
            {
                if (path.StartsWith(endpoint))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
