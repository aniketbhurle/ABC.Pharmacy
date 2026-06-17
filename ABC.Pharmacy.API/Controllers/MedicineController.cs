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

    [HttpGet("GetAllMedicines")]
    public async Task<IActionResult> GetAllMeds()
    {
        var result = await _service.GetAllMedicinesAsync();

        return Ok(result);
    }

    [HttpPost("addnewMedicine")]
    public async Task<IActionResult> AddNewMed(MedicineDto medicine)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _service.AddMedicineAsync(medicine);

        return Ok();
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchMed(string term)
    {
        var result = await _service.SearchMedicineAsync(term);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Sell(SellMedicineDto request)
    {
        await _service.UpdateMedicineStockAsync(request.MedicineId, request.Quantity);

        return Ok();
    }

}
