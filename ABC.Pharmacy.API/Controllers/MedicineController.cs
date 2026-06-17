using ABC.Pharmacy.Application.Interfaces;
using ABC.Pharmacy.Domain.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ABC.Pharmacy.API.Controllers;

[ApiController]
[Route("api/medicines")]
public class MedicineController : ControllerBase
{
    private readonly IMedicineService _service;
    public MedicineController(IMedicineService service)
    {
        _service = service;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllMedicines()
    {
        var result = await _service.GetAllMedicinesAsync();

        return Ok(result);
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddMedicine(MedicineDto medicine)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _service.AddMedicineAsync(medicine);

        return Ok();
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchMedicines(string term)
    {
        var result = await _service.SearchMedicinesAsync(term);

        return Ok(result);
    }

    [HttpPost("sell")]
    public async Task<IActionResult> SellMedicine(SellMedicineDto request)
    {
        await _service.UpdateMedicineStockAsync(request.MedicineId, request.Quantity);

        return Ok();
    }

}
