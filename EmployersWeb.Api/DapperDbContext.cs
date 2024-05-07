using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployersWeb.Api;
public class DapperDbContext
{
    private readonly IConfiguration _configuration;
    private readonly string connetionstring; 
    public DapperDbContext(IConfiguration configuration)
    {
        this._configuration = configuration;
        this.connetionstring = _configuration.GetConnectionString("connection");
    }
    public IDbConnection CreateConnection() => new SqlConnection(connetionstring);
}
