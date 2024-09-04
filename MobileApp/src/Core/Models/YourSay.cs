using System;
namespace Core.Models
{
    public class YourSay : SpItem
    {
        public string FormType { get; set; } // Complaint/Feedback/Suggestion/Safety
        public DateTime ExperienceDate { get; set; }

        public string FeedbackMessage { get; set; }

        public bool TravelRelated { get; set; }

        public bool ContactUser { get; set; }

        public byte[] Photo { get; set; }
        public string PhotoName { get; set; }

        public string Route { get; set; }

        public string JourneyFrom { get; set; }

        public string JourneyTo { get; set; }

        public string TravelDirection { get; set; }

        public string TramNumber { get; set; }

        public string Email { get; set; }

        public string MobileNumber { get; set; }

        public bool HasAttachment
        {
            get
            {
                return Photo != null;
            }
        }
    }

}
