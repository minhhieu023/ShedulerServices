using log4net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ShedulerServices.Common
{
    public static class Logger
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public static void Error(string errMessage, bool addToDB = true)
        {
            if (addToDB)
            {
                try
                {
                    var parameters = new List<SqlParameter>()
                {
                    new SqlParameter { ParameterName = "@p_Message", Value = errMessage },
                    new SqlParameter { ParameterName = "@p_EventType", Value = "Error" },
                    new SqlParameter { ParameterName = "@p_TimeReceived", Value =DateTime.Now },
                };
                    //BaseDAL.ExecuteNonQuery("sp_AddEventLog", parameters);
                }
                catch (Exception)
                {
                    log.Error("Cannot connect DB");
                }
            }

            log.Error(errMessage);
        }

        public static void Error(Exception ex, bool addToDB = true)
        {
            if (addToDB)
            {
                try
                {
                    var parameters = new List<SqlParameter>()
                {
                    new SqlParameter { ParameterName = "@p_Message", Value = ex.Message },
                    new SqlParameter { ParameterName = "@p_EventType", Value = "Error" },
                    new SqlParameter { ParameterName = "@p_TimeReceived", Value =DateTime.Now },
                };
                    //BaseDAL.ExecuteNonQuery("sp_AddEventLog", parameters);
                }
                catch (Exception)
                {
                    log.Error("Cannot connect DB");
                }
            }
            log.Error(ex);
        }

        public static void Debug(string message, bool addToDB = false)
        {
            if (addToDB)
            {
                try
                {
                    var parameters = new List<SqlParameter>()
                {
                    new SqlParameter { ParameterName = "@p_Message", Value = message },
                    new SqlParameter { ParameterName = "@p_EventType", Value = "Informational"},
                    new SqlParameter { ParameterName = "@p_TimeReceived", Value =DateTime.Now },
                };
                    //  BaseDAL.ExecuteNonQuery("sp_AddEventLog", parameters);
                }
                catch (Exception)
                {
                    log.Error("Cannot connect DB");
                }
            }
            log.Debug(message);
        }

        public static void Diagnostic(string diagnosticMessage, bool addToDB = true)
        {
            if (addToDB)
            {
                try
                {
                    var parameters = new List<SqlParameter>()
                {
                    new SqlParameter { ParameterName = "@p_Message", Value = diagnosticMessage },
                    new SqlParameter { ParameterName = "@p_EventType", Value = "Diagnostic" },
                    new SqlParameter { ParameterName = "@p_TimeReceived", Value =DateTime.Now },
                };
                    // BaseDAL.ExecuteNonQuery("sp_AddEventLog", parameters);
                }
                catch (Exception)
                {
                    log.Warn("Cannot connect DB");
                }
            }

            log.Fatal(diagnosticMessage);
        }
    }
}

