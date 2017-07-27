using Common.Log4Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;
using Mono.Math;
using System.Web.Script.Serialization;

namespace Common
{
    public static class Common
    {
        #region 集合类型转换部分

        /// <summary>
        /// 转换器：stirng类型转换为int类型的标准函数
        /// </summary>
        public static Func<string, int?> CONVERT_STRING_TO_INT32 = (item) => {
            int i;
            if(!Int32.TryParse(item, out i))
            {
                return null;
            }
            return i;
        };

        /// <summary>
        /// 转换器：string类型转换为guid类型的标准函数
        /// </summary>
        public static Func<string, Guid?> CONVERT_STRING_TO_GUID = (item) => {
            Guid guid;
            if (!Guid.TryParse(item, out guid))
            {
                return null;
            }
            return guid;
        };

        /// <summary>
        /// 集合类型转换函数（全部元素转换）
        /// </summary>
        /// <typeparam name="Tin">转换前的类型</typeparam>
        /// <typeparam name="Tout">转换后的类型（必须是值类型）</typeparam>
        /// <param name="source">待转换的集合</param>
        /// <param name="convertFunc">转换器</param>
        /// <returns>转换后的集合</returns>
        public static List<Tout> ToList<Tin, Tout>(this IEnumerable<Tin> source, Func<Tin, Tout?> convertFunc) where Tout : struct
        {
            return Common.ToList<Tin, Tout>(source, convertFunc, true);
        }

