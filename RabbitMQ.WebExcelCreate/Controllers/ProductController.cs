using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.WebExcelCreate.Models;
using RabbitMQ.WebExcelCreate.Services;

namespace RabbitMQ.WebExcelCreate.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;
        private readonly RabbitMQPublisher _rabbitMQPublisher;

        public ProductController(UserManager<IdentityUser> userManager, AppDbContext context, RabbitMQPublisher rabbitMQPublisher)
        {
            _userManager = userManager;
            _context = context;
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateProductExcel()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var fileName = $"product-excel-{Guid.NewGuid().ToString().Substring(1,10)}";
            
            UserFile userFile = new UserFile() {
            
                UserId = user.Id,
                FileName = fileName,
                FileStatus = FileStatus.Creating,

            };
            await _context.UserFiles.AddAsync(userFile);
            await _context.SaveChangesAsync();
            //RabbitMQ ye mesaj gönder
            _rabbitMQPublisher.Publish(new Shared.CreateExcelMessage()
            {
                FileId = userFile.Id,
            });

            TempData["StartCreatingExcel"] = false;
            return RedirectToAction(nameof(Files));
        }

        public async Task<IActionResult> Files()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            return View(await _context.UserFiles.Where(x=>x.UserId == user.Id).ToListAsync());
        }
    }
}
