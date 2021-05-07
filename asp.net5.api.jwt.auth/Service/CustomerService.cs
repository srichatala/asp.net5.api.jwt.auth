using asp.net5.api.jwt.auth.Model;
using asp.net5.api.jwt.auth.Service.Interface;
using Dapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace asp.net5.api.jwt.auth.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly AppSettings _appSettings;
        public CustomerService(IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
        }

        public async Task<bool> IsValidateCustomer(LoginRequest request)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_appSettings.ConnectionString))
            {
                try
                {
                    string query = "SELECT * FROM [dbo].[User] WHERE Email=" + "'" + request.UserName + "' AND Password=" + "'" + request.Password + "'";
                    var data = await sqlConnection.QueryFirstOrDefaultAsync<User>(query, sqlConnection);

                    if(data != null)
                    {
                        return true;
                    }
                }
                catch (Exception ex) { }
                finally{}
            }
            return false;
        }
        public async Task<UserDto> GetCustomer(string userName)
        {
            var customer = new UserDto();
            using (SqlConnection sqlConnection = new SqlConnection(_appSettings.ConnectionString))
            {
                try
                {
                    var query = "SELECT * FROM [dbo].[User] WHERE Email=" + "'" + userName + "'";
                    customer = await sqlConnection.QueryFirstOrDefaultAsync<UserDto>(query, sqlConnection);
                }
                catch (Exception ex) { }
                finally{}
            }
            return customer;
        }

        public async Task<bool> Regstration(User user)
        {
            bool inserted = false;
            using (SqlConnection sqlConnection = new SqlConnection(_appSettings.ConnectionString))
            {
                try
                {
                    string query = "INSERT INTO [dbo].[User] (FirstName,LastName,Email,Password,PhoneNumber) values (@FirstName,@LastName,@Email,@Password,@PhoneNumber)";
                    var affectedRows = await sqlConnection.ExecuteAsync(query, new { FirstName = user.FirstName,LastName = user.LastName, Email = user.Email, Password = user.Password, PhoneNumber = user.PhoneNumber });
                    if(affectedRows > 0)
                    {
                        inserted = true;
                    }
                }
                catch (Exception ex) { }
                finally
                {
                }
            }
            return inserted;
        }
    }
}
