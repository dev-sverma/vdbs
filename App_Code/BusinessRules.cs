using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;

namespace VDBS.App_Code
{
    public class BusinessRules
    {
        private string _fileDirectoryPath = "~\\Uploads";
        private DatabaseUtiltiy db = new DatabaseUtiltiy();
        #region Company Master Functionalities (4)
        public string GetFileName(string fileName)
        {
            string response = string.Empty;

            if (!string.IsNullOrEmpty(fileName))
                response = string.Format("{0}_{1}", DateTime.Now.ToString("HHmmss"), fileName);

            return response;
        }
        public string GetFilePath(string fileName)
        {
            string response = string.Empty;

            if (!string.IsNullOrEmpty(fileName))
            {
                response = Path.Combine(_fileDirectoryPath, fileName);
            }
            return response;
        }
        public bool SaveCompanyMaster(string companyName, List<string> files)
        {
            bool response = false;

            try
            {
                var companyMasterQuery = @"INSERT INTO CompanyMaster (Name) VALUES (@Name)";
                var comapnyMasterParams = new Dictionary<string, object>();
                comapnyMasterParams.Add("@Name", companyName);

                if (db.ExecuteNonQueryWithParameters(companyMasterQuery, comapnyMasterParams) == 1 && files != null && files.Count > 0)
                {
                    string companyId = GetMaxCompanyMasterId();

                    var fileQuery = @"INSERT INTO FileMaster (FileName, CompanyId) VALUES (@FileName ,@CompanyId)";
                    var fileQueries = new List<string>();
                    var fileQueriesParams = new List<Dictionary<string, object>>();

                    foreach (string file in files)
                    {
                        fileQueries.Add(fileQuery);
                        var parameters = new Dictionary<string, object>();
                        parameters.Add("@FileName", file);
                        parameters.Add("@CompanyId", companyId);
                        fileQueriesParams.Add(parameters);
                    }

                    response = db.ExecuteNonQueryWithParametersMultiple(fileQueries, fileQueriesParams);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return response;
        }
        private string GetMaxCompanyMasterId()
        {
            var query = @"SELECT MAX(Id) FROM CompanyMaster";
            return db.ExecuteReaderScalar(query);
        }
        #endregion

        #region Company View Functionalities

        public DataTable GetCompanyInfo()
        {
            var response = new DataTable();
            response.Columns.Add("Id");
            response.Columns.Add("Name");
            response.Columns.Add("Status");

            try
            {
                string query = @"SELECT Id, Name, ISNULL(Status, -1) AS Status FROM CompanyMaster";
                response = db.ExecuteReader(query);
            }
            catch (Exception ex)
            {
                throw;
            }
            return response;
        }

        public DataTable GetCompanyFiles(string companyId)
        {
            var response = new DataTable();
            response.Columns.Add("FileName");
            response.Columns.Add("FilePath");
            try
            {
                if (!string.IsNullOrEmpty(companyId))
                {
                    string query = "SELECT FileName FROM FileMaster WHERE CompanyId = @CompanyId";
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("@CompanyId", companyId);

                    var dbData = db.ExecuteReaderWithParameters(query, parameters);

                    if(dbData != null && dbData.Rows.Count > 0)
                    {
                        DataRow currentRow = response.NewRow();

                        foreach (DataRow row in dbData.Rows)
                        {
                            currentRow["FileName"] = row["FileName"].ToString();
                            currentRow["FilePath"] = GetFilePath(row["FileName"].ToString());
                            response.Rows.Add(currentRow);
                            currentRow = response.NewRow();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return response;
        }

        public bool ApproveDisapproveCompany(string companyId, bool approve)
        {
            var response = false;

            try
            {
                if(!string.IsNullOrEmpty(companyId))
                {
                    var query = "UPDATE CompanyMaster SET Status = @Status WHERE Id = @Id";
                    var parameters = new Dictionary<string, object>();
                    parameters.Add("@Id", companyId);
                    parameters.Add("@Status", approve ? 1 : 0);

                    if (db.ExecuteNonQueryWithParameters(query, parameters) == 1)
                        response = true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return response;
        }

        #endregion
    }
}