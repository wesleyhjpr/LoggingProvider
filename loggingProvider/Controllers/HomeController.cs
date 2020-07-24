using loggingProvider.ExtensionLogger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace loggingProvider.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly ILogger _logger;
        public HomeController(ILogger<HomeController> logger) => _logger = logger;
        public ActionResult<string> Index()
        {
            _logger.LogInformation(999,"Chamando a página inicial do site {a}","123456");
            var a = _logger.PegarId();

            _logger.LogInformation("Chamando a página inicial do site");
            var b = _logger.PegarId();

            _logger.LogWarning("Log de Warning"); 
            var c = _logger.PegarId();

            _logger.LogCritical("Log de Critical"); 
            var d = _logger.PegarId();

            _logger.LogError("Log de Error"); 
            var e = _logger.PegarId();

            _logger.LogTrace("Log de Trace"); 
            var f = _logger.PegarId();


            throw new NullReferenceException();
        }
    }
}