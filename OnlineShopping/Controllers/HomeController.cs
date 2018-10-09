using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OnlineShopping.Hubs;
using OnlineShopping.Models;

namespace OnlineShopping.Controllers
{
    public class HomeController : Controller
    {
        private IHubContext<ChatHub> HubContext { get; set; }

        public HomeController(IHubContext<ChatHub> hubcontext)
        {
            HubContext = hubcontext;
            //https://stackoverflow.com/questions/46904678/call-signalr-core-hub-method-from-controller
        }

        public IActionResult Index()
        {
            var videoList = VideoList.Videos();
            ViewBag.Videos = videoList;

            return View();
        }

        public async Task<IActionResult> ApiCall(string ip, int id)
        {
            await this.HubContext.Clients.All.SendAsync(ip, id, VideoList.Videos().First(x => x.Id == Convert.ToInt32(id)).Url);
            VideoOwned.AddUser(ip, id);

            return Content("successfull");
        }

        public IActionResult QrGenerate()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
