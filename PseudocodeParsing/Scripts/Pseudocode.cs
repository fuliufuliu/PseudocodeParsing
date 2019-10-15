using System;
using System.Collections;
using System.Collections.Generic;

namespace fuliu.pseudocode
{
    public interface IParser
    {
        PseudocodeFunc[] parse(string funcsStr);
    }


    /// <summary>
    /// 这是一个装载伪代码的壳， fuliu.pseudocode 利用这个壳，去引导执行伪代码。伪代码对应的真代码需要在 构造函数 参数 onCallFunc 中传入
    /// 请参考 TestPseudocode 中的用法。
    /// PseudocodeHelper 中封装了一些渐变的方法。
    /// 伪代码对应的真代码的写法有些特殊要求，具体参照DefaultFuncs中的写法。
    /// </summary>
    public class Pseudocode : IEnumerator
    {
        public Dictionary<string, object> vars = new Dictionary<string, object>();
        public Action<Pseudocode, string, Action<object>, object[]> onCallFunc;
        public PseudocodeFunc[] funcs;
        private string _script;
        public IParser parser = new CPseudocodeParser();
        public PseudocodeFunc enterFunc;
        /// <summary>
        /// 是否已经执行完
        /// </summary>
        private bool isRuned;

        private bool isRunning;

        public string script
        {
            get => _script;
            set
            {
                _script = value;
                funcs = parse(_script);
                enterFunc = funcs != null && funcs.Length > 0 ? funcs[0] : null;
            }
        }

        public Pseudocode(Action<Pseudocode, string, Action<object>, object[]> onCallFunc)
        {
            this.onCallFunc = onCallFunc;
        }
        
        public Pseudocode(Action<Pseudocode, string, Action<object>, object[]> onCallFunc, string script, IParser parser = null)
        {
            this.onCallFunc = onCallFunc;
            this.script = script;
            if (parser != null)
            {
                this.parser = parser;
            }
            funcs = parse(script);
            enterFunc = funcs != null && funcs.Length > 0 ? funcs[0] : null;
        }
        
        public PseudocodeFunc[] parse(string funcsStr)
        {
            if (parser != null) return parser.parse(funcsStr);
            throw new Exception("Pseudocode 未指定IParser parser");
        }
        
        public Queue<PseudocodeFunc> GetFuncParamFuncsQueue(PseudocodeFunc[] funcParamFuncs)
        {
            Queue<PseudocodeFunc> res = new Queue<PseudocodeFunc>(funcParamFuncs.Length); 
            for (int i = 0; i < funcParamFuncs.Length; i++)
            {
                res.Enqueue(funcParamFuncs[i]);
            }

            return res;
        }
        
        public void Run(Action callback = null)
        {
            if (funcs != null)
            {
                if (funcs.Length > 0)
                {
                    isRunning = true;
                    RunQueue(GetFuncParamFuncsQueue(funcs), ()=>
                    {
                        isRuned = true;
                        callback?.Invoke();
                    });
                }
            }
        }
        
        /// <summary>
        /// 只执行第一个入口处的方法
        /// </summary>
        /// <param name="callback"></param>
        public void RunEnterFunc(Action<object> callback = null)
        {
            if (funcs != null)
            {
                if (funcs.Length > 0)
                {
                    isRunning = true;
                    Run(enterFunc, () =>
                    {
                        isRuned = true;
                        isRunning = false;
                        callback?.Invoke(enterFunc.result);
                    });
                }
            }
        }

        public void Run(PseudocodeFunc pseudocodeFunc, Action callback)
        {
            if (pseudocodeFunc != null)
            {
                    if (pseudocodeFunc.funcParamFuncs != null && pseudocodeFunc.funcParamFuncs.Length > 0)
                    {
                        var callQueue = GetFuncParamFuncsQueue(pseudocodeFunc.funcParamFuncs);
                        RunParamQueue(pseudocodeFunc, callQueue, () =>
                        {
                            onCallFunc(this, pseudocodeFunc.funcName, (res) =>
                            {
                                pseudocodeFunc.result = res;
                                callback?.Invoke();
                            }, pseudocodeFunc.paramResults);
                        });
                    }
                    else
                    {
                        onCallFunc(this, pseudocodeFunc.funcName, (res) =>
                        {
                            pseudocodeFunc.result = res;
                            callback?.Invoke();
                        }, pseudocodeFunc.paramResults);
                    }

            }
        }

        public void RunAll()
        {
            RunQueue(GetFuncParamFuncsQueue(funcs), null);
        }

        public void RunQueue(Queue<PseudocodeFunc> callQueue, Action callback)
        {
            RunParamQueue(null, callQueue, callback);
        }

        private void RunParamQueue(PseudocodeFunc pseudocodeFunc, Queue<PseudocodeFunc> callQueue, Action callback)
        {
            // 初始化
            if (pseudocodeFunc != null && pseudocodeFunc.funcParamFuncs.Length == callQueue.Count)
            {
                pseudocodeFunc.paramResults = new object[pseudocodeFunc.funcParamFuncs.Length];
            }
            if (callQueue.Count > 0)
            {
                var func = callQueue.Dequeue();
                if (func.needCompute)
                {
                    Run(func, () =>
                    {
                        if (pseudocodeFunc != null)
                        {
                            pseudocodeFunc.paramResults[pseudocodeFunc.funcParamFuncs.Length - callQueue.Count - 1] =
                                func.result;
                        }
                        // 递归
                        RunParamQueue(pseudocodeFunc, callQueue, callback);
                    });
                }
                else
                {
                    if (pseudocodeFunc != null)
                    {
                        //
                        pseudocodeFunc.paramResults[pseudocodeFunc.funcParamFuncs.Length - callQueue.Count - 1] =
                            func.result;
                    }
                    // 递归
                    RunParamQueue(pseudocodeFunc, callQueue, callback);
                }
            }
            else
            {
                callback?.Invoke();
            }
        }

        public bool MoveNext()
        {
            if (! isRunning)
            {
                Run();
            }
            return !isRuned;
        }

        public void Reset()
        {
            isRuned = false;
            isRunning = false;
        }

        public object Current { get; set; }
    }
    
    /// <summary>
    /// 方法，可以有返回值
    /// </summary>
    public class PseudocodeFunc
    {
        /// <summary>
        /// 方法名
        /// </summary>
        public string funcName;
        /// <summary>
        /// 参数方法
        /// </summary>
        public PseudocodeFunc[] funcParamFuncs;
        /// <summary>
        /// 参数运行结果
        /// </summary>
        public object[] paramResults = new object[0];
        /// <summary>
        /// 运行结果
        /// </summary>
        public object result;
        /// <summary>
        /// 这个对象的实际类型
        /// </summary>
        public string type;
        /// <summary>
        /// 是否需要计算
        /// </summary>
        public bool needCompute = true;
    }

    /// <summary>
    /// 无返回值的代码块
    /// </summary>
    public class PseudocodeAction
    {
        public PseudocodeFunc[] funcs;

        public PseudocodeAction(string funcsStr, IParser parser)
        {
            funcs = parser.parse(funcsStr);
        }
    }
}