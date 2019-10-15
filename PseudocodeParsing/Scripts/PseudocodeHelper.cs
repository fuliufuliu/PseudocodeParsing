using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace fuliu.pseudocode
{
    public static class PseudocodeHelper
    {
        private static Dictionary<string, MethodInfo> methodDic;
        private static IParser defaultParser;

        static PseudocodeHelper()
        {
            AddPseudocodeFuncs(typeof(DefaultFuncs));
            defaultParser = new CPseudocodeParser();
        }
        
        public static void SetParser(IParser parser)
        {
            defaultParser = parser ?? defaultParser;
        }
        
        public static void SetPseudocodeFuncs(string funcName, MethodInfo method)
        {
            if (funcName != null)
            {
                if (method == null)
                {
                    methodDic.Remove(funcName);
                }
                else
                {
                    if (methodDic.ContainsKey(funcName))
                    {
                        Debug.LogWarning($"已存在名为 {funcName} 的方法！{method} 将覆盖原有的方法！" +
                                         "若不想覆盖，请使用 ignoreMethodNames参数 来忽略重名的方法！");
                    }
                    methodDic[funcName] = method;
                }
            }
        }

        private static void CheckExisting()
        {
            
        }

        public static void AddPseudocodeFuncs<T>(IList<string> ignoreMethodNames = null)
        {
            AddPseudocodeFuncs(typeof(T), ignoreMethodNames);
        }
        
        public static void AddPseudocodeFuncs(Type type, IList<string> ignoreMethodNames = null)
        {
            var methods = type.GetMethods();
            if (methodDic == null)
            {
                methodDic = new Dictionary<string, MethodInfo>(methods.Length);
            }
            for (int i = 0; i < methods.Length; i++)
            {
                if (ignoreMethodNames != null && ignoreMethodNames.Contains(methods[i].Name))
                {
                    continue;
                }

                if (methodDic.ContainsKey(methods[i].Name))
                {
                    Debug.LogWarning($"已存在名为 {methods[i].Name} 的方法！{type}中的同名方法将覆盖原有的方法！" +
                                     "若不想覆盖，请使用 ignoreMethodNames参数 来忽略重名的方法！");
                }
                methodDic[methods[i].Name] = methods[i];
            }
        }

        public static MethodInfo GetMethod(string funName)
        {
            return methodDic.ContainsKey(funName) ? methodDic[funName] : null;
        }

        /// <summary>
        /// 使用者保证所有伪代码用到的方法都已经通过 AddPseudocodeFuncs 方法添加成功之后，就可以直接运行
        /// </summary>
        /// <param name="pseudocodeStr"></param>
        public static void Run(string pseudocodeStr, Action callback = null)
        {
            new Pseudocode(onCallFunc, pseudocodeStr, new CPseudocodeParser()).Run(callback);
        }

        /// <summary>
        /// 可通过协程中 yeild return 来调用
        /// </summary>
        public static IEnumerator RunCode(string pseudocodeStr)
        {
            return new Pseudocode(onCallFunc, pseudocodeStr, new CPseudocodeParser());
        }
        
        private static void onCallFunc(Pseudocode pseudocode, string funName, Action<object> callback, object[] parameters)
        {
            var func = GetMethod(funName);

            if (func != null)
            {
                func.Invoke(null,new object[]{pseudocode, funName, callback, parameters});
            }
            else
            {
                Debug.LogError($"未找到“伪”方法 “{funName}” 对应的“真”方法！");
                callback(null);
            }
        }
    }
}