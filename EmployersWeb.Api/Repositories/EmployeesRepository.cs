using Dapper;
using EmployeesStore.App.Models;
using EmployersStore.Core.Abstractions;

namespace EmployersWeb.Api.Repositories;
public class EmployeesRepository : IEmployeesRepository
{
    private readonly DapperDbContext _context;
    /// <summary>
    /// конструктор
    /// </summary>
    /// <param name="context"></param>
    public EmployeesRepository(DapperDbContext context)
    {
        _context = context;
    }
    /// <summary>
    /// создание соотрудника
    /// </summary>
    /// <param name="employee"></param>
    /// <returns></returns>
    public async Task<int> Create(Employee employee)
    {

        using var connect = _context.CreateConnection();
        string query = @"
                INSERT INTO Employees (Name, Surname, Phone, CompanyId)
                VALUES (@Name, @Surname, @Phone, @CompanyId);
                SELECT SCOPE_IDENTITY();

                INSERT INTO Passport (Type, Number)
                VALUES (@PassportType, @PassportNumber);
    
                INSERT INTO Department (Name, Phone)
                VALUES (@DepartmentName, @DepartmentPhone);
                ";
        var parameters = new DynamicParameters();
        parameters.Add("Name", employee.Name, System.Data.DbType.String);
        parameters.Add("Surname", employee.Surname, System.Data.DbType.String);
        parameters.Add("Phone", employee.Phone, System.Data.DbType.String);
        parameters.Add("CompanyId", employee.CompanyId, System.Data.DbType.Int32);
        parameters.Add("PassportType", employee.Passport.Type, System.Data.DbType.String);
        parameters.Add("PassportNumber", employee.Passport.Number, System.Data.DbType.String);
        parameters.Add("DepartmentName", employee.Department.Name, System.Data.DbType.String);
        parameters.Add("DepartmentPhone", employee.Department.Phone, System.Data.DbType.String);
        int employeeId = await connect.QueryFirstOrDefaultAsync<int>(query, parameters);

        return employeeId;

    }
    /// <summary>
    /// вывод абсолютно всех сотрудников
    /// </summary>
    /// <returns></returns>
    public async Task<List<Employee>> GetAll()
    {
        string query = @"
            SELECT e.*, p.*, d.*
            FROM Employees e
            LEFT JOIN Passport p ON e.Id = p.Id
            LEFT JOIN Department d ON e.Id = d.Id";
        using var connect = _context.CreateConnection();
        var employeesWithPassportAndDepartment = await connect.QueryAsync<Employee, Passport, Department, Employee>(
            query,
            (employee, passport, department) =>
            {
                employee.Passport = passport;
                employee.Department = department;
                return employee;
            },
            splitOn: "Id, Id");
        return employeesWithPassportAndDepartment.ToList();
    }
    /// <summary>
    /// получение сотрудников по имени компании
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<List<Employee>> GetByNameCompany(string name)
    {
        string query = @"
        SELECT e.*, p.*, d.*
        FROM Employees e
        LEFT JOIN Passport p ON e.Id = p.Id
        LEFT JOIN Department d ON e.Id = d.Id
        WHERE e.Name = @Name"; 
        using var connect = _context.CreateConnection();
        var employeesWithPassportAndDepartment = await connect.QueryAsync<Employee, Passport, Department, Employee>(
            query,
            (employee, passport, department) =>
            {
                employee.Passport = passport;
                employee.Department = department;
                return employee;
            },
            param: new { Name = name }, 
            splitOn: "Id, Id");
        return employeesWithPassportAndDepartment.ToList();
    }
    /// <summary>
    /// получение соотрудников по имени отдела
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<List<Employee>> GetByNameDepartment(string name)
    {
        string query = @"
        SELECT e.*, p.*, d.*
        FROM Employees e
        LEFT JOIN Passport p ON e.Id = p.Id
        LEFT JOIN Department d ON e.Id = d.Id
        WHERE d.Name = @Name";
        using var connect = _context.CreateConnection();
        var employeesWithPassportAndDepartment = await connect.QueryAsync<Employee, Passport, Department, Employee>(
            query,
            (employee, passport, department) =>
            {
                employee.Passport = passport;
                employee.Department = department;
                return employee;
            },
            param: new { Name = name },
            splitOn: "Id, Id");
        return employeesWithPassportAndDepartment.ToList();
    }
    /// <summary>
    /// редактирование по id полей из запроса
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updateEmployee"></param>
    /// <returns></returns>
    public async Task<List<Employee>> PatchbyId(int id, Employee updateEmployee)
    {

        string query = @"
        SELECT e.*, p.*, d.*
        FROM Employees e
        LEFT JOIN Passport p ON e.Id = p.Id
        LEFT JOIN Department d ON e.Id = d.Id
        WHERE e.Id = @Id";
        using var connect = _context.CreateConnection();
        var employeesWithPassportAndDepartment = await connect.QueryAsync<Employee, Passport, Department, Employee>(
            query,
            (employee, passport, department) =>
            {
                employee.Passport = passport;
                employee.Department = department;
                return employee;
            },
            new { Id = id },
            splitOn: "Id, Id");
        var existingEmployee = employeesWithPassportAndDepartment.FirstOrDefault();
        if (existingEmployee.Name != updateEmployee.Name)
            existingEmployee.Name = updateEmployee.Name;

        if (existingEmployee.Surname != updateEmployee.Surname)
            existingEmployee.Surname = updateEmployee.Surname;

        if (existingEmployee.Phone != updateEmployee.Phone)
            existingEmployee.Phone = updateEmployee.Phone;

        if (existingEmployee.CompanyId != updateEmployee.CompanyId)
            existingEmployee.CompanyId = updateEmployee.CompanyId;

        if (existingEmployee.Passport.Type != updateEmployee.Passport.Type)
            existingEmployee.Passport.Type = updateEmployee.Passport.Type;

        if (existingEmployee.Passport.Number != updateEmployee.Passport.Number)
            existingEmployee.Passport.Number = updateEmployee.Passport.Number;

        if (existingEmployee.Department.Name != updateEmployee.Department.Name)
            existingEmployee.Department.Name = updateEmployee.Department.Name;

        if (existingEmployee.Department.Phone != updateEmployee.Department.Phone)
            existingEmployee.Department.Phone = updateEmployee.Department.Phone;
        using (var connection = _context.CreateConnection())
        {
            string updateEmployeeSql = @"
            UPDATE Employees 
            SET Name = @Name, Surname = @Surname, Phone = @Phone, CompanyId = @CompanyId 
            WHERE Id = @Id";
            await connection.ExecuteAsync(updateEmployeeSql, existingEmployee);
            string updatePassportSql = @"
                UPDATE Passport 
                SET Type = @Type, Number = @Number 
                WHERE Id = @PassportId";
            await connection.ExecuteAsync(updatePassportSql, new
            {
                Type = existingEmployee.Passport.Type,
                Number = existingEmployee.Passport.Number,
                PassportId = existingEmployee.Id  
            });
            string updateDepartmentSql = @"
            UPDATE Department 
            SET Name = @Name, Phone = @Phone 
            WHERE Id = @DepartmentId";
            await connection.ExecuteAsync(updateDepartmentSql, new
            {
                Name = existingEmployee.Department.Name,
                Phone = existingEmployee.Department.Phone,
                DepartmentId = existingEmployee.Id  
            });
        }
        return employeesWithPassportAndDepartment.ToList();
    }
    /// <summary>
    /// удаление сотрудника по id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task RemoveId(int id)
    {
        string query = @"DELETE FROM Employees WHERE Id = @Id";
        using var connect = _context.CreateConnection();
        await connect.ExecuteAsync(query, new { Id = id });
    }
}