        /// <summary>
        /// 集合类型转换函数
        /// </summary>
        /// <typeparam name="Tin">转换前的类型</typeparam>
        /// <typeparam name="Tout">转换后的类型（必须是值类型）</typeparam>
        /// <param name="source">待转换的集合</param>
        /// <param name="convertFunc">转换器</param>
        /// <param name="isCompleteConvert">是否完整转换，true-必须转换所有元素，false-可以不转换所有元素</param>
        /// <returns>转换后的集合</returns>
        public static List<Tout> ToList<Tin, Tout>(this IEnumerable<Tin> source, Func<Tin, Tout?> convertFunc, bool isCompleteConvert) where Tout : struct
        {
            List<Tout> list = new List<Tout>();
            foreach(Tin item in source)
            {
                Tout? itemResult = convertFunc(item);
                if (itemResult == null)
                {
                    if (isCompleteConvert)
                    {
                        return null;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    list.Add((Tout)itemResult);
                }
            }
            return list;
        }

        #endregion

        #region 对象与json字符串的转换函数

        /// <summary>
        /// 对象转为Json字符串
        /// </summary>
        /// <param name="item">待转换的对象</param>
        /// <returns>转换后的字符串</returns>
        public static string SerializeJsonString(object item) 
        {
            if (item == null)
            {
                return null;
            }
            try
            {
                return new JavaScriptSerializer().Serialize(item);
            }
            catch(Exception exception)
            {
                Log4NetUtils.Error("Common.Common", "json格式转换失败，item：" + item, exception);
                return null;
            }
        }

        /// <summary>
        /// Json字符串转为对象
        /// </summary>
        /// <typeparam name="T">转换后的对象类型</typeparam>
        /// <param name="jsonString">待转换的字符串</param>
        /// <returns>转换后的T类型对象</returns>
        public static T DeserializeJsonString<T>(string jsonString)
        {
            try
            {
                return new JavaScriptSerializer().Deserialize<T>(jsonString);
            }
            catch (Exception exception)
            {
                Log4NetUtils.Error("Common.Common", "json格式转换失败，jsonString：" + jsonString, exception);
                return default(T);
            }
        }

        #endregion

        #region 生成排序字段值的函数

        /// <summary>
        /// 生成当前用于排序字段值
        /// </summary>
        /// <returns>当前排序字段的值</returns>
        public static string CreateNewOrder() {
            return Common.CreateNewOrder(1);
        }

        /// <summary>
        /// 按序列索引生成当前用于排序字段值
        /// </summary>
        /// <param name="i">序列索引</param>
        /// <returns>当前排序字段的值</returns>
        public static string CreateNewOrder(int i) 
        {
            /* UNIX */
            DateTime startDatetime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            string datetimeString = (DateTime.Now - startDatetime).TotalSeconds.ToString();
            if (datetimeString.Length < 15)
            {
                datetimeString = datetimeString + string.Format("{0:D" + (15 - datetimeString.Length) + "}", 0);
            }
            return datetimeString.Substring(0, 15) + string.Format("{0:D5}", i);
        }

        /// <summary>
        /// 生成置顶排序字段值
        /// </summary>
        /// <param name="currentFirstOrder">当前排在首位的排序字段值</param>
        /// <returns>新的置顶排序字段值</returns>
        public static string CreateFirstOrder(string currentFirstOrder) {
            return (Convert.ToInt32(currentFirstOrder.Substring(0, 10)) - 1) + currentFirstOrder.Substring(10);
        }

        /// <summary>
        /// 生成末尾排序字段值
        /// </summary>
        /// <param name="currentLastOrder">当前排在末尾的排序字段值</param>
        /// <returns>新的末尾排序字段值</returns>
        public static string CreateLastOrder(string currentLastOrder)
        {
            return (Convert.ToInt32(currentLastOrder.Substring(0, 10)) + 1) + currentLastOrder.Substring(10);
        }

        /// <summary>
        /// 生成中间排序字段值
        /// </summary>
        /// <param name="prevOrder">前一个排序字段值</param>
        /// <param name="nextOrder">后一个排序字段值</param>
        /// <returns>新的中间排序字段值</returns>
        //public static string CreateMiddleOrder(string prevOrder, string nextOrder)
        //{
        //    int prevOrderLength = prevOrder.Length;
        //    int nextOrderLength = nextOrder.Length;
        //    if (prevOrderLength > nextOrderLength) 
        //    {
        //        nextOrder += string.Format("{0:D" + (prevOrderLength - nextOrderLength) + "}", 0);
        //    }
        //    else if (prevOrderLength < nextOrderLength)
        //    {
        //        prevOrder += string.Format("{0:D" + (nextOrderLength - prevOrderLength) + "}", 0);
        //    }
        //    BigInteger prevBigInteger = BigInteger.Parse(prevOrder.Substring(0, 10) + prevOrder.Substring(11));
        //    BigInteger nextBigInteger = BigInteger.Parse(nextOrder.Substring(0, 10) + nextOrder.Substring(11));
        //    BigInteger sumBigInteger = BigInteger.Add(prevBigInteger, nextBigInteger);
        //    BigInteger remainder;
        //    string dividendString = BigInteger.DivRem(sumBigInteger, new BigInteger(2), out remainder).ToString();
        //    string middleOrder = dividendString.Substring(0, 10) + "." + dividendString.Substring(10);
        //    if (!remainder.Equals(BigInteger.Zero))
        //    {
        //        middleOrder += "5";
        //    }
        //    return middleOrder;
        //}
        
        #endregion

        #region 上传文件

        /// <summary>
        /// 处理文件上传的公共方法
        /// </summary>
        /// <param name="fileCollection">Request.Files集合</param>
        /// <param name="parameterName">上传文件的参数名称</param>
        /// <param name="dictionaryName">保存文件的文件夹路径</param>
        /// <returns>Base64编码的文件名称</returns>
        //public static string SaveUploadFile(HttpFileCollectionBase fileCollection, string parameterName, string dictionaryName)
        //{
        //    try
        //    {
        //        HttpPostedFileBase uploadFile = fileCollection[parameterName];
        //        string fileName = Guid.NewGuid().ToString();
        //        if (!Directory.Exists(dictionaryName))
        //        {
        //            Directory.CreateDirectory(dictionaryName);
        //        }
        //        uploadFile.SaveAs(dictionaryName + fileName);
        //        return Convert.ToBase64String(Encoding.Default.GetBytes(fileName.ToString()));
        //    }
        //    catch (Exception exception)
        //    {
        //        Log4NetUtils.Error("Common.Common", "上传文件错误", exception);
        //        return null;
        //    }
        //}

        #endregion 

        #region 序列化与反序列化

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="parameter">待序列化的对象</param>
        /// <returns>序列化后的byte数组</returns>
        public static byte[] Serialize(object parameter)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, parameter);
                byte[] byteArray = memoryStream.ToArray();
                memoryStream.Close();
                return byteArray;
            }
            catch (Exception exception)
            {
                Log4NetUtils.Error("Common.Common", "序列化数据失败：" + parameter.ToString(), exception);
                return null;
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">反序列化的结果类型</typeparam>
        /// <param name="parameter">byte数组</param>
        /// <returns>反序列化后的结果</returns>
        public static T Deserialize<T>(byte[] parameter) where T : class
        {
            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                MemoryStream memoryStream = new MemoryStream(parameter);
                T result = binaryFormatter.Deserialize(memoryStream) as T;
                memoryStream.Close();
                return result;
            }
            catch (Exception exception)
            {
                Log4NetUtils.Error("Common.Common", "反序列化数据失败", exception);
                return null;
            }
        }

