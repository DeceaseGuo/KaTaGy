namespace MyCode.Timer
{
    using System;
    using System.Collections;
    using UnityEngine;


    public class Timer
    {
        #region 自定義間隔時間
      /*  public static IEnumerator Start(float duration, Action callback)
        {
            return Start(duration, false, callback);
        }*/
        /// <param name="duration">間隔時間</param>
        /// <param name="repeat">是否一直調用</param>
        /// <param name="callback">執行方法</param>
        /// <returns></returns>
        public static IEnumerator Start(float duration, bool repeat, Action callback)
        {
            WaitForSeconds delay = new WaitForSeconds(duration);
            do
            {
                yield return delay;

                if (callback != null)
                    callback();

            } while (repeat);
        }

        public static IEnumerator FirstAction(float duration, Action callback)
        {
            WaitForSeconds delay = new WaitForSeconds(duration);
            while (true)
            {
                if (callback != null)
                    callback();

                yield return delay;
            } 
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
            WaitForEndOfFrame everyDelay = new WaitForEndOfFrame();
            while (true)
            {
                yield return everyDelay;
                if (callback != null)
                    callback();
            }
        }
    }
}