using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace stationeryapp.Util
{
    public class GenerateID
    {
        public static string CreateNewId(List<string> idList)
        {
            if (idList.Count == 0)
            {
                return "1";
            }

            List<int> convertedList = new List<int>();

            for (int i = 0; i < idList.Count; i++)
            {
                convertedList.Add(Convert.ToInt32(idList[i]));
            };

            return (convertedList.Max() + 1).ToString();
        }
    }
}