        #endregion

        #region HttpWebReuqest访问

        /// <summary>
        /// HttpWebReuqest访问web资源，GET方式
        /// </summary>
        /// <param name="url">页面地址</param>
        /// <param name="cookieContainer">Cookie容器</param>
        /// <returns>web资源string</returns>
        public static string HttpWebReuqestGet(string url, CookieContainer cookieContainer)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                httpWebRequest.Accept = "*/*";
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                httpWebRequest.KeepAlive = true;
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 5000 * 2;
                httpWebRequest.CookieContainer = cookieContainer;
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                string webString = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                httpWebResponse.Close();
                cookieContainer.Add(httpWebRequest.CookieContainer.GetCookies(httpWebRequest.RequestUri));
                return webString;
            }
            catch(Exception exception)
            {
                Log4NetUtils.Error("Common.Common", "HttpWebReuqestGet访问失败，url：" + url, exception);
                return null;
            }
        }

        /// <summary>
        /// HttpWebReuqest访问web资源，POST方式
        /// </summary>
        /// <param name="url">页面地址</param>
        /// <param name="parameterDictionary">参数集合</param>
        /// <param name="cookieContainer">Cookie容器</param>
        /// <returns>web资源string</returns>
        public static string HttpWebReuqestPost(string url, Dictionary<string, string> parameterDictionary, CookieContainer cookieContainer)
        {
            StringBuilder parameterStringBuilder = new StringBuilder(string.Empty);
            bool isFirstParameter = true;
            if (parameterDictionary != null) 
            {
                foreach (KeyValuePair<string, string> item in parameterDictionary)
                {
                    parameterStringBuilder
                        .Append(!isFirstParameter ? "&" : "")
                        .Append(item.Key)
                        .Append("=")
                        .Append(item.Value);
                    isFirstParameter = false;
                }
            }
            string parameterString = parameterStringBuilder.ToString();
            try
            {
                byte[] parameterByteArray = Encoding.ASCII.GetBytes(parameterString);
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                httpWebRequest.Accept = "*/*";
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                httpWebRequest.KeepAlive = true;
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 5000 * 2;
                httpWebRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                httpWebRequest.ContentLength = parameterByteArray.Length;
                httpWebRequest.CookieContainer = cookieContainer;
                Stream requestStream = httpWebRequest.GetRequestStream();
                requestStream.Write(parameterByteArray, 0, parameterByteArray.Length);
                requestStream.Close();
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                string webString = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();
                httpWebResponse.Close();
                cookieContainer.Add(httpWebRequest.CookieContainer.GetCookies(httpWebRequest.RequestUri));
                return webString;
            }
            catch (Exception exception)
            {
                Log4NetUtils.Error("Common.Common", "HttpWebReuqestPost访问失败，url：" + url, exception);
                return null;
            }
        }

        #endregion

        #region Unix时间戳

        /// <summary>
        /// 获取当前时间的时间戳
        /// </summary>
        /// <returns></returns>
        public static long DatetimeToUnixTimestamp()
        {
            return Common.DatetimeToUnixTimestamp(DateTime.Now);
        }

        /// <summary>
        /// 获取指定时间的时间戳
        /// </summary>
        /// <param name="datetime">指定的Datetime对象</param>
        /// <returns></returns>
        public static long DatetimeToUnixTimestamp(DateTime datetime)
        {
            DateTime baseTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (long)((datetime - baseTime).TotalMilliseconds);
        }

        /// <summary>
        /// 将Unix时间戳转化为时间
        /// </summary>
        /// <param name="unixTimestamp">时间戳</param>
        /// <returns></returns>
        public static DateTime UnixTimestampToDatetime(long unixTimestamp)
        {
            DateTime time = System.DateTime.MinValue;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            time = startTime.AddMilliseconds(unixTimestamp);
            return time;
        }

        #endregion

        #region md5加密

        
        #endregion
    }
}
