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

        public async Task<bool> CheckAvailabilityAsync(int doctorId, DateTime startAt, DateTime endAt)
        {
            return await _repo.IsAvailableAsync(doctorId, startAt, endAt);
        }

        public async Task<Appointment> AddAppointmentAsync(AppointmentDto dto)
        {
            if (dto.EndAt <= dto.StartAt)
                throw new Exception("End time must be after start time");

            var available = await _repo.IsAvailableAsync(
                dto.DoctorId,
                dto.StartAt,
                dto.EndAt);

            if (!available)
                throw new Exception("Time slot is not available");

            var appointment = new Appointment
            {
                DoctorId = dto.DoctorId,
                PatientId = dto.PatientId,
                StartAt = dto.StartAt,
                EndAt = dto.EndAt,
                Notes = dto.Notes ?? "",
                Status = "Pending"
            };

            return await _repo.AddAsync(appointment);
        }

        public async Task CancelAsync(int appointmentId)
        {
            var appointment = await _repo.GetByIdAsync(appointmentId)
                ?? throw new Exception("Appointment not found");

            appointment.Status = "Canceled";
            await _repo.SaveChangesAsync();
        }

        public async Task ConfirmAsync(int appointmentId)
        {
            var appointment = await _repo.GetByIdAsync(appointmentId)
                ?? throw new Exception("Appointment not found");

            appointment.Status = "Confirmed";
            await _repo.SaveChangesAsync();
        }

        public async Task RescheduleAsync(
            int appointmentId,
            DateTime startAt,
            DateTime endAt,
            string? notes)
        {
            if (endAt <= startAt)
                throw new Exception("Invalid time range");

            var appointment = await _repo.GetByIdAsync(appointmentId)
                ?? throw new Exception("Appointment not found");

            var available = await _repo.IsAvailableAsync(
                appointment.DoctorId,
                startAt,
                endAt,
                appointmentId);

            if (!available)
                throw new Exception("Time slot not available");

            appointment.StartAt = startAt;
            appointment.EndAt = endAt;
            if (!string.IsNullOrWhiteSpace(notes))
                appointment.Notes = notes;

            await _repo.SaveChangesAsync();
        }

        public Task<List<object>> GetMyAppointmentsAsync(int patientId)
        {
            return _repo.GetMyAppointmentsAsync(patientId);
        }

        public Task<List<object>> GetTodayAppointmentsAsync(int doctorId)
        {
            return _repo.GetTodayAppointmentsAsync(doctorId);
        }
    }
}
