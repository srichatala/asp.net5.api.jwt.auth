using asp.net5.api.jwt.auth.Model;
using asp.net5.api.jwt.auth.Service.Interface;
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
                    sqlConnection.Open();
                    SqlCommand sqlCmd = new SqlCommand("SELECT * FROM Customer WHERE UserName=" + "'" + request.UserName + "' AND Password=" + "'" + request.Password + "'", sqlConnection);
                    SqlDataReader reader = await sqlCmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        return true;
                    }
                }
                catch (Exception ex) { }
                finally
                {
                    sqlConnection.Close();
                }
            }
            return false;
        }
        public async Task<CustomerDto> GetCustomer(string userName)
        {
            var customer = new CustomerDto();
            using (SqlConnection sqlConnection = new SqlConnection(_appSettings.ConnectionString))
            {
                try
                {
                    sqlConnection.Open();
                    SqlCommand sqlCmd = new SqlCommand("SELECT * FROM Customer WHERE UserName=" + "'" + userName + "'", sqlConnection);
                    SqlDataReader reader = await sqlCmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            customer = new CustomerDto()
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                UserName = reader["UserName"].ToString(),
                                Name = reader["Name"].ToString()
                            };
                        }
                    }
                }
                catch (Exception ex) { }
                finally
                {
                    sqlConnection.Close();
                }
            }
            return customer;
        }

        public async Task<bool> Regstration(Customer customer)
        {
            bool inserted = false;
            using (SqlConnection sqlConnection = new SqlConnection(_appSettings.ConnectionString))
            {
                try
                {
                    sqlConnection.Open();
                    string query = "INSERT INTO Customer(UserName,Password,Name)values(@userName,@password,@name)";
                    using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
                    {
                        cmd.Parameters.Add("@userName", System.Data.SqlDbType.NVarChar, 50).Value = customer.UserName;
                        cmd.Parameters.Add("@password", System.Data.SqlDbType.NVarChar, 50).Value = customer.Password;
                        cmd.Parameters.Add("@name", System.Data.SqlDbType.NVarChar, 50).Value = customer.Name;
                        await cmd.ExecuteNonQueryAsync();
                        inserted = true;
                    };
                }
                catch (Exception ex) { }
                finally
                {
                    sqlConnection.Close();
                }
            }
            return inserted;
        }
    }
}
