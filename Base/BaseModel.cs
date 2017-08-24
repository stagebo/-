using Common.Log4Net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Base
{
    [Serializable]
    public abstract class BaseModel
    {
        #region InitializeModel

        /// <summary>
        /// 初始化Model对象
        /// </summary>
        /// <typeparam name="T">BaseModel子类</typeparam>
        /// <param name="jsonString">Json字符串</param>
        /// <returns></returns>
        public virtual T InitializeModel<T>(string jsonString) where T : BaseModel
        {
            if (!(this is T))
            {
                LogUtils.Error(this, "泛型类型与当前this对象类型不符");
                return null;
            }
            try
            {
                Dictionary<string, object> jsonObject = Common.Common.DeserializeJsonString<Dictionary<string, object>>(jsonString);
                return this.InitializeModel<T>(jsonObject);
            }
            catch (Exception exception)
            {
                LogUtils.Error(this, "初始化Model对象失败，jsonString：" + jsonString, exception);
                return (T)this;
            }
        }

        /// <summary>
        /// 初始化Model对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataDictionary">初始化数据集合</param>
        /// <returns></returns>
        public virtual T InitializeModel<T>(Dictionary<string, object> dataDictionary) where T : BaseModel
        {
            try
            {
                Type type = this.GetType();
                PropertyInfo[] propertyInfoList = type.GetProperties();
                foreach (PropertyInfo propertyInfo in propertyInfoList)
                {
                    if(dataDictionary.ContainsKey(propertyInfo.Name))
                    {
                        if (dataDictionary[propertyInfo.Name] != null) 
                        {
                            Type convertsionType = propertyInfo.PropertyType;
                            if (convertsionType.Name == "Guid")
                            {
                                propertyInfo.SetValue(this, Guid.Parse(Uri.UnescapeDataString(dataDictionary[propertyInfo.Name].ToString())), null);
                            }
                            else
                            {
                                if (convertsionType.IsGenericType && convertsionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                                {
                                    NullableConverter nullableConverter = new NullableConverter(convertsionType);
                                    convertsionType = nullableConverter.UnderlyingType;
                                }
                                if (convertsionType.Name == "Guid")
                                {
                                    propertyInfo.SetValue(this, Guid.Parse(Uri.UnescapeDataString(dataDictionary[propertyInfo.Name].ToString())), null);
                                }
                                else
                                {
                                    propertyInfo.SetValue(this, Convert.ChangeType(Uri.UnescapeDataString(dataDictionary[propertyInfo.Name].ToString()), convertsionType), null);
                                }
                            }
                        }
                    }
                }
                return (T)this;
            }
            catch (Exception exception)
            {
                LogUtils.Error(this, "初始化Model对象失败", exception);
                return (T)this;
            }
        }

        /// <summary>
        /// 初始化Model对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static List<T> InitializeModelList<T>(string jsonString) where T : BaseModel
        {
            List<Dictionary<string, object>> jsonObjectList = null;
            try
            {
                jsonObjectList = Common.Common.DeserializeJsonString<List<Dictionary<string, object>>>(jsonString);
            }
            catch (Exception exception)
            {
                LogUtils.Error("Common.BaseModel", "初始化Model对象集合失败，jsonString：" + jsonString, exception);
                return null;
            }
            try
            {
                ConstructorInfo defaultConstructorInfo = typeof(T).GetConstructor(new Type[0]);
                List<T> modelList = new List<T>();
                int index = 0;
                foreach (Dictionary<string, object> jsonObjectItem in jsonObjectList)
                {
                    T model = ((T)defaultConstructorInfo.Invoke(new object[0])).InitializeModel<T>(jsonObjectItem);
                    if (model == null)
                    {
                        throw new Exception("初始化Model对象失败，index：" + index);
                    }
                    modelList.Add(model);
                    index++;
                }
                return modelList;
            }
            catch (Exception exception)
            {
                LogUtils.Error("Common.BaseModel", "初始化Model对象集合失败，jsonString：" + jsonString, exception);
                return null;
            }
        }

        /// <summary>
        /// 初始化Model对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonObjectList"></param>
        /// <returns></returns>
        public static List<T> InitializeModelList<T>(ArrayList jsonObjectList) where T : BaseModel
        {
            try
            {
                ConstructorInfo defaultConstructorInfo = typeof(T).GetConstructor(new Type[0]);
                List<T> modelList = new List<T>();
                int index = 0;
                foreach (Dictionary<string, object> jsonObjectItem in jsonObjectList)
                {
                    T model = ((T)defaultConstructorInfo.Invoke(new object[0])).InitializeModel<T>(jsonObjectItem);
                    if (model == null)
                    {
                        throw new Exception("初始化Model对象失败，index：" + index);
                    }
                    modelList.Add(model);
                    index++;
                }
                return modelList;
            }
            catch (Exception exception)
            {
                LogUtils.Error("Common.BaseModel", "初始化Model对象集合失败", exception);
                return null;
            }
        }

        /// <summary>
        /// 初始化Model对象
        /// </summary>
        /// <typeparam name="T">BaseModel子类</typeparam>
        /// <param name="dataCollection">初始化数据集合</param>
        /// <returns></returns>
        public virtual T InitializeModel<T>(NameValueCollection dataCollection) where T : BaseModel
        {
            try
            {
                Type type = this.GetType();
                PropertyInfo[] propertyInfoList = type.GetProperties();
                foreach (PropertyInfo propertyInfo in propertyInfoList)
                {
                    if (dataCollection[propertyInfo.Name] != null)
                    {
                        Type convertsionType = propertyInfo.PropertyType;
                        if (convertsionType.Name == "Guid")
                        {
                            propertyInfo.SetValue(this, Guid.Parse(dataCollection[propertyInfo.Name].ToString()), null);
                        }
                        else
                        {
                            if (convertsionType.IsGenericType && convertsionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                            {
                                NullableConverter nullableConverter = new NullableConverter(convertsionType);
                                convertsionType = nullableConverter.UnderlyingType;
                            }
                            if (convertsionType.Name == "Guid")
                            {
                                propertyInfo.SetValue(this, Guid.Parse(dataCollection[propertyInfo.Name].ToString()), null);
                            }
                            else
                            {
                                propertyInfo.SetValue(this, Convert.ChangeType(dataCollection[propertyInfo.Name], convertsionType), null);
                            }
                        }
                    }
                }
                return (T)this;
            }
            catch (Exception exception)
            {
                LogUtils.Error(this, "初始化Model对象失败", exception);
                return null;
            }
        }

        #endregion

        #region ToJsonString

        /// <summary>
        /// 返回表示Model对象的Json字符串（闭合结构）
        /// </summary>
        /// <returns></returns>
        public virtual string ToJsonString()
        {
            return this.ToJsonString(true);
        }

        /// <summary>
        /// 返回表示Model对象的Json字符串
        /// </summary>
        /// <param name="isClose">是否为闭合结构Json</param>
        /// <returns></returns>
        public abstract string ToJsonString(bool isClose);

        /// <summary>
        /// 返回包含集合中所有元素的Json字符串
        /// </summary>
        /// <param name="modelList">Model对象集合</param>
        /// <returns></returns>
        public static string ModelListToJsonString(IEnumerable<BaseModel> modelList)
        {
            StringBuilder jsonStringBuilder = new StringBuilder(string.Empty);
            jsonStringBuilder.Append("[");
            foreach (BaseModel baseModel in modelList)
            {
                jsonStringBuilder.Append(baseModel.ToJsonString()).Append(",");
            }
            if (modelList.Count<BaseModel>() > 0)
            {
                jsonStringBuilder.Remove(jsonStringBuilder.Length - 1, 1);
            }
            jsonStringBuilder.Append("]");
            return jsonStringBuilder.ToString();
        }

        #endregion
    }
}
