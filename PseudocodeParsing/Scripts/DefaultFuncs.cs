using System;
using System.Collections.Generic;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;
using UnityEngine;

namespace fuliu.pseudocode
{
    public static class DefaultFuncs
    {
        public static void IsGreaterOrEqualsTo(Pseudocode Pseudocode, string funcName, Action<object> callback, object[] parameters)
        {
            float a = (float)parameters[0];
            float b = (float)parameters[1];
        
            callback?.Invoke(a >= b);
        }
        
        public static void IsGreaterTo(Pseudocode Pseudocode, string funcName, Action<object> callback, object[] parameters)
        {
            float a = (float)parameters[0];
            float b = (float)parameters[1];
        
            callback?.Invoke(a > b);
        }
        
        public static void IsLessOrEqualsTo(Pseudocode Pseudocode, string funcName, Action<object> callback, object[] parameters)
        {
            float a = (float)parameters[0];
            float b = (float)parameters[1];
        
            callback?.Invoke(a <= b);
        }
        
        public static void IsLessTo(Pseudocode Pseudocode, string funcName, Action<object> callback, object[] parameters)
        {
            float a = (float)parameters[0];
            float b = (float)parameters[1];
        
            callback?.Invoke(a < b);
        }
        
        public static void IsEqualsTo(Pseudocode Pseudocode, string funcName, Action<object> callback, object[] parameters)
        {
            float a = (float)parameters[0];
            float b = (float)parameters[1];
        
            callback?.Invoke(Math.Abs(a - b) < 0.0000001f);
        }

        public static void SetVar(Pseudocode Pseudocode, string funcName, Action<object> callback, object[] parameters)
        {
            string varName = (string)parameters[0];
            object varValue = parameters[1];

            Pseudocode.vars[varName] = varValue;
            callback?.Invoke(varValue);
        }
        
        public static void GetVar(Pseudocode Pseudocode, string funcName, Action<object> callback, object[] parameters)
        {
            string varName = (string)parameters[0];

            callback?.Invoke(Pseudocode.vars[varName]);
        }
        
        public static void RemoveVar(Pseudocode Pseudocode, string funcName, Action<object> callback, object[] parameters)
        {
            string varName = (string)parameters[0];

            var res = Pseudocode.vars[varName];
            Pseudocode.vars.Remove(varName);
            callback?.Invoke(res);
        }
        
        public static void If(Pseudocode Pseudocode, string funcName, Action<object> callback, object[] parameters)
        {
            bool isTrue = (bool)parameters[0];
            PseudocodeAction TrueDoSth = (PseudocodeAction)parameters[1];
            PseudocodeAction FalseDoSth = (PseudocodeAction)parameters[2];

            if (isTrue)
            {
                if (TrueDoSth != null)
                    Pseudocode.RunQueue(Pseudocode.GetFuncParamFuncsQueue(TrueDoSth.funcs),
                        () => { callback?.Invoke(null); });
                else
                {
                    callback?.Invoke(null); 
                }
            }
            else
            {
                if (FalseDoSth != null)
                    Pseudocode.RunQueue(Pseudocode.GetFuncParamFuncsQueue(FalseDoSth.funcs),
                        () => { callback?.Invoke(null); });
                else
                {
                    callback?.Invoke(null); 
                }
            }
        }
        
        public static void IF(Pseudocode Pseudocode, string funcName, Action<object> callback, object[] parameters)
        {
            if (parameters[0] is bool)
            {
                throw new Exception("定义IF方法的条件参数，需要使用方法: NewFunc(表达式)");
            }
            PseudocodeFunc isTruePseudocodeFunc = (PseudocodeFunc) parameters[0];
            PseudocodeAction TrueDoSth = (PseudocodeAction)parameters[1];
            PseudocodeAction FalseDoSth = (PseudocodeAction)parameters[2];

            Pseudocode.Run(isTruePseudocodeFunc, () =>
            {
                if ((bool) isTruePseudocodeFunc.result)
                {

                    if (TrueDoSth != null)
                        Pseudocode.RunQueue(Pseudocode.GetFuncParamFuncsQueue(TrueDoSth.funcs),
                            () => { callback?.Invoke(null); });
                    else
                    {
                        callback?.Invoke(null);
                    }
                }
                else
                {
                    if (FalseDoSth != null)
                        Pseudocode.RunQueue(Pseudocode.GetFuncParamFuncsQueue(FalseDoSth.funcs),
                            () => { callback?.Invoke(null); });
                    else
                    {
                        callback?.Invoke(null);
                    }
                }
            });
        }

