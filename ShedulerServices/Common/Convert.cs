using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ShedulerServices.Common
{
    public static class Convert
    {
        public static int ToInt(this string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value)) return 0;
                else return Convert.ToInt(value);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + "\n" + ex.HelpLink + "\n" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.TargetSite + "\n");
            }
            return 0;
        }

        public static decimal ToDecimal(this string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value)) return 0;
                else return Convert.ToDecimal(value);
            }
            catch (Exception ex)
            {
                Logger.Error("ToDecimal:" + value);
            }
            return 0;
        }

        public static DateTime ToDateTime(this string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value)) return DateTime.MinValue;
                else return Convert.ToDateTime(value);
            }
            catch (Exception ex)
            {
                Logger.Error("ToDateTime:" + value.ToString());
                return DateTime.MinValue;
            }
        }

        public static string ToYearMonth(this string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (!value.Contains("/")) return value;
            string yearMonth = string.Join("-", value.Split('/').Reverse());
            return yearMonth;
        }

        public static string ToCurrency(this decimal value)
        {
            return string.Format("${0:#,##0.00}", value);
        }

        public static string ToCardMask(this string value)
        {
            if (value.Length <= 8) return value;
            return value.Substring(value.Length - 8);
        }

        public static TDestination To<TSource, TDestination>(this TSource source)
        {
            var oDest = (TDestination)Activator.CreateInstance(typeof(TDestination));
            try
            {
                foreach (PropertyInfo p in source.GetType().GetProperties())
                {
                    foreach (PropertyInfo dest in oDest.GetType().GetProperties())
                    {
                        if (dest.Name.Equals(p.Name, StringComparison.Ordinal) && dest.CanWrite
                        && p.GetValue(source) != null && dest.PropertyType.Name == p.PropertyType.Name)
                        {
                            if (dest.PropertyType.Name == "Guid" && p.GetValue(source).ToString() == Guid.Empty.ToString()) break;
                            dest.SetValue(oDest, p.GetValue(source), null);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + "\n" + ex.HelpLink + "\n" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.TargetSite + "\n");
            }
            return oDest;
        }

        public static T To<T>(this DataRow row)
        {
            var obj = (T)Activator.CreateInstance(typeof(T));
            string pName = "";
            try
            {
                if (row != null)
                {
                    foreach (PropertyInfo p in obj.GetType().GetProperties())
                    {
                        if (p.CanWrite)
                        {
                            if (row.Table.Columns.Contains(p.Name) && row[p.Name] != null)
                            {
                                pName = p.Name;
                                if (p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?))
                                {
                                    if (row[p.Name] == null || row[p.Name] == DBNull.Value)
                                        p.SetValue(obj, Convert.ToDecimal("0"), null);
                                    else
                                        p.SetValue(obj, decimal.Parse(row[p.Name].ToString()), null);
                                }
                                else if (p.PropertyType == typeof(sbyte) || p.PropertyType == typeof(sbyte?))
                                {
                                    if (row[p.Name] == null || row[p.Name] == DBNull.Value)
                                        p.SetValue(obj, 0, null);
                                    else
                                    {
                                        p.SetValue(obj, sbyte.Parse(row[p.Name].ToString()), null);
                                    }
                                }
                                else if (p.PropertyType == typeof(int) || p.PropertyType == typeof(int?) || p.PropertyType == typeof(Int16) || p.PropertyType == typeof(Int32))
                                {
                                    p.SetValue(obj, row[p.Name].ToString().ToInt(), null);
                                }
                                else if (p.PropertyType == typeof(Int64) || p.PropertyType == typeof(Int64?))
                                {
                                    if (row[p.Name] == null || row[p.Name] == DBNull.Value)
                                        p.SetValue(obj, 0, null);
                                    else
                                        p.SetValue(obj, Int64.Parse(row[p.Name].ToString()), null);
                                }
                                else if (p.PropertyType == typeof(long) || p.PropertyType == typeof(long?))
                                {
                                    if (row[p.Name] == null || row[p.Name] == DBNull.Value)
                                        p.SetValue(obj, 0, null);
                                    else
                                        p.SetValue(obj, long.Parse(row[p.Name].ToString()), null);
                                }
                                else if (p.PropertyType == typeof(double) || p.PropertyType == typeof(double?))
                                {
                                    if (row[p.Name] == null || row[p.Name] == DBNull.Value)
                                        p.SetValue(obj, 0d, null);
                                    else
                                        p.SetValue(obj, double.Parse(row[p.Name].ToString()), null);
                                }
                                //2020/02/03 gnguyen start add
                                else if (p.PropertyType == typeof(float) || p.PropertyType == typeof(float?))
                                {
                                    if (row[p.Name] == null || row[p.Name] == DBNull.Value)
                                        p.SetValue(obj, 0, null);
                                    else
                                        p.SetValue(obj, float.Parse(row[p.Name].ToString()), null);
                                }
                                //2020/02/03 gnguyen end add
                                else if (p.PropertyType == typeof(bool) || p.PropertyType == typeof(bool?))
                                {
                                    if (row[p.Name] == null)
                                        p.SetValue(obj, false, null);
                                    else
                                    {
                                        if (row[p.Name].ToString().Equals("1"))
                                            p.SetValue(obj, true, null);
                                        else if (row[p.Name].ToString().Equals("0"))
                                            p.SetValue(obj, false, null);
                                        else
                                            p.SetValue(obj, bool.Parse(row[p.Name].ToString()), null);
                                    }
                                }
                                else if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
                                {
                                    if (row[p.Name] == null || string.IsNullOrEmpty(row[p.Name].ToString()))
                                    {
                                        p.SetValue(obj, DateTime.MinValue, null);
                                    }
                                    else
                                        p.SetValue(obj, DateTime.Parse(row[p.Name].ToString()), null);
                                }
                                else if (p.PropertyType == typeof(string) || p.PropertyType == typeof(String))
                                {
                                    if (row[p.Name] == null || row[p.Name] == DBNull.Value) p.SetValue(obj, string.Empty, null);
                                    else p.SetValue(obj, row[p.Name], null);
                                }
                                else
                                {
                                    p.SetValue(obj, row[p.Name], null);
                                }
                            }
                            else
                            {
                                p.SetValue(obj, null, null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(pName + obj.GetType().Name);
                //throw;
                Logger.Error(ex.Message + "\n" + pName + "-" + obj.GetType().Name);
            }
            return obj;
        }

        public static List<TDestination> To<TSource, TDestination>(this List<TSource> sourceList)
        {
            List<TDestination> ret = new List<TDestination>();
            foreach (TSource sourceItem in sourceList)
            {
                TDestination t = sourceItem.To<TSource, TDestination>();
                ret.Add(t);
            }
            return ret;
        }

        public static List<TDestination> To<TDestination>(this DataTable sourceList)
        {
            List<TDestination> ret = new List<TDestination>();
            try
            {
                foreach (DataRow row in sourceList.Rows)
                {
                    TDestination t = row.To<TDestination>();
                    ret.Add(t);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + "\n" + ex.HelpLink + "\n" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.TargetSite + "\n");
            }
            return ret;
        }

        public static string ToPhoneNumber(this string phone)
        {
            if (String.IsNullOrEmpty(phone)) return phone;
            return phone.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "");
        }

        public static string ToPhoneFormat(this string phone)
        {
            return System.Text.RegularExpressions.Regex.Replace(phone.ToPhoneNumber(), @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3");
        }
        /// <summary>
        /// Return format month year as Jun 2020 from 2020-06
        /// </summary>
        /// <param name="monthYear"></param>
        /// <returns></returns>
        public static string ToMonthYearFormat(this string monthYear)
        {
            if (!monthYear.Contains('-')) return monthYear;
            string[] myArr = monthYear.Split('-');
            DateTime date = new DateTime(myArr[0].ToInt(), myArr[1].ToInt(), 1);
            return date.ToString("MMM yyyy");
        }
        public static DateTime FromTimeZone(this DateTime date, int minuteTimeZone)
        {
            return date.AddMinutes(minuteTimeZone);
        }

        //public static string Encrypt(string plainText)
        //{
        //    byte[] EncryptKey = { };
        //    byte[] IV = { 55, 34, 87, 64, 87, 195, 54, 21 };
        //    EncryptKey = System.Text.Encoding.UTF8.GetBytes(Configuration.CloudSurveySecretKey.Substring(0, 8));
        //    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        //    byte[] inputByte = System.Text.Encoding.UTF8.GetBytes(plainText);
        //    MemoryStream mStream = new MemoryStream();
        //    CryptoStream cStream = new CryptoStream(mStream, des.CreateEncryptor(EncryptKey, IV), CryptoStreamMode.Write);
        //    cStream.Write(inputByte, 0, inputByte.Length);
        //    cStream.FlushFinalBlock();
        //    return Convert.ToBase64String(mStream.ToArray());
        //}

        //public static string Decrypt(string encryptedText)
        //{
        //    byte[] DecryptKey = { };
        //    byte[] IV = { 55, 34, 87, 64, 87, 195, 54, 21 };
        //    byte[] inputByte = new byte[encryptedText.Length];

        //    DecryptKey = System.Text.Encoding.UTF8.GetBytes(SurveyConfigurations.CloudSurveySecretKey.Substring(0, 8));
        //    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        //    inputByte = Convert.FromBase64String(encryptedText);
        //    MemoryStream ms = new MemoryStream();
        //    CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(DecryptKey, IV), CryptoStreamMode.Write);
        //    cs.Write(inputByte, 0, inputByte.Length);
        //    cs.FlushFinalBlock();
        //    System.Text.Encoding encoding = System.Text.Encoding.UTF8;
        //    return encoding.GetString(ms.ToArray());
        //}

        public static string ToHourMinute(this TimeSpan ts)
        {
            string displayTime = "";

            var hours = Math.Round(ts.TotalHours);
            var minutes = ts.Minutes;

            if (hours > 1)
            {
                if (minutes == 1)
                {
                    displayTime = hours + " hours and " + ts.Minutes + " minute";
                }
                else if (minutes > 1)
                {
                    displayTime = hours + " hours and " + ts.Minutes + " minutes";
                }
                else if (minutes == 0)
                {
                    displayTime = hours + " hours";
                }
            }
            else
            {
                displayTime = Math.Round(ts.TotalMinutes) + " minutes";
            }

            return displayTime;
        }
        public static string ToFormatCurrency(this decimal value)
        {
            return value < 0 ? ("-$" + Math.Abs(value).ToString("#,##0.00")) : ("$" + value.ToString("#,##0.00"));
        }
    }
}

