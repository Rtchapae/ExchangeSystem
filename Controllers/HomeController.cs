using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExchangeSystem.Services;

namespace ExchangeSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IDataJoinService _dataJoinService;
        private readonly IProductService _productService;
        private readonly IStoreService _storeService;
        private readonly ITransactionService _transactionService;

        public HomeController(
            IDataJoinService dataJoinService,
            IProductService productService,
            IStoreService storeService,
            ITransactionService transactionService)
        {
            _dataJoinService = dataJoinService;
            _productService = productService;
            _storeService = storeService;
            _transactionService = transactionService;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Products()
        {
            return View();
        }

        public IActionResult Stores()
        {
            return View();
        }

        public IActionResult Transactions()
        {
            return View();
        }

        public IActionResult Import()
        {
            return View();
        }

        public IActionResult DataImport()
        {
            return View();
        }

        public IActionResult ProductMapping()
        {
            return View();
        }

        public IActionResult Reports()
        {
            return View();
        }

        public IActionResult ProductsComparison()
        {
            return View();
        }

        public IActionResult SvsIntegration()
        {
            return View();
        }

        public IActionResult EducationDepartments()
        {
            return View();
        }

        public IActionResult DataMapping()
        {
            return View();
        }
    }
}