        public static void While(Pseudocode Pseudocode, string funcName, Action<object> callback, object[] parameters)
        {
            if (parameters[0] is bool)
            {
                throw new Exception("定义While方法的条件参数，需要使用方法: NewFunc(表达式)");
            }
            PseudocodeFunc isTruePseudocodeFunc = (PseudocodeFunc) parameters[0];
            PseudocodeAction LoopDoSth = (PseudocodeAction) parameters[1];

            Pseudocode.Run(isTruePseudocodeFunc, () =>
            {
                if ((bool) isTruePseudocodeFunc.result)
                {
                    if (LoopDoSth != null)
                    {
                        Pseudocode.RunQueue(Pseudocode.GetFuncParamFuncsQueue(LoopDoSth.funcs),
                            () => { While(Pseudocode, funcName, callback, parameters); });
                    }
                    else
                    {
                        callback?.Invoke(null);
                    }
                }
                else
                {
                    callback?.Invoke(null);
                }
            });
        }
        
        public static void Add(Pseudocode Pseudocode, string funcName, Action<object> callback, object[] parameters)
        {
            float a = (float)parameters[0];
            float b = (float)parameters[1];
        
            callback?.Invoke(a + b);
        }
        
        public static void Subtract(Pseudocode Pseudocode, string funcName, Action<object> callback, object[] parameters)
        {
            float a = (float)parameters[0];
            float b = (float)parameters[1];
        
            callback?.Invoke(a - b);
        }
        
        public static void SelfAdd(Pseudocode Pseudocode, string funcName, Action<object> callback, object[] parameters)
        {
            float a = (float)parameters[0];
        
            callback?.Invoke(++a);
        }
        
        public static void SelfSubtract(Pseudocode Pseudocode, string funcName, Action<object> callback, object[] parameters)
        {
            float a = (float)parameters[0];
        
            callback?.Invoke(--a);
        }
        
        
        public static void Multiply(Pseudocode Pseudocode, string funcName, Action<object> callback, object[] parameters)
        {
            float a = (float)parameters[0];
            float b = (float)parameters[1];
        
            callback?.Invoke(a * b);
        }
        
        public static void Divide(Pseudocode Pseudocode, string funcName, Action<object> callback, object[] parameters)
        {
            float a = (float)parameters[0];
            float b = (float)parameters[1];
        
            callback?.Invoke(a / b);
        }
        
        public static void Remaind(Pseudocode Pseudocode, string funcName, Action<object> callback, object[] parameters)
        {
            float a = (float)parameters[0];
            float b = (float)parameters[1];

            callback?.Invoke(a % b);
        }
        
        public static void Print(Pseudocode Pseudocode, string funcName, Action<object> callback, object[] parameters)
        {
            string s = parameters[0].ToString();
            Debug.Log(s);
            callback?.Invoke(null);
        }
        
        public static void And(Pseudocode Pseudocode, string funcName, Action<object> callback, object[] parameters)
        {
            bool a = (bool)parameters[0];
            bool b = (bool)parameters[1];
        
            callback?.Invoke(a && b);
        }
        
        public static void Or(Pseudocode Pseudocode, string funcName, Action<object> callback, object[] parameters)
        {
            bool a = (bool)parameters[0];
            bool b = (bool)parameters[1];
        
            callback?.Invoke(a || b);
        }
        
        public static void Not(Pseudocode Pseudocode, string funcName, Action<object> callback, object[] parameters)
        {
            bool a = (bool)parameters[0];
        
            callback?.Invoke(! a);
        }
    }
    
}