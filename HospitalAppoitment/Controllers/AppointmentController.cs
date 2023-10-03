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

        public AppointmentController(DoctorDbContext dbcontex)
        {
            _dbcontext= dbcontex;
        }

        [HttpGet]
        [Route("GetAllResource")]
        public List<AppointmentViewModel> GetAllResource()
        {
            return GetModellistToDTOMapper(_dbcontext.Appointments.Include("Doctor").ToList());
        }

        [HttpGet]
        [Route("GetAllResourceByDoctorId")]
        public List<AppointmentViewModel> GetAllResourceByDoctorId(int doctorId)
        {
            return GetModellistToDTOMapper(_dbcontext.Appointments.Include("Doctor").Where(x=>x.DoctorId == doctorId).ToList());
        }

        [HttpPost]
        [Route("CreateResource")]
        public AppointmentViewModel CreateResource(AppointmentViewModel appoitmentViewModel)
        {
            AppointmentViewModel appointmentViewModel = new AppointmentViewModel();
            List<AppointmentSlot> appointmentSlotList = _dbcontext.Appointments.Include("Doctor").Where(x => x.Start >= Convert.ToDateTime(appoitmentViewModel.Start) && x.End <= Convert.ToDateTime(appoitmentViewModel.End) && x.DoctorId == appoitmentViewModel.DoctorId).Select(x => x).ToList();
            if (appointmentSlotList.Count == 0)
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
                return GetModelToDTOMapper(_dbcontext.Appointments.Include("Doctor").Where(x => x.Id == appointmentSlot.Id).FirstOrDefault());
            }
            else
            {
                appointmentViewModel.StatusMessage = "Already booked on this time Slots for this DOCTOR...! Can you please change the time slot to create your appointment?";
            }
            return appointmentViewModel;
        }

        [HttpPut]
        [Route("UpdateResource")]
        public AppointmentViewModel UpdateResource(AppointmentViewModel appoitmentViewModel)
        {
            AppointmentViewModel appointmentViewModel = new AppointmentViewModel();

            // check the condition for Start date null or empty
            // check the condition for End date null or empty
            // check the condition for Doctor input value null or empty
            if (appoitmentViewModel.Start != "string" && appoitmentViewModel.End != "string" && appoitmentViewModel.DoctorId != 0)
            {
                List<AppointmentSlot> appointmentSlotList = _dbcontext.Appointments.Include("Doctor").Where(x => x.Start >= Convert.ToDateTime(appoitmentViewModel.Start) && x.End <= Convert.ToDateTime(appoitmentViewModel.End) && x.DoctorId == appoitmentViewModel.DoctorId).Select(x => x).ToList();
                if (appointmentSlotList.Count != 0)
                {
                    appointmentViewModel.StatusMessage = "Already booked on this time Slots for this DOCTOR...! Can you please change the time slot to create your appointment?";
                    return appointmentViewModel;
                }
            }
            else
            {
                if (appoitmentViewModel.DoctorId == 0)
                {
                    appointmentViewModel.StatusMessage = "Please give a input for DoctorId..!";
                    return appointmentViewModel;
                }
            }

            // check the condition for Start date null or empty
            // check the condition for End date null or empty
            if ((appoitmentViewModel.Start != "string" && appoitmentViewModel.End == "string") || (appoitmentViewModel.Start == "string" && appoitmentViewModel.End != "string"))
            {
                appointmentViewModel.StatusMessage = "Please give input for Start Date and End Date...!";
                return appointmentViewModel;
            }

            // get the appointment slot by appointmentId
            AppointmentSlot appointmentSlot = _dbcontext.Appointments.Include("Doctor").Where(x=>x.Id == appoitmentViewModel.AppoitmentId).FirstOrDefault();
            if (appointmentSlot != null)
            {
                if (appoitmentViewModel.Start != "string")
                {
                    appointmentSlot.Start = Convert.ToDateTime(appoitmentViewModel.Start);
                }
                if (appoitmentViewModel.End != "string")
                {
                    appointmentSlot.End = Convert.ToDateTime(appoitmentViewModel.End);
                }
                if (appoitmentViewModel.PatientName != "string")
                {
                    appointmentSlot.PatientName = appoitmentViewModel.PatientName;
                }
                if (appoitmentViewModel.DoctorId != 0)
                {
                    appointmentSlot.DoctorId = appoitmentViewModel.DoctorId;
                }
                if (appoitmentViewModel.PatientPhoneNumber != "string")
                {
                    appointmentSlot.PatientPhoneNumber = appoitmentViewModel.PatientPhoneNumber;
                }
                if (appoitmentViewModel.Address != "string")
                {
                    appointmentSlot.Address = appoitmentViewModel.Address;
                }
                _dbcontext.Appointments.Update(appointmentSlot);
                _dbcontext.SaveChanges();
            }
            return GetModelToDTOMapper(appointmentSlot);
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
        public List<AppointmentViewModel>? GetPatientListbySearch(string patientName,int doctorId)
        {
            if (patientName != null)
            {
                List<AppointmentSlot> appointmentSlotList = _dbcontext.Appointments.Include("Doctor").Where(x => x.PatientName.ToUpper().Contains(patientName.ToUpper()) && x.DoctorId == doctorId).Select(x => x).ToList();
                return GetModellistToDTOMapper(appointmentSlotList);
            }
            return null;
        }

        [HttpGet]
        [Route("GetTimePeriodListbySearch")]
        public List<AppointmentViewModel>? GetTimePeriodListbySearch(string startDate,string endDate,int doctorId)
        {
            if (startDate != null && endDate != null)
            {
                List<AppointmentSlot> appointmentSlotList = _dbcontext.Appointments.Include("Doctor").Where(x => x.Start >= Convert.ToDateTime(startDate) && x.End <= Convert.ToDateTime(endDate) && x.DoctorId == doctorId).Select(x => x).ToList();
                return GetModellistToDTOMapper(appointmentSlotList);
            }
            return null;
        }

        private List<AppointmentViewModel> GetModellistToDTOMapper(List<AppointmentSlot> appointmentSlots)
        {
            List<AppointmentViewModel> appointmentViewModels = new List<AppointmentViewModel>();
            foreach (var appointmentSlot in appointmentSlots)
            {
                appointmentViewModels.Add(GetModelToDTOMapper(appointmentSlot));
            }
            return appointmentViewModels;
        }

        private AppointmentViewModel GetModelToDTOMapper(AppointmentSlot appointmentSlot)
        {
            AppointmentViewModel appointmentViewModel = new AppointmentViewModel()
            {
                PatientName = appointmentSlot.PatientName,
                Start = appointmentSlot.Start.ToString("dd-MM-yyyy hh:mm tt"),
                End = appointmentSlot.End.ToString("dd-MM-yyyy hh:mm tt"),
                DoctorId = appointmentSlot.DoctorId,
                Address = appointmentSlot.Address,
                PatientPhoneNumber = appointmentSlot.PatientPhoneNumber,
                AppoitmentId = appointmentSlot.Id
            };
            return appointmentViewModel;
        }
    }
}