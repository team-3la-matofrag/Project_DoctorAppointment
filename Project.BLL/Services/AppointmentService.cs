using Project.BLL.DTOs;
using Project.BLL.Interfaces;
using Project.DAL.Interfaces;
using Project.DAL.Models;

namespace Project.BLL.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repo;

        public AppointmentService(IAppointmentRepository repo)
        {
            _repo = repo;
        }

        public async Task CreateAsync(CreateAppointmentDto dto)
        {
            if (dto.EndAt <= dto.StartAt)
                throw new Exception("Invalid time range");

            if (await _repo.HasConflictAsync(dto.DoctorId, dto.StartAt, dto.EndAt))
                throw new Exception("Doctor is busy at this time");

            var appointment = new Appointment
            {
                DoctorId = dto.DoctorId,
                PatientId = dto.PatientId,
                StartAt = dto.StartAt,
                EndAt = dto.EndAt,
                Notes = dto.Notes,
                Status = AppointmentStatus.Pending
            };


            await _repo.AddAsync(appointment);
        }
        public async Task CompleteAsync(int appointmentId)
        {
            var appointment = await _repo.GetByIdAsync(appointmentId)
                ?? throw new Exception("Appointment not found");

            if (appointment.Status != AppointmentStatus.Confirmed)
                throw new Exception("Only confirmed appointments can be completed");

            appointment.Status = AppointmentStatus.Completed;
            await _repo.SaveChangesAsync();
        }

        public async Task ConfirmAsync(int appointmentId)
        {
            var appointment = await _repo.GetByIdAsync(appointmentId)
                ?? throw new Exception("Appointment not found");

            if (appointment.Status != AppointmentStatus.Pending)
                throw new Exception("Only pending appointments can be confirmed");

            appointment.Status = AppointmentStatus.Confirmed;
            await _repo.SaveChangesAsync();
        }

        public async Task<List<AppointmentDetailsDto>> GetAllAsync()
        {
            var appointments = await _repo.GetAllWithUsersAsync();

            return appointments.Select(a => new AppointmentDetailsDto
            {
                Id = a.Id,
                StartAt = a.StartAt,
                EndAt = a.EndAt,
                Status = a.Status.ToString(),

                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.User.FullName,

                PatientId = a.PatientId,
                PatientName = a.Patient.User.FullName
            }).ToList();
        }



        public async Task CancelAsync(int id)
        {
            var appointment = await _repo.GetByIdAsync(id)
                ?? throw new Exception("Appointment not found");

            appointment.Status = AppointmentStatus.Cancelled;
            await _repo.SaveChangesAsync();
        }

        public async Task<List<AppointmentDto>> GetDoctorAppointmentsAsync(int doctorId)
            => (await _repo.GetByDoctorAsync(doctorId))
                .Select(Map)
                .ToList();

        public async Task<List<AppointmentDto>> GetPatientAppointmentsAsync(int patientId)
            => (await _repo.GetByPatientAsync(patientId))
                .Select(Map)
                .ToList();

        private AppointmentDto Map(Appointment a) => new()
        {
            Id = a.Id,
            StartAt = a.StartAt,
            EndAt = a.EndAt,
            Status = a.Status.ToString()
        };
         
    }

}
