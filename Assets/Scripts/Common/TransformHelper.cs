using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    ///<summary>
    ///变换组件助手类
    ///</summary>
    public static class TransformHelper
    {
        /// <summary>
        /// 未知层级，查找后代指定名称的变换组件
        /// </summary>
        /// <param name="currentTF">当前变换组件</param>
        /// <param name="childName">后代物体名称</param>
        /// <returns></returns>
        // this 表示扩展方法 给第一个参数的类型Transform扩展方法
        public static Transform FindChildByName(this Transform currentTF, string childName)
        {
            //递归：方法内部又调用自身的过程
            //1.在子物体中查找
            Transform childTF = currentTF.Find(childName);
            if (childTF != null) return childTF;

            for (int i = 0; i < currentTF.childCount; i++)
            {
                //2.将任务交给子物体
                childTF = FindChildByName(currentTF.GetChild(i), childName);
                if (childTF != null) return childTF;
            }

            return null;
        }
    }
}
