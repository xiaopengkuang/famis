using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOM.Models
{
    public class NavigationInfo
    {
        public string Title { set; get; }

        public string ActionName { set; get; }

        public string ControllerName { set; get; }

        public object Values { set; get; }

        public void SetNavInfo(string title, string actionName, string controllerName, object values)
        {

            Title = title;

            ActionName = actionName;

            ControllerName = controllerName;

            Values = values;

        }

        public NavigationInfo()

        { }

        public NavigationInfo(string title, string actionName, string controllerName, object values)
        {

            this.SetNavInfo(title, actionName, controllerName, values);

        }

        public NavigationInfo(string title, string actionName, string controllerName)
        {

            this.SetNavInfo(title, actionName, controllerName, null);

        }

        public NavigationInfo(string title, string actionName, object values)
        {

            this.SetNavInfo(title, actionName, null, values);

        }

        public NavigationInfo(string title)
        {

            this.SetNavInfo(title, null, null, null);

        }

    }
}