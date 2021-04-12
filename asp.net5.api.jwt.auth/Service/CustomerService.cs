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

        public bool IsValidateCustomer(LoginRequest request)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_appSettings.ConnectionString))
            {
                try
                {
                    sqlConnection.Open();
                    SqlCommand sqlCmd = new SqlCommand("SELECT * FROM Customer WHERE UserName=" + "'" + request.UserName + "' AND Password=" + "'" + request.Password + "'", sqlConnection);
                    SqlDataReader reader = sqlCmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    sqlConnection.Close();
                }
            }
            return false;
        }
        public CustomerDto GetCustomer(string userName)
        {
            var customer = new CustomerDto();
            using (SqlConnection sqlConnection = new SqlConnection(_appSettings.ConnectionString))
            {
                try
                {
                    sqlConnection.Open();
                    SqlCommand sqlCmd = new SqlCommand("SELECT * FROM Customer WHERE UserName=" + "'" + userName + "'", sqlConnection);
                    SqlDataReader reader = sqlCmd.ExecuteReader();
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
                catch (Exception ex)
                {

                }
                finally
                {
                    sqlConnection.Close();
                }
            }
            return customer;
        }
    }
}
