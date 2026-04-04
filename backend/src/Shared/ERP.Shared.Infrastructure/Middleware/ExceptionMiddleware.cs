using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using ERP.Shared.Models;
using ERP.Shared.Exceptions;

namespace ERP.Shared.Infrastructure.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";

            // 1. กำหนดค่าเริ่มต้นเป็น 500
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = "Internal Server Error";

            // 2. เช็คประเภท Exception (ใช้ Pattern Matching เพื่อความคลีน)
            if (ex is ERP.Shared.Exceptions.BadRequestException)
            {
                statusCode = (int)HttpStatusCode.BadRequest; // 400
                message = ex.Message;
            }
            else if (ex is NotFoundException) // เพิ่มตรงนี้เข้าไปครับ
            {
                statusCode = (int)HttpStatusCode.NotFound; // 404
                message = ex.Message;
            }
            else if (ex is UnauthorizedAccessException)
            {
                statusCode = (int)HttpStatusCode.Unauthorized; // 401
                message = "Unauthorized Access";
            }
            // เพิ่มสิทธิ์จัดการ Exception อื่นๆ ได้ที่นี่ในอนาคต

            context.Response.StatusCode = statusCode;

            // 3. สร้าง Response Object
            var response = new ErrorResponse
            {
                StatusCode = statusCode,
                Message = message,
                Details = _env.IsDevelopment() ? ex.StackTrace?.ToString() : null
            };

            // 4. Serialize และส่งกลับ
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }
    }
}