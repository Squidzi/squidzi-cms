﻿using Squidzi.Infrastructure.Cache;
using Squidzi.Infrastructure.Cache.Contracts;
using Squidzi.Infrastructure.Configuration;
using Squidzi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using Squidex.ClientLibrary.Utils;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Squidzi.Web.Controllers
{
    [Route("api/[controller]")]
    public class WebhookController : Controller
    {
        private readonly ILogger _logger;
        private readonly SquidexSettings _settings;
        private readonly ICacheProvider _cacheProvider;

        public WebhookController(
            IOptions<SquidexSettings> settings,
            ICacheProvider cacheProvider,
            ILogger<WebhookController> logger
        )
        {
            _settings = settings.Value;
            _cacheProvider = cacheProvider;
            _logger = logger;
        }

        [HttpPost]
        [Route("flush-content-cache")]
        public async Task<IActionResult> FlushContentCache()
        {
            Request.Headers.TryGetValue("X-Signature", out StringValues signature);

            using (var reader = new StreamReader(Request.Body))
            {
                var requestBody = await reader.ReadToEndAsync();
                var generatedSignature = $"{requestBody}{_settings.WebHookContentSecret}".Sha256Base64();

                if (generatedSignature == signature)
                {
                    try
                    {
                        dynamic jObject = JObject.Parse(requestBody);
                        var payload = jObject.payload;

                        string schemaName = ((string)payload.schemaId).Split(',')[1];
                        _cacheProvider.Remove(schemaName);

                        return Ok();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error ocurred trying to flush cached items");
                    }
                }
            }
            _logger.LogWarning("Could not authenticate request");
            return Unauthorized();
        }
    }
}
