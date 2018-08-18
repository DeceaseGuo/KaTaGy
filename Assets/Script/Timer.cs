namespace MyCode.Timer
{
    using System;
    using System.Collections;
    using UnityEngine;


    public class Timer
    {
        #region 自定義間隔時間
        public static IEnumerator Start(float duration, Action callback)
        {
            return Start(duration, false, callback);
        }
        /// <param name="duration">間隔時間</param>
        /// <param name="repeat">是否一直調用</param>
        /// <param name="callback">執行方法</param>
        /// <returns></returns>
        public static IEnumerator Start(float duration, bool repeat, Action callback)
        {
            do
            {
                yield return new WaitForSeconds(duration);

                if (callback != null)
                    callback();

            } while (repeat);
        }

        public static IEnumerator FirstAction(float duration, Action callback)
        {
            while (true)
            {
                if (callback != null)
                    callback();

                yield return new WaitForSeconds(duration);
            } 
        }

        public static IEnumerator Start_MoreFunction(float duration_1, Action callback_1, float duration_2, Action callback_2)
        {
            yield return new WaitForSeconds(duration_1);
            if (callback_1 != null)
                callback_1();
            yield return new WaitForSeconds(duration_2);
            if (callback_2 != null)
                callback_2();
        }
        #endregion

        #region 過一段時間後執行
       /* public static IEnumerator StartRealtime(float time, Action callback)
        {
            float start = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < start + time)
            {
                yield return null;
            }
            if (callback != null) callback();
            
        }*/
        #endregion

        public static IEnumerator NextFrame(Action callback)
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                if (callback != null)
                    callback();
            }
        }
    }
}