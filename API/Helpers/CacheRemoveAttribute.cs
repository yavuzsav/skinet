using System;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    public class CacheRemoveAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string _pattern;

        public CacheRemoveAttribute(string pattern)
        {
            _pattern = pattern;
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

            if (_pattern != null)
                await cacheService.DeleteCacheByPatternAsync(_pattern);

            await next();
        }
    }
}