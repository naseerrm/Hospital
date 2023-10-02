using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HospitalAppoitment.Models
{
    public class AppointmentSlot
    {
        [Key]
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }
        [Required]
        public virtual Doctor Doctor { get; set; }
        [Required]
        public string? PatientName {  set; get; }
        [Required]
        public string Address { get; set; }
        [Required]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string PatientPhoneNumber { get; set; }

    }

    public class Doctor
    {
        [Key]
        public int Id { get; set; }
        public string DoctorName { get; set; }
        public string DoctorPosition { get; set; }
    }

    public class DoctorDbContext : DbContext
    {
        public DbSet<AppointmentSlot> Appointments { get; set; }
        public DbSet<Doctor> Doctors { get; set; }

        public DoctorDbContext(DbContextOptions<DoctorDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>().HasData(new Doctor { Id = 1, DoctorName = "Doctor 1", DoctorPosition ="General" });
            modelBuilder.Entity<Doctor>().HasData(new Doctor { Id = 2, DoctorName = "Doctor 2", DoctorPosition ="ENT" });
            modelBuilder.Entity<Doctor>().HasData(new Doctor { Id = 3, DoctorName = "Doctor 3", DoctorPosition ="EYE" });
            modelBuilder.Entity<Doctor>().HasData(new Doctor { Id = 4, DoctorName = "Doctor 4", DoctorPosition = "psychiatric" });

        }
    }
}
