using EmployeesStore.App.Models;
using EmployeesStore.App.Validators;
using EmployersStore.Core.Abstractions;
using EmployersWeb.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmployeesWeb.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeesRepository _employeesRepository;
    private readonly ValidationResult _validationResult;

    public EmployeesController(IEmployeesRepository employeesRepository, ValidationResult validationResult)
    {
        _employeesRepository = employeesRepository;
        _validationResult = validationResult;
    }
    /// <summary>
    /// получение абсолютно всех сотрудников
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetAll")]
    public async Task<ActionResult<List<Employee>>> GetAllEmployees()
    {
        var app = await _employeesRepository.GetAll();
        return Ok(app);
    }
    /// <summary>
    /// занесение в базу нового сотрудника
    /// </summary>
    /// <param name="employee"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateEmployee(Employee employee)
    {
        var app = await _employeesRepository.Create(employee);
        return Ok(app);
    }
    /// <summary>
    /// удаление сотрудника по id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveEmployee(int id)
    {
        bool idExists = await _validationResult.IsIdExists(id);
        if (idExists) {
            await _employeesRepository.RemoveId(id);
            return Ok(); 
        } else { 
            return BadRequest("Сотрудника с данным Id не существует."); 
        }
    }
    /// <summary>
    /// получение всех сотрудников по имени компании
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpGet("GetEmployeesbyNameCompany{name}")]
    public async Task<ActionResult<List<Employee>>> GetEmployeeCompany(string name)
    {
        bool nameExists = await _validationResult.IsCompanyNameExists(name, "Employees");
        if (nameExists)
        {
            var answer = await _employeesRepository.GetByNameCompany(name);
            return Ok(answer);
        }
        else
        {
            return BadRequest("Компании с данным названием не существует");
        }
    }
    /// <summary>
    /// получение сотрудников по имени отдела
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpGet("GetEmployeesbyNameDepartment{name}")]
    public async Task<ActionResult<List<Employee>>> GetEmployeeDepartment(string name)
    {
        bool nameExists = await _validationResult.IsCompanyNameExists(name, "Department");
        if (nameExists)
        {
            var answer = await _employeesRepository.GetByNameDepartment(name);
            return Ok(answer);
        }
        else
        {
            return BadRequest("Отдела с данным названием не существует");
        }
    }
    /// <summary>
    /// редактирование информации сотрудника по id 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updateModel"></param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee updateModel)
    {
        bool idExists = await _validationResult.IsIdExists(id);
        if (idExists)
        {
            await _employeesRepository.PatchbyId(id, updateModel);
            return Ok();
        }
        else
        {
            return BadRequest("Сотрудника с данным Id не существует.");
        }
    }
}
