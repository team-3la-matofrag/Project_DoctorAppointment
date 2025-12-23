using Microsoft.AspNetCore.Mvc;
using Project.BLL.Interfaces;
using Project.BLL.DTOs;

[ApiController]
[Route("api/doctors")]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _service;

    public DoctorsController(IDoctorService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var doctor = await _service.GetByIdAsync(id);
        if (doctor == null) return NotFound();
        return Ok(doctor);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody]CreateDoctorDto dto)
    {
        await _service.AddAsync(dto);
        return Ok();
    }


    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, DoctorDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpPost("{id:int}/toggle")]
    public async Task<IActionResult> Toggle(int id)
    {
        await _service.ToggleStatusAsync(id);
        return Ok();
    }
}
