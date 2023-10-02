using HospitalAppoitment.Models;
using HospitalAppoitment.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppoitment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly DoctorDbContext _dbcontext;
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(ILogger<AppointmentController> logger, DoctorDbContext dbcontex)
        {
            _logger = logger;
            _dbcontext= dbcontex;
        }

        [HttpGet]
        [Route("GetAllResource")]
        public List<AppointmentSlot> GetAllResource()
        {
            return _dbcontext.Appointments.Include("Doctor").ToList();
        }

        [HttpPost]
        [Route("CreateResource")]
        public AppointmentSlot CreateResource(AppointmentViewModel appoitmentViewModel)
        {
            AppointmentSlot appointmentSlot = new AppointmentSlot
            {
                Start = Convert.ToDateTime(appoitmentViewModel.Start),
                End = Convert.ToDateTime(appoitmentViewModel.End),
                DoctorId = appoitmentViewModel.DoctorId,
                PatientName = appoitmentViewModel.PatientName,
                Address = appoitmentViewModel.Address,
                PatientPhoneNumber = appoitmentViewModel.PatientPhoneNumber
            };
            _dbcontext.Appointments.Add(appointmentSlot);
            _dbcontext.SaveChanges();
            return _dbcontext.Appointments.Include("Doctor").Where(x => x.Id == appointmentSlot.Id).FirstOrDefault();
        }

        [HttpPut]
        [Route("UpdateResource")]
        public AppointmentSlot UpdateResource(AppointmentViewModel appoitmentViewModel)
        {
            AppointmentSlot appointmentSlot = _dbcontext.Appointments.Include("Doctor").Where(x=>x.Id == appoitmentViewModel.AppoitmentId).FirstOrDefault();
            if (appointmentSlot != null)
            {
                appointmentSlot.Start = Convert.ToDateTime(appoitmentViewModel.Start);
                appointmentSlot.End = Convert.ToDateTime(appoitmentViewModel.End);
                appointmentSlot.PatientName = appoitmentViewModel.PatientName;
                appointmentSlot.DoctorId = appoitmentViewModel.DoctorId;
                appointmentSlot.PatientPhoneNumber = appoitmentViewModel.PatientPhoneNumber;
                appointmentSlot.Address = appoitmentViewModel.Address;
            }
            _dbcontext.Appointments.Update(appointmentSlot);
            _dbcontext.SaveChanges();
            return appointmentSlot;
        }

        [HttpDelete]
        [Route("DeleteResource")]
        public string DeleteResource(List<int> appoitmentIds)
        {
            string returnStatus = "Failed";
            try
            {
                foreach (var appoitmentId in appoitmentIds)
                {
                    AppointmentSlot appointmentSlot = _dbcontext.Appointments.Include("Doctor").Where(x => x.Id == appoitmentId).FirstOrDefault();
                    if (appointmentSlot != null)
                    {
                        _dbcontext.Remove(appointmentSlot);
                        _dbcontext.SaveChanges();
                    }
                }
                returnStatus = "Removed Successfully....!";
            }
            catch (Exception)
            {
                returnStatus = "Failed";
            }
           return returnStatus;
        }

        [HttpGet]
        [Route("GetPatientListbySearch")]
        public List<AppointmentSlot> GetPatientListbySearch(string patientName)
        {
            if (patientName != null)
            {
                List<AppointmentSlot> appointmentSlotList = _dbcontext.Appointments.Include("Doctor").Where(x=>x.PatientName.ToUpper().Contains(patientName.ToUpper())).Select(x=>x).ToList();
                return appointmentSlotList;
            }
            return null;
        }

        [HttpGet]
        [Route("GettimePeriodListbySearch")]
        public List<AppointmentSlot> GettimePeriodListbySearch(string startDate,string endDate)
        {
            if (startDate != null && endDate != null)
            {
                List<AppointmentSlot> appointmentSlotList = _dbcontext.Appointments.Include("Doctor").Where(x => x.Start >= Convert.ToDateTime(startDate) && x.End <= Convert.ToDateTime(endDate)).Select(x => x).ToList();
                return appointmentSlotList;
            }
            return null;
        }
    }
}