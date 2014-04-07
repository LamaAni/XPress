using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XPress.Web.Controls;
using XPress.Web.Controls.Linq;
using XPress.Web.Html;
using XPress.Web.Html.Linq;
using XPress.Web.JCom.Attributes;



namespace WebTester.Controls
{
    public class PlaceHolder : XPressControl
    {
        public PlaceHolder(string tagName="div")
            :base(tagName)
        {
        }

        

        [ClientSideMethod()]
        protected void sendvar(string data)
        {
            DateTime sentDate = new DateTime(1970, 1, 1).AddMilliseconds(double.Parse(data));

            System.Threading.Thread.Sleep(1000);
            this.Children.Clear();
            this.Write("Tomorrow is:" + sentDate.AddDays(1).DayOfWeek);
            this.UpdateClient();
            //return "Time on server:" + DateTime.Now;
        }
    }
}