namespace SmartQueueWeb.Models
{
    public class Ticket
    {
        public string TicketNumber { get; set; } = "";
        public string ServiceType { get; set; } = "";
        public string Branch { get; set; } = "";
        public DateTime JoinTime { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Waiting";
        public string Phone { get; set; } = "";

        // Calculated properties
        public int PeopleAhead { get; set; }
        public int EstimatedMinutes { get; set; }
    }

    public class Office
    {
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string Hours { get; set; } = "";
        public string MapUrl { get; set; } = "";
    }
}