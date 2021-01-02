﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ContactForm.Microservice.Models;
using Helper.Microservice;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;

namespace ContactForm.Microservice.Extensions
{
    public static class ExceptionMiddlewareExtension
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode =  (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (contextFeature != null)
                    {
                        log.Error($"{ExceptionHelper.GetMessages(contextFeature.Error, string.Empty, true)}");

                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
#if DEBUG
                            Message = ExceptionHelper.GetMessages(contextFeature.Error)
#else
                            Message = "Internal Server Error."
#endif
                        }.ToString());

                    }
                });
            });
        }
    }
}
