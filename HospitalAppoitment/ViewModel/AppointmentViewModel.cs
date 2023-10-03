using HospitalAppoitment.Models;

namespace HospitalAppoitment.ViewModel
{
    public class AppointmentViewModel
    {
        public int AppoitmentId { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string? PatientName { set; get; }
        public int DoctorId { get; set; }
        public string Address { get; set; }
        public string PatientPhoneNumber { get; set; }
        public string StatusMessage { get; set; }
    }
}
