using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExchangeSystem.Services;

namespace ExchangeSystem.Controllers
{
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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        [Authorize]
        public IActionResult Products()
        {
            return View();
        }

        [Authorize]
        public IActionResult Stores()
        {
            return View();
        }

        [Authorize]
        public IActionResult Transactions()
        {
            return View();
        }

        [Authorize]
        public IActionResult Import()
        {
            return View();
        }

        [Authorize]
        public IActionResult DataImport()
        {
            return View();
        }

        [Authorize]
        public IActionResult ProductMapping()
        {
            return View();
        }

        [Authorize]
        public IActionResult Reports()
        {
            return View();
        }

        [Authorize]
        public IActionResult ProductsComparison()
        {
            return View();
        }

        public IActionResult SvsIntegration()
        {
            return View();
        }

        public IActionResult SvsCatalog()
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


