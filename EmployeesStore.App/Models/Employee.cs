namespace EmployeesStore.App.Models;

public class Employee
{
    /// <summary>
    /// id соотрудника
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// имя соотрудника
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// фамилия соотрудника
    /// </summary>
    public string Surname { get; set; }
    /// <summary>
    /// телефон соотрудника
    /// </summary>
    public string Phone { get; set; }
    /// <summary>
    /// id компании
    /// </summary>
    public int CompanyId { get; set; }
    /// <summary>
    /// данные о паспорте соотрудника
    /// </summary>
    public Passport Passport { get; set; }
    /// <summary>
    /// отдел соотрудника
    /// </summary>
    public Department Department { get; set; }
}
