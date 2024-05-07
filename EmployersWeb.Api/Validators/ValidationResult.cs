using Dapper;
using EmployersWeb.Api;
namespace EmployeesStore.App.Validators;
public class ValidationResult
{
    private readonly DapperDbContext _context;

    public ValidationResult(DapperDbContext context)
    {
        _context = context;
    }
    /// <summary>
    /// проверка существования сотрудника в базе по id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> IsIdExists(int id)
    {
        using var connect = _context.CreateConnection();
        string query = "SELECT COUNT(*) FROM Employees WHERE Id = @Id";
        int count = await connect.ExecuteScalarAsync<int>(query, new { Id = id });
        return count > 0;
    }
    /// <summary>
    /// проверка существования отдела/компании 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="table"></param>
    /// <returns></returns>
    public async Task<bool> IsCompanyNameExists(string name, string table)
    {
        using var connect = _context.CreateConnection();
        string query = $"SELECT COUNT(*) FROM {table} WHERE Name = @name";
        int count = await connect.ExecuteScalarAsync<int>(query, new { Name = name });
        return count > 0;
    }
}
