using Microsoft.AspNetCore.Mvc;
using SmartQueueWeb.Models;

namespace SmartQueueWeb.Controllers
{
    public class QueueController : Controller
    {
        public static List<Ticket> queue = new List<Ticket>();
        public static int counter = 1;

        public static List<Office> offices = new List<Office>
        {
            new Office { Name = "Home Affairs - Pietermaritzburg", Address = "218 Church St, PMB", Hours = "08:00-16:00", MapUrl = "https://maps.google.com/?q=Home+Affairs+Pietermaritzburg" },
            new Office { Name = "SASSA - Durban Central", Address = "1 Bank St, Durban", Hours = "07:30-15:30", MapUrl = "https://maps.google.com/?q=SASSA+Durban" },
            new Office { Name = "Capitec - Scottsville", Address = "Scottsville Mall", Hours = "08:00-17:00", MapUrl = "https://maps.google.com/?q=Capitec+Scottsville" },
        };

        public IActionResult Index(string lang = "en")
        {
            // Calculate people ahead + estimated time
            for (int i = 0; i < queue.Count; i++)
            {
                queue[i].PeopleAhead = i;
                queue[i].EstimatedMinutes = i * 5; // 5 min per person
            }
            ViewBag.Offices = offices;
            ViewBag.Lang = lang;
            return View(queue);
        }

        [HttpPost]
        public IActionResult Join(string service, string branch, string phone)
        {
            if (string.IsNullOrEmpty(service)) service = "General Service";
            var ticket = new Ticket
            {
                TicketNumber = "Q-" + counter.ToString("D3"),
                ServiceType = service,
                Branch = branch ?? "Pietermaritzburg",
                JoinTime = DateTime.Now,
                Status = "Waiting",
                Phone = phone ?? ""
            };
            queue.Add(ticket);
            counter++;
            TempData["MyTicket"] = ticket.TicketNumber;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Cancel(string ticketNo)
        {
            queue.RemoveAll(x => x.TicketNumber == ticketNo);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult DonateSpot(string ticketNo)
        {
            var ticket = queue.FirstOrDefault(x => x.TicketNumber == ticketNo);
            if (ticket != null)
            {
                queue.Remove(ticket);
                ticket.JoinTime = DateTime.Now;
                queue.Add(ticket); // Move to end = donate spot
                TempData["Ubuntu"] = $"❤️ Ubuntu! You donated {ticketNo} to Gogo/ Mama! You are now last. Siyabonga!";
                TempData["MyTicket"] = ticketNo;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ServeNext()
        {
            if (queue.Count > 0)
            {
                var next = queue[0];
                queue.RemoveAt(0);
                TempData["Served"] = next.TicketNumber;
            }
            return RedirectToAction("Index");
        }

        public IActionResult Dashboard()
        {
            ViewBag.Total = queue.Count;
            ViewBag.AvgWait = queue.Count * 5;
            return View(queue);
        }

        // API for live tracking
        public JsonResult LiveStatus()
        {
            return Json(new { count = queue.Count, nextTicket = queue.FirstOrDefault()?.TicketNumber });
        }
    }
}