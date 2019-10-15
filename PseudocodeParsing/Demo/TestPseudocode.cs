using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace fuliu.pseudocode
{

    public class TestPseudocode : MonoBehaviour
    {
        private int count;
        public enum ExcuteMode
        {
            CoroutineExcute,
            CommonExcute,
        }

        public ExcuteMode mode;
        
        /// 格式如“AbnormalKillHuman(RandomHuman(SubtractSet(GetAllHumans(2),NewSet("{100008}"))));CloseDoor(SelectDoor(false));OpenDoor(SelectDoor(true));”
        /// “A=10;If(IsGreaterOrEqualsTo(A,5),NewAction(CloseDoor(2);OpenLuDeng(3);),NewAction(CheckWin();));MurdererToKill();”
        [SerializeField]
        private List<string> codeList = new List<string>
        {
            "If(IsGreaterOrEqualsTo(10,5),NewAction(CloseDoor(2);OpenLuDeng(3);),NewAction(CheckWin();));MurdererToKill();",
            "SetVar(\"A\", 0); While( NewFunc(IsLessTo(GetVar(\"A\"), 10)), NewAction(CloseDoor(2);OpenLuDeng(3);SetVar(\"A\", Add(GetVar(\"A\"), 1)) ;Print(GetVar(\"A\")); ));MurdererToKill();",
            "AbnormalKillHuman(RandomHuman(SubtractSet(GetAllHumans(2),NewSet(\"{100008}\"))));CloseDoor(SelectDoor(false));OpenDoor(SelectDoor(true));",
        };

        private void Start()
        {
            if (mode == ExcuteMode.CommonExcute)
            {
                for (int i = 0; i < codeList.Count; i++)
                {
                    var codeStr = codeList[i];
                    Debug.Log($"执行伪代码： {codeStr}");
                    PseudocodeHelper.Run(codeStr);
                }
            }
            else if (mode == ExcuteMode.CoroutineExcute)
            {
                StartCoroutine(Routine(codeList));
            }
        }

        private IEnumerator Routine(List<string> codeStrList)
        {
            var codeStrListTemp = codeStrList.ToList();
            while (codeStrListTemp.Count > 0)
            {
                var codeStr = codeStrListTemp[0];
                Debug.Log($"执行伪代码： {codeStr}");
                yield return PseudocodeHelper.RunCode(codeStr);
                yield return new WaitForSeconds(1);
                codeStrListTemp.RemoveAt(0);
            }
        }
    }
}