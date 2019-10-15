using System;
using System.Collections.Generic;
using System.Linq;

namespace fuliu.pseudocode
{
    /// <summary>
    /// 格式如“AbnormalKillHuman(RandomHuman(SubtractSet(GetAllHumans(2),NewSet("{100008}"))));CloseDoor(SelectDoor(false));OpenDoor(SelectDoor(true));”
    /// “DefineVar(A,10);If(IsGreaterOrEqualsTo(A,5),NewAction(CloseDoor(2);OpenLuDeng(3);),NewAction(CheckWin();));MurdererToKill();”
    /// </summary>
    public class CPseudocodeParser : IParser
    {
        public PseudocodeFunc[] parse(string funcsStr)
        {
            var sentences = SplitToSentences(funcsStr);
            List<PseudocodeFunc> funcs = new List<PseudocodeFunc>(sentences.Length);
            for (int i = 0; i < sentences.Length; i++)
                funcs.Add(CreatePseudocodeFunc(sentences[i]));

            return funcs.ToArray();
        }
        
        private PseudocodeFunc[] SplitParams(string funcsStr, string sentenceSeperater)
        {
            var sentences = SplitToSentences(funcsStr, sentenceSeperater);
            List<PseudocodeFunc> funcs = new List<PseudocodeFunc>(sentences.Length);
            for (int i = 0; i < sentences.Length; i++)
                funcs.Add(CreatePseudocodeFunc(sentences[i]));

            return funcs.ToArray();
        }

        private PseudocodeFunc CreatePseudocodeFunc(string sentence)
        {
            var res = new PseudocodeFunc();
            if (sentence.Contains('('))
            {
                res.funcName = sentence.Substring(0, sentence.IndexOf('(')).Trim();
                var funcContent = sentence.SplitByFirstAndLastSymbol("(", ")");
                switch (res.funcName)
                {
                    case "NewAction": 
                        res.result = new PseudocodeAction(funcContent,this);
                        res.type = "Action";
                        res.needCompute = false;
                        break;
                    case "NewFunc": 
                        res.result = CreatePseudocodeFunc(funcContent);
                        res.type = "Func"; // 注意这里是大些，此种类型每次循环都要执行
                        res.needCompute = false;
                        break;
                    default: 
                        res.funcParamFuncs = SplitParams(funcContent, ",");
                        res.type = "func";
                        res.needCompute = true;
                        break;
                }
            }
            else
            {
                res.funcName = sentence.Trim();
                res.type = "func";
                res.needCompute = true;
                //
                if (int.TryParse(res.funcName, out int resInt))
                {
                    res.result = resInt;
                    res.type = "int";
                    res.needCompute = false;
                }
                if (float.TryParse(res.funcName, out float resFloat))
                {
                    res.result = resFloat;
                    res.type = "float";
                    res.needCompute = false;
                }
                if (bool.TryParse(res.funcName, out bool resBool))
                {
                    res.result = resBool;
                    res.type = "bool";
                    res.needCompute = false;
                }
                if (res.funcName.Contains("\""))
                {
                    res.result = res.funcName.SplitByFirstAndLastSymbol("\"", "\"");
                    res.type = "string";
                    res.needCompute = false;
                }
            }

            return res;
        }

        /// <summary>
        /// 句子间以;隔开，所以用;符号分割成多个句子。处理后每个句子的格式都是 functionName(...) 或 functionName。不包含空句子
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        string[] SplitToSentences(string txt, string sentenceSeperater = ";")
        {
            var oss = txt.Split(new []{sentenceSeperater}, StringSplitOptions.None);
            // 为了防止方法内的含有;符号而导致分割不正确，所以这里做了处理
            List<string> ss = new List<string>(oss.Length);
            bool isOk = true;
            for (int i = 0; i < oss.Length; i++)
            {
                if (isOk)
                {
                    if (string.IsNullOrWhiteSpace(oss[i]))
                    {
                        continue;
                    }
                    if (oss[i].ContainsCount('(') == oss[i].ContainsCount(')'))
                    {
                        ss.Add(oss[i]);
                    }
                    else
                    {
                        isOk = false;
                        ss.Add(oss[i]);
                    }
                }
                else
                {
                    var newStr = ss.Last() + sentenceSeperater + oss[i];
                    isOk = newStr.ContainsCount('(') == newStr.ContainsCount(')');
                    ss[ss.Count - 1] = newStr;
                }
            }

            return ss.ToArray();
        }
    }
}