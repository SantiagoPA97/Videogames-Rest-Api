using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace ApiWithoutEF.Models
{
    public class Dal
    {
        SqlConnection sqlConnection;
        SqlCommand command;

        public Dal()
        {
            IConfigurationRoot configuration = GetConfiguration();
            sqlConnection = new SqlConnection(configuration.GetSection("Data").GetSection("CommandApiConnection").GetSection("ConnectionString").Value);
        }

        IConfigurationRoot GetConfiguration()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);
            return builder.Build();
        }

        public DataTable GetData(string storeProcedure, List<Parameter> parameters = default(List<Parameter>))
        {
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter;
            using (SqlConnection connection = sqlConnection)
            {
                connection.Open();
                command = new SqlCommand(storeProcedure, connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParametersToStoreProcedure(parameters);

                adapter = new SqlDataAdapter(command);
                adapter.Fill(dataSet, storeProcedure);

                return dataSet.Tables[0];
            }
        }

        private void AddParametersToStoreProcedure(List<Parameter> parameters)
        {
            if (parameters?.Count > 0)
            {
                foreach (Parameter parameter in parameters)
                {
                    SqlParameter param = new SqlParameter(parameter.Name, parameter.Value);
                    param.Direction = ParameterDirection.Input;
                    param.DbType = DbType.String;
                    command.Parameters.Add(param);
                }
            }
        }

        public void SetData(string storeProcedure, List<Parameter> parameters = default(List<Parameter>))
        {
            using (SqlConnection connection = sqlConnection)
            {
                connection.Open();
                command = new SqlCommand(storeProcedure, connection);
                command.CommandType = CommandType.StoredProcedure;

                AddParametersToStoreProcedure(parameters);

                command.ExecuteNonQuery();
            } 
        }
        public int SetDataWithReturn(string storeProcedure, List<Parameter> parameters = default(List<Parameter>))
        {
            using (SqlConnection connection = sqlConnection)
            {
                try
                {
                    connection.Open();
                    command = new SqlCommand(storeProcedure, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    AddParametersToStoreProcedure(parameters);

                    int id = Convert.ToInt32(command.ExecuteScalar());

                    return id;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            } 
        }
    }
    
}